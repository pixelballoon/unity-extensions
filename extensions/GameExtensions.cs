using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameExtensions
{

	public static void Resize<T>(this List<T> list, int newSize) where T : new()
	{
		int currentSize = list.Count;
		if (newSize < currentSize)
		{
			list.RemoveRange(newSize, currentSize - newSize);
		}
		else if (newSize > currentSize)
		{
			if (newSize > list.Capacity)
				list.Capacity = newSize;

			for (int i = 0; i < (newSize - currentSize); i++)
			{
				list.Add(new T());
			}
		}
	}

	public static Quaternion GetRelativeOrbQuaternion(this MonoBehaviour monoBehaviour)
	{
		return Quaternion.Euler(0, 0, GetRelativeOrbRotation(monoBehaviour));
	}

	public static float GetRelativeOrbRotation(this MonoBehaviour monoBehaviour)
	{
		return (Mathf.Atan2(monoBehaviour.transform.localPosition.y, monoBehaviour.transform.localPosition.x) * Mathf.Rad2Deg + 270f) % 360f;
	}

	public static float AngleTo(this Transform main, Transform other)
	{
		return AngleTo(main, other.position);
	}

	public static float AngleTo(this Transform main, Vector3 other)
	{
		return (Mathf.Atan2(main.position.y - other.y, main.position.x - other.x) * Mathf.Rad2Deg + 360f + 90f) % 360f;
	}
	
	public static Color SetFromHSV(this Color color, float h, float s, float v, float a = 1)
	{
		// no saturation, we can return the value across the board (grayscale)
		if (Math.Abs(s) < 0.01f)
		{
			color.r = color.g = color.b = v;
			color.a = a;
			return color;
		}

		// which chunk of the rainbow are we in?
		float sector = h / 60;

		// split across the decimal (ie 3.87 into 3 and 0.87)
		int i = (int)sector;
		float f = sector - i;

		float p = v * (1 - s);
		float q = v * (1 - s * f);
		float t = v * (1 - s * (1 - f));

		// build our rgb color
		switch (i)
		{
			case 0:
				color.r = v;
				color.g = t;
				color.b = p;
				break;

			case 1:
				color.r = q;
				color.g = v;
				color.b = p;
				break;

			case 2:
				color.r = p;
				color.g = v;
				color.b = t;
				break;

			case 3:
				color.r = p;
				color.g = q;
				color.b = v;
				break;

			case 4:
				color.r = t;
				color.g = p;
				color.b = v;
				break;

			default:
				color.r = v;
				color.g = p;
				color.b = q;
				break;
		}

		return color;
	}

	public static void ToHsv(this Color color, out float h, out float s, out float v)
	{
		float min = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
		float max = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
		float delta = max - min;

		// value is our max color
		v = max;

		// saturation is percent of max
		if (!Mathf.Approximately(max, 0))
			s = delta / max;
		else
		{
			// all colors are zero, no saturation and hue is undefined
			s = 0;
			h = -1;
			return;
		}

		// grayscale image if min and max are the same
		if (Mathf.Approximately(min, max))
		{
			v = max;
			s = 0;
			h = -1;
			return;
		}

		// hue depends which color is max (this creates a rainbow effect)
		if (Math.Abs(color.r - max) < 0.01f)
			h = (color.g - color.b) / delta;            // between yellow & magenta
		else if (Math.Abs(color.g - max) < 0.01f)
			h = 2 + (color.b - color.r) / delta;                // between cyan & yellow
		else
			h = 4 + (color.r - color.g) / delta;                // between magenta & cyan

		// turn hue into 0-360 degrees
		h *= 60;
		if (h < 0)
			h += 360;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		System.Random rng = new System.Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

}
