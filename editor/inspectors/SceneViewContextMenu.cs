using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Component), true)]
public class SceneViewContextMenu : Editor
{

	protected virtual void OnSceneGUI()
	{
		var currentEvent = Event.current;

		if (currentEvent.GetTypeForControl(EditorGUIUtility.GetControlID(FocusType.Passive)) == EventType.keyDown)
		{
			if (currentEvent.keyCode == KeyCode.Space)
			{
				if (ShowContextMenu())
				{
					currentEvent.Use();
					return;
				}
			}
		}
	}
	
	protected virtual bool ShowContextMenu()
	{
		GenericMenu menu = new GenericMenu();

		ContextMenuConstructor constructor = new ContextMenuConstructor();

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

		if (constructor.ConstructMenu(menu))
		{
			menu.ShowAsContext();
			return true;
		}

		return false;
	}

}
