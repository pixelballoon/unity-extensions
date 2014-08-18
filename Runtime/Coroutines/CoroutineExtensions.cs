using System;
using UnityEngine;
using System.Collections;

public static class CoroutineExtensions
{
	
	public static CoroutineExtended StartCoroutineEx(this MonoBehaviour monoBehaviour, IEnumerator coroutine)
	{
		CoroutineExtended coroutineEx = new CoroutineExtended();
		coroutineEx.Coroutine = monoBehaviour.StartCoroutine(coroutineEx.InternalRoutine(coroutine));
		return coroutineEx;
	}

}

public interface ICoroutineYieldable
{

	bool IsFinished { get; }

}

public class CoroutineExtended
{

	public Coroutine Coroutine;

	public IEnumerator InternalRoutine(IEnumerator coroutine)
	{
		while (coroutine != null && coroutine.MoveNext())
		{
			object yielded = coroutine.Current;

			ICoroutineYieldable yieldable = yielded as ICoroutineYieldable;
			if (yieldable != null)
			{
				while (!yieldable.IsFinished)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				yield return yielded;
			}
		}
	}

}
