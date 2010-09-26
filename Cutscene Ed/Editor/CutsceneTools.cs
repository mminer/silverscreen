using UnityEditor;
using UnityEngine;

public enum Tool {
	MoveResize,
	Scissors,
	Zoom
}

class CutsceneTools : ICutsceneGUI {
	readonly CutsceneEditor ed;

	readonly GUIContent[] tools = {
		new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/tool_move.png")     as Texture, "Move/Resize"),
		new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/tool_scissors.png") as Texture, "Scissors"),
		new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/tool_zoom.png")     as Texture, "Zoom")
	};

	public CutsceneTools (CutsceneEditor ed) {
		this.ed = ed;
	}

	public void OnGUI (Rect rect) {
		GUILayout.BeginArea(rect);
		
		EditorGUILayout.BeginHorizontal(ed.style.GetStyle("Tools Bar"), GUILayout.Width(rect.width));

		GUILayout.FlexibleSpace();
		ed.currentTool = (Tool)GUILayout.Toolbar((int)ed.currentTool, tools);

		EditorGUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}