using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EditorUpdate : MonoBehaviour
{

#if UNITY_EDITOR
	private HashSet<MonoBehaviour> _monoBehaviours;

	protected void Awake()
	{
		if (Application.isPlaying)
			return;

		GameObject[] gameObjects = FindObjectsOfType<GameObject>();

		foreach (GameObject go in gameObjects)
		{
			if (go == null || PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
				continue;

			foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
			{
				IEditorUpdate editorUpdate = mb as IEditorUpdate;

				if (editorUpdate != null)
				{
					editorUpdate.OnEditorInit();
				}
			}
		}
	}

	protected void Update()
	{
		if (Application.isPlaying)
			return;

		foreach (GameObject go in Selection.gameObjects)
		{
			if (go == null || PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
				continue;

			foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
			{
				IEditorUpdate editorUpdate = mb as IEditorUpdate;

				if (editorUpdate != null)
				{
					editorUpdate.OnEditorUpdate();
				}
			}
		}
	}

	protected void OnRenderObject()
	{
		if (Application.isPlaying)
			return;

		if (_monoBehaviours == null)
		{
			_monoBehaviours = new HashSet<MonoBehaviour>();
		}

		if (Selection.gameObjects.Length == 0)
		{
			_monoBehaviours.Clear();
		}
		else
		{
			foreach (GameObject go in Selection.gameObjects)
			{
				if (go == null || PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
					continue;

				foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
				{
					IEditorUpdate editorUpdate = mb as IEditorUpdate;

					if (editorUpdate != null)
					{
						if (!_monoBehaviours.Contains(mb))
						{
							_monoBehaviours.Add(mb);
							editorUpdate.OnEditorInit();
						}
					}
				}
			}
		}
	}
#endif

}
