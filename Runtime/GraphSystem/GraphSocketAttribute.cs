using UnityEngine;
using System.Collections;
using System;

public class GraphSocketAttribute : Attribute
{

	public enum SocketType
	{
		In,
		Out
	}

	public string System;
	public SocketType Type;
	public string Name;

	public GraphSocketAttribute(string system, SocketType type, string name)
	{
		System = system;
		Type = type;
		Name = name;
	}

}
