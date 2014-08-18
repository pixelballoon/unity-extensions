using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Shortcuts
{

	// Taken from http://framebunker.com/blog/a-few-useful-shortcuts/
	[MenuItem("Assets/Find In Project %g", false, 0)]
	static void ProjectSearch()
	{
		// Get the internal UnityEditor.ObjectBrowser window
		System.Type t = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");

		// Get the window & focus it
		EditorWindow win = EditorWindow.GetWindow(t);
		win.Focus();

		// Send a find command
		Event e = new Event();
		e.type = EventType.ExecuteCommand;
		e.commandName = "Find";
		win.SendEvent(e);
	}

}
