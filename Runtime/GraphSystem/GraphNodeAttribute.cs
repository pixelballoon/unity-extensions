using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GraphNodeAttribute : Attribute
{

	public string System;
	public string Type;
	public string Name;

	public GraphNodeAttribute(string system, string type, string name)
	{
		System = system;
		Type = type;
		Name = name;
	}

}
