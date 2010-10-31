/**
 * Copyright (c) 2010 Matthew Miner
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEditor;

class CutsceneTrackControls : ICutsceneGUI {
	readonly CutsceneEditor ed;
	
	readonly GUIContent newTrackLabel = new GUIContent(
		EditorGUIUtility.LoadRequired("Cutscene Ed/icon_addtrack.png") as Texture,
		"Add a new track."
	);

	public CutsceneTrackControls (CutsceneEditor ed) {
		this.ed = ed;
	}

	public void OnGUI (Rect rect) {
		// TODO Make this a style in the cutscene editor's GUISkin
		GUIStyle popupStyle = "MiniToolbarButton";
		popupStyle.padding = new RectOffset(0, 0, 2, 1); // Left, right, top, bottom

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