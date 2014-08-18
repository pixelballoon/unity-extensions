using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EditorExtensions
{

#if UNITY_EDITOR
	public static void Editor_UnshareMesh(this GameObject gameObject, bool editorOnly)
	{
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

		if (!meshFilter)
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}

		Mesh mesh = meshFilter.sharedMesh;

		if (!mesh)
		{
			mesh = new Mesh();
		}
		else
		{
			foreach (MeshFilter other in Object.FindObjectsOfType<MeshFilter>())
			{
				if (other.gameObject == gameObject)
				{
					continue;
				}

				if (other.sharedMesh == mesh)
				{
					mesh = new Mesh();
					break;
				}
			}
		}

		meshFilter.sharedMesh = mesh;

		if (editorOnly)
		{
			mesh.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	
	public static void Editor_AssignMesh(this MeshFilter meshFilter, Vector3[] vertices, Vector2[] uvs, int[] triangles)
	{
		Mesh mesh = meshFilter.sharedMesh;
		
		if (!mesh)
		{
			mesh = new Mesh();
			meshFilter.sharedMesh = mesh;
		}
		
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.Optimize();
	}

	public static void Editor_RotateToParent(this MonoBehaviour monoBehaviour, float offset = 0f)
	{
		if (Selection.Contains(monoBehaviour.gameObject))
		{
			monoBehaviour.transform.localRotation = Quaternion.Euler(0, 0,
				Mathf.Atan2(monoBehaviour.transform.localPosition.y, monoBehaviour.transform.localPosition.x) * Mathf.Rad2Deg - 90f + offset);
		}
	}
#endif	

	public static void SetStaticRecursive(this GameObject gameObject)
	{
		for (int i=0; i<gameObject.transform.childCount; i++)
		{
			gameObject.transform.GetChild(i).gameObject.SetStaticRecursive();
		}

		gameObject.gameObject.isStatic = true;
	}

}
