using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphData
{

	[Serializable]
	public class NodeDataDictionary
	{
		[SerializeField]
		List<Component> _keys;

		[SerializeField]
		List<GraphNodeData> _values;
		
		public NodeDataDictionary()
		{
			_keys = new List<Component>();
			_values = new List<GraphNodeData>();
		}

		public void Clean()
		{
			for (int i = 0; i < _keys.Count;)
			{
				if (_keys[i] == null)
				{
					_keys.RemoveAt(i);
					_values.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		public GraphNodeData GetValue(Component key, Vector2 position)
		{
			return _values[GetIndex(key, position)];
		}

		int GetIndex(Component key, Vector2 position)
		{
			if (!_keys.Contains(key))
			{
				_keys.Add(key);
				_values.Add(new GraphNodeData(position));
			}

			return _keys.IndexOf(key);
		}
	}

	[SerializeField]
	private NodeDataDictionary _nodeData;

	public GraphData()
	{
		_nodeData = new NodeDataDictionary();
	}

	public void Clean()
	{
		_nodeData.Clean();
	}

	public GraphNodeData GetNodeData(Component component)
	{
		return GetNodeData(component, new Vector2(2500, 2500));
	}

	public GraphNodeData GetNodeData(Component component, Vector2 position)
	{
		return _nodeData.GetValue(component, position);
	}
}

[Serializable]
public class GraphNodeData
{

	public Vector2 Position;
	public bool ShowInspector;

	public float Width
	{
		get
		{
			return ShowInspector ? 400 : 200;
		}
	}

	public GraphNodeData(Vector2 position)
	{
		Position = position;
	}

}
