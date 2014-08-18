using System;
using System.IO;
using UnityEditor;
using UnityEngine;
public class UtilityWindow : EditorWindow
{

	string _currentScene;

	[MenuItem("Window/Celsian/Utilities")]
	public static void Init()
	{
		// Get existing open window or if none, make a new one:
		UtilityWindow window = GetWindow<UtilityWindow>();
		window.title = "Utilities";
		
		SceneView.onSceneGUIDelegate += OnScene;
	}

	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= OnScene;
	}

	public void OnGUI()
	{
		if (GUILayout.Button("Reload and Save All Scenes"))
		{
			string currentScene = EditorApplication.currentScene;
			EditorApplication.SaveCurrentSceneIfUserWantsTo();

			FileInfo[] sceneInfos = (new DirectoryInfo(Application.dataPath)).GetFiles("*.unity", SearchOption.AllDirectories);
			foreach (FileInfo sceneInfo in sceneInfos)
			{
				Debug.Log("Loading scene " + sceneInfo.FullName);
				
				EditorApplication.OpenScene(sceneInfo.FullName);
				EditorApplication.SaveScene();
			}

			EditorApplication.OpenScene(currentScene);
		}
	}

	public void Update()
	{
		SceneView.onSceneGUIDelegate -= OnScene;
		SceneView.onSceneGUIDelegate += OnScene;

		if (_currentScene != EditorApplication.currentScene)
		{
			_currentScene = EditorApplication.currentScene;

			CallSceneLoaded();
		}
		
		/*
		var view = SceneView.currentDrawingSceneView;
		if (view != null)
		{
			if (view.in2DMode)
			{
				Vector3 position = view.camera.transform.position;
				//view.camera.transform.position = new Vector3(position.x, position.y, -2500);
				view.camera.nearClipPlane = 500;
				view.camera.farClipPlane = 10000;

				view.pivot = new Vector3(view.pivot.x, view.pivot.y, -2500f);


				view.Repaint();
				Debug.Log(view.pivot);
			}
		}*/
	}

	private static void CallSceneLoaded()
	{
		GameObject[] gameObjects = FindObjectsOfType<GameObject>();

		foreach (GameObject go in gameObjects)
		{
			if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
				continue;

			foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
			{
				ISceneLoaded sceneGui = mb as ISceneLoaded;

				if (sceneGui != null)
				{
					sceneGui.OnSceneLoaded();
				}
			}
		}
	}

	private static void OnScene(SceneView sceneview)
	{
		var currentEvent = Event.current;

		if (currentEvent.GetTypeForControl(EditorGUIUtility.GetControlID(FocusType.Passive)) == EventType.keyDown)
		{
			if (currentEvent.keyCode == KeyCode.Space)
			{
				if (ShowContextMenu())
				{
					currentEvent.Use();
				}
			}
		}

		GameObject[] gameObjects = FindObjectsOfType<GameObject>();

		foreach (GameObject go in gameObjects)
		{
			if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
				continue;

			foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
			{
				ISceneGUI sceneGui = mb as ISceneGUI;

				if (sceneGui != null)
				{
					sceneGui.OnSceneGUI();
				}
			}
		}
	}

	private static bool ShowContextMenu()
	{
		GenericMenu menu = new GenericMenu();

		ContextMenuConstructor constructor = new ContextMenuConstructor();

		if (Selection.gameObjects.Length == 0)
		{
			GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
			foreach (GameObject go in objects)
			{
				if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab)
					continue;

				Component[] components = go.GetComponents<Component>();
				foreach (Component component in components)
				{
					ISceneViewGlobalContextMenu contextMenu = component as ISceneViewGlobalContextMenu;
					if (contextMenu != null)
					{
						contextMenu.AddGlobalContextMenuItems(constructor);
					}
				}
			}
		}
		else
		{
			foreach (GameObject go in Selection.gameObjects)
			{
				Component[] components = go.GetComponents<Component>();
				foreach (Component component in components)
				{
					ISceneViewContextMenu contextMenu = component as ISceneViewContextMenu;
					if (contextMenu != null)
					{
						contextMenu.AddContextMenuItems(constructor);
					}
				}
			}
		}

		if (constructor.ConstructMenu(menu))
		{
			menu.ShowAsContext();
			return true;
		}

		return false;
	}
}
