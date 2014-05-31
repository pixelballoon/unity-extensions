using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ContextMenuConstructor
{
	
#if UNITY_EDITOR
	private class ItemDetails
	{
		public string Function;
		public List<object> Targets;
		
		public ItemDetails()
		{
			Targets = new List<object>();
		}
	}
	
	private Dictionary<string, ItemDetails> _items;

	public ContextMenuConstructor()
	{
		_items = new Dictionary<string, ItemDetails>();
	}
	
	public void AddItem(string name, string function, object target)
	{
		ItemDetails item;
		if (!_items.TryGetValue(name, out item))
		{
			item = new ItemDetails();
		}
		
		item.Function = function;
		item.Targets.Add(target);

		_items[name] = item;
		
	}
	
	public bool ConstructMenu(GenericMenu menu)
	{
		foreach (var item in _items)
		{
			menu.AddItem(new UnityEngine.GUIContent(item.Key), false, OnContextItem, item.Value);
		}

		return _items.Count > 0;
	}

	private void OnContextItem(object userData)
	{
		ItemDetails item = (userData as ItemDetails);

		foreach (object target in item.Targets)
		{
			target.GetType().GetMethod(item.Function, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, null);
		}
	}
#endif
	
}
