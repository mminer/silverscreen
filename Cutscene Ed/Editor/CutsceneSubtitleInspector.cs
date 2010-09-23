using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom inspector for a subtitle.
/// </summary>
[CustomEditor (typeof(CutsceneSubtitle))]
public class CutsceneSubtitleInspector : Editor {
	CutsceneSubtitle subtitle {
		get { return target as CutsceneSubtitle; }
	}
	
	public override void OnInspectorGUI () {
		EditorGUILayout.BeginHorizontal();

			EditorGUILayout.PrefixLabel("Dialog");
			subtitle.dialog = EditorGUILayout.TextArea(subtitle.dialog);

		EditorGUILayout.EndHorizontal();
		
		if (GUI.changed) {
			EditorUtility.SetDirty(subtitle);
		}
	}
}
