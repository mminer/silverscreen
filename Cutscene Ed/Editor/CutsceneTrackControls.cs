using UnityEngine;
using UnityEditor;

class CutsceneTrackControls : ICutsceneGUI {
	readonly CutsceneEditor ed;

	readonly Texture newTrackIcon = EditorGUIUtility.LoadRequired("Cutscene Ed/icon_addtrack.png") as Texture;
	readonly GUIContent newTrackLabel;

	public CutsceneTrackControls (CutsceneEditor ed) {
		this.ed = ed;

		newTrackLabel = new GUIContent(newTrackIcon, "Add a new track.");
	}

	public void OnGUI (Rect rect) {
		// TODO Make this a style in the cutscene editor's GUISkin
		GUIStyle popupStyle = "MiniToolbarButton";
		popupStyle.padding.top		= 2;
		popupStyle.padding.right	= 0;
		popupStyle.padding.bottom	= 1;
		popupStyle.padding.left		= 0;

		GUI.BeginGroup(rect, GUI.skin.GetStyle("MiniToolbarButtonLeft"));

		// New track popdown
		Rect newTrackRect = new Rect(0, 0, 33, rect.height);
		GUI.Label(newTrackRect, newTrackLabel, popupStyle);

		if (Event.current.type == EventType.MouseDown && newTrackRect.Contains(Event.current.mousePosition)) {
			GUIUtility.hotControl = 0;
			EditorUtility.DisplayPopupMenu(newTrackRect, "Component/Cutscene/Track", null);
			Event.current.Use();
		}

		// Timeline zoom slider
		Rect timelineZoomRect = new Rect(newTrackRect.xMax + GUI.skin.horizontalSlider.margin.left, -1, rect.width - newTrackRect.xMax - GUI.skin.horizontalSlider.margin.horizontal, rect.height);
		ed.timelineZoom = GUI.HorizontalSlider(timelineZoomRect, ed.timelineZoom, ed.timelineMin, CutsceneTimeline.timelineZoomMax);

		GUI.EndGroup();
	}
}