using UnityEditor;
using UnityEngine;

public enum DragEvent {
	Move,
	ResizeLeft,
	ResizeRight
}

class CutsceneTimeline : ICutsceneGUI {
	readonly CutsceneEditor ed;

	public const int scrubLargeJump = 10;
	public const int scrubSmallJump = 1;

	readonly ICutsceneGUI navigation;
	readonly ICutsceneGUI tracksView;
	readonly ICutsceneGUI trackControls;
	readonly ICutsceneGUI trackInfo;

	public const float timelineZoomMin	= 1f;
	public const float timelineZoomMax	= 100f;
	public const float trackInfoWidth	= 160f;
	
	public CutsceneTimeline (CutsceneEditor ed) {
		this.ed = ed;

		navigation		= new CutsceneNavigation(ed);
		tracksView		= new CutsceneTracksView(ed);
		trackControls	= new CutsceneTrackControls(ed);
		trackInfo		= new CutsceneTrackInfo(ed);
	}

	/// <summary>
	/// Displays the timeline's GUI.
	/// </summary>
	/// <param name="rect">The timeline's Rect.</param>
	public void OnGUI (Rect rect) {
		GUILayout.BeginArea(rect);

		float rightColWidth = GUI.skin.verticalScrollbar.fixedWidth;
		float middleColWidth = rect.width - CutsceneTimeline.trackInfoWidth - rightColWidth;

		ed.timelineMin = CutsceneTimeline.timelineZoomMin * (middleColWidth / ed.scene.duration);
		ed.timelineZoom = Mathf.Clamp(ed.timelineZoom, ed.timelineMin, CutsceneTimeline.timelineZoomMax);

		// Navigation bar
		Rect navigationRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toolbar, GUILayout.Width(rect.width));
		navigation.OnGUI(navigationRect);

		// Begin Tracks
		EditorGUILayout.BeginHorizontal(GUILayout.Width(rect.width), GUILayout.ExpandHeight(true));

			// Track info
			//Rect trackInfoRect = GUILayoutUtility.GetRect(trackInfoWidth, rect.height - navigationRect.yMax - GUI.skin.horizontalScrollbar.fixedHeight);
			Rect trackInfoRect = new Rect(0, 0, CutsceneTimeline.trackInfoWidth, 9999);
			trackInfo.OnGUI(trackInfoRect);

			// Track controls
			float trackControlsHeight = GUI.skin.horizontalScrollbar.fixedHeight;
			Rect trackControlsRect = new Rect(0, rect.height - trackControlsHeight, CutsceneTimeline.trackInfoWidth, trackControlsHeight);
			trackControls.OnGUI(trackControlsRect);

			// Tracks
			Rect tracksRect = new Rect(trackInfoRect.xMax, navigationRect.yMax, middleColWidth + rightColWidth, rect.height - navigationRect.yMax);
			tracksView.OnGUI(tracksRect);
		
		EditorGUILayout.EndHorizontal();

		if (Event.current.type == EventType.KeyDown) { // Key presses
			ed.HandleKeyboardShortcuts(Event.current);
		}

		GUILayout.EndArea();
	}

	/// <summary>
	/// Moves the playhead.
	/// </summary>
	/// <param name="playheadPos">The position to move the playhead to.</param>
	void MovePlayheadToPosition (float playheadPos) {
		ed.scene.playhead = playheadPos / ed.timelineZoom;
	}

	/// <summary>
	/// Splits a clip into two separate ones at the specified time.
	/// </summary>
	/// <param name="splitPoint">The time at which to split the clip.</param>
	/// <param name="track">The track the clip is sitting on.</param>
	/// <param name="clip">The clip to split.</param>
	/// <returns>The new clip.</returns>
	public static CutsceneClip SplitClipAtTime (float splitPoint, CutsceneTrack track, CutsceneClip clip) {
		CutsceneClip newClip = clip.GetCopy();

		// Make sure the clip actually spans over the split point
		if (splitPoint < clip.timelineStart || splitPoint > clip.timelineStart + clip.duration) {
			EDebug.Log("Cutscene Editor: cannot split clip; clip does not contain the split point");
			return null;
		}

		clip.SetOutPoint(clip.inPoint + (splitPoint - clip.timelineStart));
		newClip.SetInPoint(clip.outPoint);
		newClip.SetTimelineStart(splitPoint);

		track.clips.Add(newClip);

		Event.current.Use();
		EDebug.Log("Cutscene Editor: splitting clip at time " + splitPoint);

		return newClip;
	}
}