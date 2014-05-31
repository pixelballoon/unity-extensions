using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

public class CreateCubes
{

#if UNITY_EDITOR
	[PostProcessScene]
	public static void OnPostprocessScene()
	{
		for (int i = 0; i < 10; i++)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(-250 + i * 10f, 80, 0);
		}
	}
#endif

}
