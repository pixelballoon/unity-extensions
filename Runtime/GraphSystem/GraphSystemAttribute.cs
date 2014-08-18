using UnityEngine;
using System.Collections;
using System;

public class GraphSystemAttribute : Attribute
{
	public string System;

	public GraphSystemAttribute(string system)
	{
		System = system;
	}

}
