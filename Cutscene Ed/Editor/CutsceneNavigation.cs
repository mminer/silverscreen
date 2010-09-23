using UnityEditor;
using UnityEngine;

class CutsceneNavigation : ICutsceneGUI {
	readonly CutsceneEditor ed;

	readonly ICutsceneGUI playbackControls;
	readonly ICutsceneGUI timecodeBar;

	readonly GUIContent zoomButton = new GUIContent("", "Zoom timeline to entire scene.");

	public CutsceneNavigation (CutsceneEditor ed) {
		this.ed = ed;
		playbackControls	= new CutscenePlaybackControls(ed);
		timecodeBar			= new CutsceneTimecodeBar(ed);
	}

	public void OnGUI (Rect rect) {
		GUI.BeginGroup(rect);

		float zoomButtonWidth = GUI.skin.verticalScrollbar.fixedWidth;
		float timecodeBarWidth = ed.position.width - CutsceneTimeline.trackInfoWidth - zoomButtonWidth;

		// Playback controls
		Rect playbackControlsRect = new Rect(0, 0, CutsceneTimeline.trackInfoWidth, rect.height);
		playbackControls.OnGUI(playbackControlsRect);

		// Timecode bar
		Rect timecodeBarRect = new Rect(playbackControlsRect.xMax, 0, timecodeBarWidth, rect.height);
		timecodeBar.OnGUI(timecodeBarRect);

		// Zoom to view entire project
		Rect zoomButtonRect = new Rect(timecodeBarRect.xMax, 0, zoomButtonWidth, rect.height);
		if (GUI.Button(zoomButtonRect, zoomButton, EditorStyles.toolbarButton)) {
			ed.timelineZoom = ed.timelineMin;
			EDebug.Log("Cutscene Editor: zoomed timeline to entire scene");
		}

		GUI.EndGroup();
	}
}