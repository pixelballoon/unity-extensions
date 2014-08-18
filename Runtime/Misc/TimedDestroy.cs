using UnityEngine;
using System.Collections;
using PathologicalGames;

public class TimedDestroy : MonoBehaviour
{

#pragma warning disable 649
	[SerializeField]
	private float _time;
#pragma warning restore 649

	public void Start()
	{
		DestroyIn(_time);
	}

	public void OnSpawned(SpawnPool pool)
	{
		DestroyIn(_time);
	}

	private void DestroyIn(float time)
	{
		StopAllCoroutines();
		StartCoroutine(Destroy(time));
	}

	public IEnumerator Destroy(float time)
	{
		yield return new WaitForSeconds(time);
		GetComponent<Poolable>().Destroy();
	}

}
