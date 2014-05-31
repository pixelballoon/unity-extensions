using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

public class Baker
{
	#if UNITY_EDITOR
	[PostProcessScene(-1)]
	public static void OnPostprocessScene()
	{
		foreach (MonoBehaviour mb in Object.FindObjectsOfType<MonoBehaviour>())
		{
			IBakeable bakeable = mb as IBakeable;

			if (bakeable != null)
			{
				bakeable.Bake();
			}
		}
	}
	#endif
}
