using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;

public class EditorHelpers
{

	public static void DrawAngleRange(Transform transform, float rotation, float minAngle, float maxAngle)
	{
		rotation = Mathf.Clamp(rotation, minAngle, maxAngle);
		
		for (float i = -maxAngle; i < -minAngle - 1f; i += 1f)
		{
			Vector3 first = transform.position + (Quaternion.Euler(0, 0, i) * transform.up) * 10f;
			Vector3 second = transform.position + (Quaternion.Euler(0, 0, i + 1f) * transform.up) * 10f;
			
			Gizmos.DrawLine(first, second);
		}
		
		Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, 0, -minAngle) * transform.up * 10f));
		Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, 0, -maxAngle) * transform.up * 10f));
		
		Gizmos.color = Color.green;
		
		Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, 0, -rotation) * transform.up * 10f));
	}

	public static bool IsSubclassOfRawGeneric(Type generic, Type type)
	{
		while (type != null && type != typeof(object))
		{
			var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			if (generic == current)
			{
				return true;
			}
			type = type.BaseType;
		}
		return false;
	}

	public static void FindFields(ICollection<FieldInfo> fields, Type t)
	{
		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		foreach (var field in t.GetFields(flags))
		{
			// Ignore inherited fields
			if (field.DeclaringType == t)
				fields.Add(field);
		}

		var baseType = t.BaseType;
		if (baseType != null)
			FindFields(fields, baseType);
	}

}
