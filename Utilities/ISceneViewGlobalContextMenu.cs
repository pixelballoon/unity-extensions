using UnityEngine;
using System.Collections;

public interface ISceneViewGlobalContextMenu
{
	
#if UNITY_EDITOR
	void AddGlobalContextMenuItems(ContextMenuConstructor constructor);
#endif

}
