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

using UnityEditor;
using UnityEngine;

class CutsceneNavigation : ICutsceneGUI
{
	readonly CutsceneEditor ed;

	readonly ICutsceneGUI playbackControls;
	readonly ICutsceneGUI timecodeBar;

	readonly GUIContent zoomButton = new GUIContent("", "Zoom timeline to entire scene.");

	public CutsceneNavigation (CutsceneEditor ed)
	{
		this.ed = ed;
		playbackControls = new CutscenePlaybackControls(ed);
		timecodeBar      = new CutsceneTimecodeBar(ed);
	}

	public void OnGUI (Rect rect)
	{
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