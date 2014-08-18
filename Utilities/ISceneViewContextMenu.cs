using UnityEngine;
using System.Collections;

public interface ISceneViewContextMenu
{
	
#if UNITY_EDITOR
	void AddContextMenuItems(ContextMenuConstructor constructor);
#endif

}
