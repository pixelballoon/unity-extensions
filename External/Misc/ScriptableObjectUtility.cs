#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

// Obtained from https://github.com/eriksvedang/UnitySnippets/blob/master/ScriptableObjectUtility.cs
// Slightly modified to clean up spacing and variable names
public static class ScriptableObjectUtility
{
	
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T scriptableObject = ScriptableObject.CreateInstance<T>();
		
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
		
		AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = scriptableObject;
	}

}

#endif
