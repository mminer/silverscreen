using UnityEditor;
using UnityEngine;

class CutscenePlaybackControls : ICutsceneGUI {
	readonly CutsceneEditor ed;

	const int buttonWidth = 18;

	readonly GUIContent inPointLabel = new GUIContent(
		EditorGUIUtility.LoadRequired("Cutscene Ed/playback_in.png") as Texture,
		"Go to in point."
	);
	readonly GUIContent backLabel = new GUIContent(
		EditorGUIUtility.LoadRequired("Cutscene Ed/playback_back.png") as Texture,
		"Go back a second."
	);
	/*
	readonly GUIContent playLabel = new GUIContent(
		EditorGUIUtility.LoadRequired("Cutscene Ed/playback_play.png") as Texture,
		"Play."
	);
	*/
	readonly GUIContent forwardLabel = new GUIContent(
		EditorGUIUtility.LoadRequired("Cutscene Ed/playback_forward.png") as Texture,
		"Go forward a second."
	);
	readonly GUIContent outPointLabel = new GUIContent(
		EditorGUIUtility.LoadRequired("Cutscene Ed/playback_out.png") as Texture,
		"Go to out point."
	);

	public CutscenePlaybackControls (CutsceneEditor ed) {
		this.ed = ed;
	}
	
	/// <summary>
	/// Displays playback controls.
	/// </summary>
	/// <param name="rect">The playback controls' Rect.</param>
	public void OnGUI (Rect rect) {
		GUI.BeginGroup(rect, EditorStyles.toolbar);
			
		// In point
		Rect inPointRect = new Rect(6, 0, buttonWidth, rect.height);
		if (GUI.Button(inPointRect, inPointLabel, EditorStyles.toolbarButton)) {
			ed.scene.playhead = ed.scene.inPoint;
		}
		// Back
		Rect backRect = new Rect(inPointRect.xMax, 0, buttonWidth, rect.height);
		if (GUI.Button(backRect, backLabel, EditorStyles.toolbarButton)) {
			ed.scene.playhead -= CutsceneTimeline.scrubSmallJump;
		}
		/* Feature not implemented yet:
		// Play
		Rect playRect = new Rect(backRect.xMax, 0, buttonWidth, rect.height);
		if (GUI.Button(playRect, playLabel, EditorStyles.toolbarButton)) {
			EDebug.Log("Cutscene Editor: previewing scene (feature not implemented)");
		};*/
		// Forward
		Rect forwardRect = new Rect(backRect.xMax, 0, buttonWidth, rect.height);
		if (GUI.Button(forwardRect, forwardLabel, EditorStyles.toolbarButton)) {
			ed.scene.playhead += CutsceneTimeline.scrubSmallJump;
		}
		// Out point
		Rect outPointRect = new Rect(forwardRect.xMax, 0, buttonWidth, rect.height);
		if (GUI.Button(outPointRect, outPointLabel, EditorStyles.toolbarButton)) {
			ed.scene.playhead = ed.scene.outPoint;
		}

		Rect floatRect = new Rect(outPointRect.xMax + 4, 2, 50, rect.height);
		ed.scene.playhead = EditorGUI.FloatField(floatRect, ed.scene.playhead, EditorStyles.toolbarTextField);

		GUI.EndGroup();
	}
}