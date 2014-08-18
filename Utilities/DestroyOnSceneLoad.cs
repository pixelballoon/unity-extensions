using UnityEngine;

[ExecuteInEditMode]
public class DestroyOnSceneLoad : MonoBehaviour, ISceneLoaded
{

	private string _scene;

	public void Start()
	{
		_scene = Application.loadedLevelName;
	}

	public void Update()
	{
		if (_scene != Application.loadedLevelName)
		{
			Destroy();
		}
	}

	public void OnDestroy()
	{
		Destroy(false);
	}

	public void OnSceneLoaded()
	{
		Destroy();
	}

	private void Destroy(bool destroyThis = true)
	{
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		
		if (meshFilter && meshFilter.sharedMesh)
		{
			DestroyImmediate(meshFilter.sharedMesh);
		}

		if (destroyThis)
		{
			DestroyImmediate(gameObject);
		}
	}

}
