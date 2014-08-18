using UnityEngine;
using System.Collections;

public interface ISceneGUI
{
	
#if UNITY_EDITOR
	void OnSceneGUI();
#endif

}
