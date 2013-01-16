using UnityEditor;
using UnityEngine;

public delegate void GUIContents ();

public static class CutsceneEditorGUILayout
{
	public static Rect Horizontal (GUIContents contents, params GUILayoutOption[] options)
	{
		Rect rect = EditorGUILayout.BeginHorizontal(options);
			contents();
		EditorGUILayout.EndHorizontal();
		
		return rect;
	}
	
	public static Rect Horizontal (GUIContents contents, GUIStyle style, params GUILayoutOption[] options)
	{
		Rect rect = EditorGUILayout.BeginHorizontal(style, options);
			contents();
		EditorGUILayout.EndHorizontal();
		
		return rect;
	}
}

public static class CutsceneGUILayout
{
	public static void Area (GUIContents contents, Rect screenRect)
	{
		GUILayout.BeginArea(screenRect);
			contents();
		GUILayout.EndArea();
	}
}