using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DepthShifter : MonoBehaviour, IBakeable, ISceneGUI
{

#if UNITY_EDITOR
	private bool _Editing;

	public void Bake()
	{
		DestroyImmediate(this);
	}

	public void OnSceneGUI()
	{
		if (!Selection.Contains(gameObject))
		{
			_Editing = false;
			return;
		}
		
		var currentEvent = Event.current;
		
		if (currentEvent.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.keyDown)
		{
			if (currentEvent.keyCode == KeyCode.BackQuote || currentEvent.keyCode == KeyCode.Backslash)
			{
				currentEvent.Use();
				_Editing = true;
			}
		}
		else if (currentEvent.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.keyUp)
		{
			if (currentEvent.keyCode == KeyCode.BackQuote || currentEvent.keyCode == KeyCode.Backslash)
			{
				currentEvent.Use();
				_Editing = false;
			}
		}
		else if (currentEvent.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.mouseMove)
		{
			if (_Editing)
			{
				float newZ = transform.localPosition.z + currentEvent.delta.y / 10f;
				newZ = Mathf.Clamp(newZ, -10f, 10f);
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);
				currentEvent.Use();
			}
		}
	}
#endif

}
