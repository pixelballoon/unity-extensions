using UnityEngine;
using System.Collections;

public class FindNearestPostProcess : MonoBehaviour, IBakeable
{
	
	public GameObject Nearest;

#if UNITY_EDITOR
	public void Bake()
	{
		GameObject[] objects = FindObjectsOfType<GameObject>();

		float closestDistance = float.MaxValue;

		foreach (GameObject o in objects)
		{
			float distance = Vector3.Distance(o.transform.position, transform.position);
			if (distance < closestDistance && o != gameObject)
			{
				closestDistance = distance;
				Nearest = o;
			}
		}
	}
#endif

}
