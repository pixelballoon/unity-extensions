using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

class AOBaking : AssetPostprocessor
{
	private const int NumSamples = 50;
	private const int NumTriangleSamples = 10;

	protected void OnPostprocessModel(GameObject gameObject)
	{
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			OnPostprocessModel(gameObject.transform.GetChild(i).gameObject);
		}

		if (gameObject.transform.root.name.Contains("_RC"))
		{
			ResetColor(gameObject);
		}

		if (gameObject.transform.root.name.Contains("_AO"))
		{
			AddAmbientOcclusion(gameObject, gameObject.transform.root.name.Contains("_ARC"));
		}
	}

	private void ResetColor(GameObject gameObject)
	{
		var meshFilter = gameObject.GetComponent<MeshFilter>();

		if (!meshFilter)
			return;

		var mesh = meshFilter.sharedMesh;

		var newColors = new List<Color>();

		for (int i = 0; i < mesh.colors.Length; i++)
		{
			newColors.Add(Color.black);
		}

		mesh.colors = newColors.ToArray();
	}

	private void AddAmbientOcclusion(GameObject gameObject, bool addRandomColor)
	{
		Random.seed = 0;

		var meshFilter = gameObject.GetComponent<MeshFilter>();

		if (!meshFilter)
			return;

		var mesh = meshFilter.sharedMesh;

		var collider = gameObject.GetComponent<MeshCollider>();
		bool destroyCollider = false;

		if (collider == null)
		{
			destroyCollider = true;
			gameObject.AddComponent<MeshCollider>();
		}

		var samplePoints = new Vector3[NumSamples];

		float sampleDistance = Mathf.Max(mesh.bounds.extents.x, mesh.bounds.extents.y, mesh.bounds.extents.z) * 2f;

		for (var i = 0; i < NumSamples; i++)
		{
			samplePoints[i] = Random.onUnitSphere * sampleDistance;
		}

		var newUvs = new List<Vector2>();

		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			newUvs.Add(new Vector2(0, 0));
		}

		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			int triangle1 = mesh.triangles[i];
			int triangle2 = mesh.triangles[i+1];
			int triangle3 = mesh.triangles[i+2];
			Vector3 vertex1 = mesh.vertices[triangle1];
			Vector3 vertex2 = mesh.vertices[triangle2];
			Vector3 vertex3 = mesh.vertices[triangle3];
			
			Vector3 triangleNormalOffset = mesh.normals[triangle1] * 0.01f;

			var hits = 0f;
			int samplesTaken = 0;

			for (int sample = 0; sample < NumTriangleSamples; sample++)
			{
				// Calculate random position on triangle
				float r1 = Random.Range(0f, 1f);
				float r2 = Random.Range(0f, 1f);
				Vector3 trianglePosition = (1 - Mathf.Sqrt(r1)) * vertex1 + (Mathf.Sqrt(r1) * (1 - r2)) * vertex2 + (Mathf.Sqrt(r1) * r2) * vertex3;

				foreach (var direction in samplePoints)
				{
					if (Vector3.Dot(direction.normalized, triangleNormalOffset.normalized) > 0f)
					{
						samplesTaken++;

						Vector3 start = gameObject.transform.position + trianglePosition + triangleNormalOffset;
						Vector3 end = start + direction;
						if (Physics.Linecast(start, end))
						{
							hits += 1f;
						}
					}
				}
			}

			hits = 1f - hits / (float)(samplesTaken);

			const float colorRamp = 8;
			float ao = Mathf.Round((hits + (addRandomColor ? Random.Range(-0.1f, 0.1f) : 0)) * colorRamp) / colorRamp;

			newUvs[triangle1] = new Vector2(ao, 0);
			newUvs[triangle2] = new Vector2(ao, 0);
			newUvs[triangle3] = new Vector2(ao, 0);
		}

		mesh.uv = newUvs.ToArray();
		mesh.Optimize();

		if (destroyCollider)
		{
			Object.DestroyImmediate(gameObject.GetComponent<MeshCollider>());
		}
	}

}
