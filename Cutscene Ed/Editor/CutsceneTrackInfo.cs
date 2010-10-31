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

class CutsceneTrackInfo : ICutsceneGUI {
	readonly CutsceneEditor ed;

	readonly GUIContent visibilityIcon = new GUIContent("",
		EditorGUIUtility.LoadRequired("Cutscene Ed/icon_eye.png") as Texture,
		"Toggle visibility."
	);
	
	readonly GUIContent lockIcon = new GUIContent("",
		EditorGUIUtility.LoadRequired("Cutscene Ed/icon_lock.png") as Texture,
		"Toggle track lock."
	);

	public CutsceneTrackInfo (CutsceneEditor ed) {
		this.ed = ed;
	}

	/// <summary>
	/// Displays the track info GUI.
	/// </summary>
	/// <param name="rect">The track info's Rect.</param>
	public void OnGUI (Rect rect) {
		ed.timelineScrollPos.y = EditorGUILayout.BeginScrollView(
			new Vector2(0, ed.timelineScrollPos.y),
			false, false,
			ed.style.horizontalScrollbar,
			ed.style.verticalScrollbar,
			GUIStyle.none,
			GUILayout.Width(CutsceneTimeline.trackInfoWidth), GUILayout.ExpandHeight(true)).y;

		foreach (CutsceneTrack track in ed.scene.tracks) {
			if (track == ed.selectedTrack) {
				GUI.SetNextControlName("track");
			}

			Rect infoRect = EditorGUILayout.BeginHorizontal(ed.style.GetStyle("Track Info"));
			track.enabled = GUILayout.Toggle(track.enabled, visibilityIcon, EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false));
			track.locked  = GUILayout.Toggle(track.locked, lockIcon, EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false));

			GUILayout.Space(10);

			GUI.enabled = track.enabled;

			// Track name and icon
			GUIContent trackIcon = new GUIContent(ed.mediaIcons[(int)track.type]);
			GUILayout.Label(trackIcon, GUILayout.ExpandWidth(false));
			// Set a minimum width to keep the label from becoming uneditable
			Rect trackNameRect = GUILayoutUtility.GetRect(new GUIContent(track.name), EditorStyles.miniLabel, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
			track.name = EditorGUI.TextField(trackNameRect, track.name, EditorStyles.miniLabel);

			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();

			if (track == ed.selectedTrack) {
				GUI.FocusControl("track");
			}

			// Handle clicks
			Vector2 mousePos = Event.current.mousePosition;
			if (Event.current.type == EventType.MouseDown && infoRect.Contains(mousePos)) {
				switch (Event.current.button) {
					case 0: // Left mouse button
						ed.selectedTrack = track;
						break;
					
					case 1: // Right mouse button
						EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "CONTEXT/CutsceneTrack/", new MenuCommand(track));
						break;
					
					default:
						break;
				}

				Event.current.Use();
			}
		}

		// Divider line
		Handles.color = Color.grey;
		Handles.DrawLine(new Vector3(67, 0), new Vector3(67, rect.yMax));

		EditorGUILayout.EndScrollView();
	}
}