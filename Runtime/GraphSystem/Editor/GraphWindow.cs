using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class GraphWindow : EditorWindow
{

	private Rect _areaRect;
	private Vector2 _mousePosition;

	private string _systemName;
	private Component _system;
	private GraphData _graphData;
	
	private MonoBehaviour _dragItem;
	private Vector2 _dragConnectionPosition;
	private Component _dragConnectionComponent;
	private FieldInfo _dragConnectionField;

	private List<KeyValuePair<Vector2, MonoBehaviour>> _connections;

	private Vector2 _scrollPosition;
	
	[MenuItem("Window/Celsian/Graph Editor")]
	static protected void Init()
	{
		GetWindow(typeof(GraphWindow));
	}

	public GraphWindow()
	{
		title = "Graph Editor";
		_connections = new List<KeyValuePair<Vector2, MonoBehaviour>>();
	}

	protected void OnGUI()
	{
		if (_system == null)
		{
			return;
		}

		// Not sure why the magic numbers on size are needed to make the scrollbars appear in the correct place....
		_scrollPosition = GUI.BeginScrollView(new Rect(0,0,Screen.width-4,Screen.height-21), _scrollPosition, new Rect(0,0,5000,5000), true, true);

		Event evt = Event.current;

		if (evt.type == EventType.MouseDrag)
		{
			Repaint();
		}

		_connections.Clear();

		foreach (MonoBehaviour component in _system.gameObject.GetComponents<MonoBehaviour>())
		{
			Type componentType = component.GetType();

			GraphNodeAttribute[] nodeAttributes = componentType.GetCustomAttributes(typeof(GraphNodeAttribute), false) as GraphNodeAttribute[];
			if (nodeAttributes != null)
			{
				bool hasSystem = false;
				foreach (GraphNodeAttribute attribute in nodeAttributes)
				{
					if (attribute.System == _systemName)
					{
						hasSystem = true;
						break;
					}
				}

				if (!hasSystem)
				{
					continue;
				}

				GraphNodeData nodeData = _graphData.GetNodeData(component);

				_areaRect = new Rect(nodeData.Position.x, nodeData.Position.y, nodeData.Width, 1000);
				GUILayout.BeginArea(_areaRect);

				GUILayout.Box(ObjectNames.NicifyVariableName(componentType.Name), GUILayout.ExpandWidth(true));

				if (evt.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Delete"), false, (object data) => { DestroyImmediate(data as MonoBehaviour, false); },
						component);
					menu.ShowAsContext();
					evt.Use();
				}

				if (evt.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
				    _dragItem == null)
				{
					_dragItem = component;
				}

				if (evt.type == EventType.MouseDrag && _dragItem == component)
				{
					nodeData.Position += Event.current.delta;
					Event.current.Use();
				}

				GUILayout.Box("", GUILayout.ExpandWidth(true));

				if (evt.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
				    _dragConnectionComponent != null)
				{
					if (EditorHelpers.IsSubclassOfRawGeneric(_dragConnectionField.FieldType, component.GetType()))
					{
						_dragConnectionField.SetValue(_dragConnectionComponent, component);
					}
				}

				string description = component.ToString();
				// ) is the last character on the default ToString... there's probably a better way of doing this, without requiring a custom method on every node
				if (!string.IsNullOrEmpty(description) && description[description.Length - 1] != ')')
				{
					GUI.skin.label.wordWrap = true;
					GUILayout.Label(description, GUILayout.ExpandWidth(true));
				}

				List<FieldInfo> componentFields = new List<FieldInfo>();
				EditorHelpers.FindFields(componentFields, componentType);

				foreach (FieldInfo field in componentFields)
				{
					GraphConnectionAttribute[] connectionAttributes = field.GetCustomAttributes(typeof(GraphConnectionAttribute), false) as GraphConnectionAttribute[];
					if (connectionAttributes != null && connectionAttributes.Length > 0)
					{
						GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

						if (connectionAttributes[0].Direction == GraphConnectionAttribute.DirectionType.In)
						{
							DoConnectionBox(component, field);
						}
						else
						{
							GUILayout.Space(24);
						}

						GUILayout.FlexibleSpace();
						GUILayout.Label(ObjectNames.NicifyVariableName(field.Name), GUILayout.ExpandWidth(true));
						GUILayout.FlexibleSpace();

						if (connectionAttributes[0].Direction == GraphConnectionAttribute.DirectionType.Out)
						{
							DoConnectionBox(component, field);
						}
						else
						{
							GUILayout.Space(24);
						}

						GUILayout.EndHorizontal();
					}
				}

				nodeData.ShowInspector = EditorGUILayout.Foldout(nodeData.ShowInspector, "Inspector");
				if (nodeData.ShowInspector)
				{
					var editor = Editor.CreateEditor(component);
					editor.OnInspectorGUI();
				}
				
				GUILayout.EndArea();
			}
		}

		foreach (var connection in _connections)
		{
			GraphNodeData graphData = _graphData.GetNodeData(connection.Value);

			if (graphData != null)
			{
				if (connection.Key.x > graphData.Position.x)
				{
					DrawLine(connection.Key, graphData.Position + new Vector2(graphData.Width, 32));
				}
				else
				{
					DrawLine(connection.Key, graphData.Position + new Vector2(0, 32));
				}
				
			}
		}

		if (_dragConnectionComponent != null)
		{
			DrawLine(_dragConnectionPosition, Event.current.mousePosition);
		}

		if (evt.type == EventType.MouseUp)
		{
			_dragItem = null;
			_dragConnectionComponent = null;
			_dragConnectionField = null;
			Repaint();
		}

		if (evt.type == EventType.ContextClick)
		{
			_mousePosition = evt.mousePosition;

			GenericMenu menu = new GenericMenu();

			menu.AddSeparator("Add New Item");

			Assembly assembly = typeof(Realtime).Assembly;
			foreach (Type type in assembly.GetTypes())
			{
				object[] attributes = type.GetCustomAttributes(false);

				foreach (Attribute attribute in attributes)
				{
					GraphNodeAttribute graphNode = attribute as GraphNodeAttribute;
					if (graphNode != null && graphNode.System == _systemName)
					{
						menu.AddItem(new GUIContent(graphNode.Type + "/" + graphNode.Name), false, OnMenuItemClicked, type);
						break;
					}
				}
			}

			menu.ShowAsContext();
			evt.Use();
		}

		GUI.EndScrollView();

		_graphData.Clean();

		EditorUtility.SetDirty(_system.gameObject);
	}

	protected void DrawLine(Vector2 startPoint, Vector2 endPoint)
	{
		const float gravity = 40;

		float tangentOffset = (endPoint.x - startPoint.x) / 2;
		Vector2 startTangent = new Vector2(startPoint.x + tangentOffset, startPoint.y + gravity);
		Vector2 endTangent = new Vector2(endPoint.x - tangentOffset, endPoint.y + gravity);

		Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, Color.green, null, 3f);
	}

	protected void OnMenuItemClicked(object type)
	{
		Component component = _system.gameObject.AddComponent(type as Type);
		_graphData.GetNodeData(component, _mousePosition);
	}

	protected void OnSelectionChange()
	{
		UpdateSystem();
		Repaint();
	}

	protected void DoConnectionBox(Component component, FieldInfo field)
	{
		Event evt = Event.current;

		GUILayout.Box("", GUILayout.Width(20));

		Vector2 connectionPosition = GUILayoutUtility.GetLastRect().center + new Vector2(_areaRect.x, _areaRect.y);

		if (evt.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
		{
			_dragConnectionPosition = connectionPosition;
			_dragConnectionComponent = component;
			_dragConnectionField = field;
		}

		if (evt.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
		{
			field.SetValue(component, null);
			evt.Use();
		}

		MonoBehaviour connectedValue = field.GetValue(component) as MonoBehaviour;
		if (connectedValue != null)
		{
			_connections.Add(new KeyValuePair<Vector2, MonoBehaviour>(connectionPosition, connectedValue));
		}
	}

	protected void OnFocus()
	{
		UpdateSystem();
	}

	protected void UpdateSystem()
	{
		_system = null;

		GameObject active = Selection.activeGameObject;

		if (!active)
			return;

		foreach (MonoBehaviour component in active.GetComponents<MonoBehaviour>())
		{
			GraphSystemAttribute[] attributes = component.GetType().GetCustomAttributes(typeof(GraphSystemAttribute), false) as GraphSystemAttribute[];

			if (attributes != null && attributes.Length > 0)
			{
				_system = component;
				_systemName = attributes[0].System;
				_scrollPosition = new Vector2(2500,2500);
				_graphData = component.GetType().GetField("GraphData").GetValue(component) as GraphData;
				if (_graphData == null)
				{
					_graphData = new GraphData();
					component.GetType().GetField("GraphData").SetValue(component, _graphData);
				}
			}
		}
	}
}
