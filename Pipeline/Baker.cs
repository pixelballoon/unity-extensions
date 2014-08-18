using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

public class Baker : MonoBehaviour, IBakeable
{
	
	// Processed objects are stored in this hash set to make sure they're only processed once.
	// It's not necessary when running a final build, but is necessary when hitting play through
	// the edit. This is because if two scenes are loaded additively then this callback gets
	// called twice, and because it finds the objects using Object.FindObjectsOfType it gets all
	// objects in both scenes the second time round.

	static HashSet<IBakeable> _processed;


#if UNITY_EDITOR
	public void Awake()
	{
		ProcessBake();
	}

	[PostProcessScene(-1)]
	public static void OnPostprocessScene()
	{
		ProcessBake();

		if (Application.isEditor && Application.isPlaying)
		{
			Baker baker = FindObjectOfType<Baker>();
			if (baker == null)
			{
				Debug.LogWarning("There is no instance of Baker within this scene, Awake on objects will be run before baking is complete. Make sure to set the script priority of Baker to highest.");
			}
		}
	}

	private static void ProcessBake()
	{
		if (_processed == null)
		{
			_processed = new HashSet<IBakeable>();
		}

		MonoBehaviour[] monoBehaviours = Object.FindObjectsOfType<MonoBehaviour>();
		foreach (MonoBehaviour mb in monoBehaviours)
		{
			if (mb == null)
				continue;

			if (PrefabUtility.GetPrefabType(mb) == PrefabType.Prefab)
				continue;
	
			IBakeable bakeable = mb as IBakeable;

			if (bakeable != null && !_processed.Contains(bakeable))
			{
				_processed.Add(bakeable);

				bakeable.Bake();
			}
		}
	}

	public void Bake()
	{
#if !UNITY_EDITOR
		DestroyImmediate(this);
#endif
	}
#endif

}
