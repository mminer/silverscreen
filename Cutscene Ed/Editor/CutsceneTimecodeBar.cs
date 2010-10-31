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

class CutsceneTimecodeBar : ICutsceneGUI {
	readonly CutsceneEditor ed;

	const int shortTickHeight = 3;
	const int tallTickHeight  = 6;

	readonly Color tickColor          = Color.gray;
	readonly Color playheadBlockColor = Color.black;
	readonly Color inOutPointColour   = Color.cyan;

	readonly Texture positionIndicatorIcon = EditorGUIUtility.LoadRequired("Cutscene Ed/position_indicator.png") as Texture;
	readonly Texture inPointIndicatorIcon  = EditorGUIUtility.LoadRequired("Cutscene Ed/inpoint_indicator.png")  as Texture;
	readonly Texture outPointIndicatorIcon = EditorGUIUtility.LoadRequired("Cutscene Ed/outpoint_indicator.png") as Texture;

	readonly GUIContent inTooltip  = new GUIContent("", "In point.");
	readonly GUIContent outTooltip = new GUIContent("", "Out point.");

	public CutsceneTimecodeBar (CutsceneEditor ed) {
		this.ed = ed;
	}

	/// <summary>
	/// Displays the timecode bar with vertical lines indicating the time and the playhead.
	/// </summary>
	/// <param name="rect">The timecode bar's Rect.</param>
	public void OnGUI (Rect rect) {
		GUI.BeginGroup(rect);
		
		// Create a button that looks like a toolbar
		if (GUI.RepeatButton(new Rect(0, 0, rect.width, rect.height), GUIContent.none, EditorStyles.toolbar)) {
			float position = Event.current.mousePosition.x + ed.timelineScrollPos.x;
			ed.scene.playhead = position / ed.timelineZoom;

			// Show a visual notification of the position
			GUIContent notification = new GUIContent("Playhead " + ed.scene.playhead.ToString("N2"));
			ed.ShowNotification(notification);

			ed.Repaint();
			//SceneView.RepaintAll();
		} else {
			// TODO Investigate if attempting to remove the notification every frame like this is wasteful
			ed.RemoveNotification();
		}

		DrawTicks();
		DrawLabels();
		DrawPlayhead();
		DrawInOutPoints();

		GUI.EndGroup();
	}

	/// <summary>
	/// Draws vertical lines representing time increments.
	/// </summary>
	void DrawTicks () {
		Handles.color = tickColor;

		// Draw short ticks every second
		for (float i = 0; i < ed.scene.duration * ed.timelineZoom; i += ed.timelineZoom) {
			float xPos = i - ed.timelineScrollPos.x;
			Handles.DrawLine(new Vector3(xPos, 0, 0), new Vector3(xPos, shortTickHeight));
		}

		// Draw tall ticks every ten seconds
		for (float i = 0; i < ed.scene.duration * ed.timelineZoom; i += ed.timelineZoom * 10) {
			float xPos = i - ed.timelineScrollPos.x;
			Handles.DrawLine(new Vector3(xPos, 0, 0), new Vector3(xPos, tallTickHeight));
		}
	}

	/// <summary>
	/// Draws labels indicating the time.
	/// </summary>
	void DrawLabels () {
		for (float i = 0; i < 1000; i += 10) {
			float xPos = (i * ed.timelineZoom) - ed.timelineScrollPos.x;
			GUIContent label = new GUIContent(i + "");
			Vector2 dimensions = EditorStyles.miniLabel.CalcSize(label);
			Rect labelRect = new Rect(xPos - (dimensions.x / 2), 2, dimensions.x, dimensions.y);
			GUI.Label(labelRect, label, EditorStyles.miniLabel);
		}
	}

	/// <summary>
	/// Draws the playhead.
	/// </summary>
	void DrawPlayhead () {
		// Draw position indicator
		float pos = (ed.scene.playhead * ed.timelineZoom) - ed.timelineScrollPos.x;
		GUI.DrawTexture(new Rect(pos - 4, 0, 8, 8), positionIndicatorIcon);

		Handles.color = Color.black;

		// Vertical line
		Vector3 top = new Vector3(pos, 0);
		Vector3 bottom = new Vector3(pos, EditorStyles.toolbar.fixedHeight);
		Handles.DrawLine(top, bottom);

		// Zoom indicator block
		Handles.color = playheadBlockColor;
		
		for (int i = 1; i <= ed.timelineZoom; i++) {
			float xPos = pos + i;
			top = new Vector3(xPos, 4);
			bottom = new Vector3(xPos, EditorStyles.toolbar.fixedHeight - 1);
			Handles.DrawLine(top, bottom);
		}
	}

	/// <summary>
	/// Draws the in and out points.
	/// </summary>
	void DrawInOutPoints () {
		Handles.color = inOutPointColour;
		
		DrawInPoint();
		DrawOutPoint();
	}

	/// <summary>
	/// Draws the in point.
	/// </summary>
	void DrawInPoint () {
		float pos = (ed.scene.inPoint * ed.timelineZoom) - ed.timelineScrollPos.x;
		
		// Icon
		Rect indicatorRect = new Rect(pos, 0, 4, 8);
		GUI.DrawTexture(indicatorRect, inPointIndicatorIcon);

		// Tooltip
		GUI.Label(indicatorRect, inTooltip);

		// Vertical line
		Vector3 top = new Vector3(pos, 0);
		Vector3 bottom = new Vector3(pos, EditorStyles.toolbar.fixedHeight);
		Handles.DrawLine(top, bottom);
	}

	/// <summary>
	/// Draws the out point.
	/// </summary>
	void DrawOutPoint () {
		float pos = (ed.scene.outPoint * ed.timelineZoom) - ed.timelineScrollPos.x;
		
		// Icon
		Rect indicatorRect = new Rect(pos - 4, 0, 4, 8);
		GUI.DrawTexture(indicatorRect, outPointIndicatorIcon);
		
		// Tooltip
		GUI.Label(indicatorRect, outTooltip);
		
		// Vertical line
		Vector3 top = new Vector3(pos, 0);
		Vector3 bottom = new Vector3(pos, EditorStyles.toolbar.fixedHeight);
		Handles.DrawLine(top, bottom);
	}
}