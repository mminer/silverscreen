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

class CutsceneTracksView : ICutsceneGUI {
	readonly CutsceneEditor ed;

	readonly Texture overlay = EditorGUIUtility.LoadRequired("Cutscene Ed/overlay.png")	as Texture;
	readonly Color inOutPointColour = Color.cyan;

	public CutsceneTracksView (CutsceneEditor ed) {
		this.ed = ed;
	}
	
	/// <summary>
	/// Displays the tracks GUI.
	/// </summary>
	/// <param name="rect">The tracks' Rect.</param>
	public void OnGUI (Rect rect) {
		// Display background
		Rect background = new Rect(rect.x, rect.y,
			rect.width - GUI.skin.verticalScrollbar.fixedWidth,
			rect.height - GUI.skin.horizontalScrollbar.fixedHeight);
		GUI.BeginGroup(background, GUI.skin.GetStyle("AnimationCurveEditorBackground"));
		GUI.EndGroup();

		float trackHeight  = ed.style.GetStyle("Track").fixedHeight + ed.style.GetStyle("Track").margin.vertical;
		// TODO Make the track width take into account clips that are beyond the out point
		float tracksWidth  = (ed.scene.outPoint + 10) * ed.timelineZoom;
		float tracksHeight = trackHeight * ed.scene.tracks.Length;

		Rect view = new Rect(0, 0,
			Mathf.Max(background.width + 1, tracksWidth),
			Mathf.Max(background.height + 1, tracksHeight));

		ed.timelineScrollPos = GUI.BeginScrollView(rect, ed.timelineScrollPos, view, true, true);
			
			// Zoom clicks
			if (ed.currentTool == Tool.Zoom && Event.current.type == EventType.MouseDown) {
				if (Event.current.alt && Event.current.shift) { // Zoom out all the way
					ed.timelineZoom = CutsceneTimeline.timelineZoomMin;
				} else if (Event.current.shift) {               // Zoom in all the way
					ed.timelineZoom = CutsceneTimeline.timelineZoomMax;
				} else if (Event.current.alt) {                 // Zoom out
					ed.timelineZoom -= 10;
				} else {                                        // Zoom in
					ed.timelineZoom += 10;
				}

				Event.current.Use();
			}
			
			// Draw track divider lines
			Handles.color = Color.grey;
			for (int i = 0; i < ed.scene.tracks.Length; i++) {
				Rect trackRect = new Rect(0, trackHeight * i, view.width, trackHeight);

				DisplayTrack(trackRect, ed.scene.tracks[i]);
				float yPos = (i + 1) * trackHeight - 1;
				Handles.DrawLine(new Vector3(0, yPos), new Vector3(view.width, yPos));
			}
			
			// Draw overlay over in area
			Rect inOverlay = new Rect(0, 0, ed.scene.inPoint * ed.timelineZoom, view.height);
			GUI.DrawTexture(inOverlay, overlay);
			
			// Draw overlay over out area
			Rect outOverlay = new Rect(ed.scene.outPoint * ed.timelineZoom, 0, 0, view.height);
			outOverlay.width = view.width - outOverlay.x;
			GUI.DrawTexture(outOverlay, overlay);
			
			DrawLines(view);
		
		GUI.EndScrollView();
	}

	/// <summary>
	/// Displays visual tracks upon which clips sit.
	/// </summary>
	void DisplayTrack (Rect rect, CutsceneTrack track) {
		GUI.enabled = track.enabled;
		
		for (int i = track.clips.Count - 1; i >= 0; i--) {
			DisplayClip(rect, track, track.clips[i]);
		}

		GUI.enabled = true;

		// Handle clicks
		Vector2 mousePos = Event.current.mousePosition;
		if (Event.current.type == EventType.MouseDown && rect.Contains(mousePos)) {
			switch (Event.current.button) {
				case 0: // Left mouse button
					ed.selectedTrack = track;
					ed.selectedClip = null;
					break;
				case 1: // Right mouse button
					EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "CONTEXT/CutsceneTrack/", new MenuCommand(track));
					Event.current.Use();
					break;
				default:
					break;
			}

			Event.current.Use();
		}
	}

	/// <summary>
	/// Displays a clip.
	/// </summary>
	/// <param name="trackRect">The Rect of the track the clip sits on.</param>
	/// <param name="track">The track the clip sits on.</param>
	/// <param name="clip">The clip to display.</param>
	void DisplayClip (Rect trackRect, CutsceneTrack track, CutsceneClip clip) {
		const float trimWidth = 5f;

		GUIStyle clipStyle = ed.style.GetStyle("Selected Clip");
		// Set the clip style if this isn't the selected clip (selected clips all share the same style)
		if (clip != ed.selectedClip) {
			switch (clip.type) {
				case Cutscene.MediaType.Shots:
					clipStyle = ed.style.GetStyle("Shot Clip");
					break;
				case Cutscene.MediaType.Actors:
					clipStyle = ed.style.GetStyle("Actor Clip");
					break;
				case Cutscene.MediaType.Audio:
					clipStyle = ed.style.GetStyle("Audio Clip");
					break;
				default: // Cutscene.MediaType.Subtitles
					clipStyle = ed.style.GetStyle("Subtitle Clip");
					break;
			}
		}

		Rect rect = new Rect((trackRect.x + clip.timelineStart) * ed.timelineZoom, trackRect.y + 1, clip.duration * ed.timelineZoom, clipStyle.fixedHeight);

		GUI.BeginGroup(rect, clipStyle);

		GUIContent clipLabel = new GUIContent(clip.name, "Clip: " + clip.name + "\nDuration: " + clip.duration + "\nTimeline start: " + clip.timelineStart + "\nTimeline end: " + (clip.timelineStart + clip.duration));
		Rect clipLabelRect = new Rect(clipStyle.contentOffset.x, 0, rect.width - clipStyle.contentOffset.x, rect.height);
		GUI.Label(clipLabelRect, clipLabel);

		GUI.EndGroup();

		// Handle mouse clicks
		Vector2 mousePos = Event.current.mousePosition;
		if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
			switch (Event.current.button) {
				case 0: // Left mouse button
					ed.selectedClip = clip;
					ed.selectedTrack = track;
					break;
				case 1: // Right mouse button
					EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "CONTEXT/CutsceneClip/", new MenuCommand(clip));
					Event.current.Use();
					break;
				default:
					break;
			}
		}

		if (clip.setToDelete) {
			ed.selectedTrack.clips.Remove(clip);
			return;
		}

		// Don't allow actions to be performed on the clip if the track is disabled or locked
		if (!track.enabled || track.locked) {
			return;
		}

		switch (ed.currentTool) {
			case Tool.MoveResize:
				// Define edit areas, adding custom cursors when hovered over

				// Move
				Rect move = new Rect(rect.x + trimWidth, rect.y, rect.width - (2 * trimWidth), rect.height);
				EditorGUIUtility.AddCursorRect(move, MouseCursor.SlideArrow);

				// Resize left
				Rect resizeLeft = new Rect(rect.x, rect.y, trimWidth, rect.height);
				EditorGUIUtility.AddCursorRect(resizeLeft, MouseCursor.ResizeHorizontal);

				// Resize right
				Rect resizeRight = new Rect(rect.xMax - trimWidth, rect.y, trimWidth, rect.height);
				EditorGUIUtility.AddCursorRect(resizeRight, MouseCursor.ResizeHorizontal);

				if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
					ed.dragClip = clip;
				
					// Move
					if (move.Contains(Event.current.mousePosition)) {
						ed.dragEvent = DragEvent.Move;
						EDebug.Log("Cutscene Editor: starting clip move");
					// Resize left
					} else if (resizeLeft.Contains(Event.current.mousePosition)) {
						ed.dragEvent = DragEvent.ResizeLeft;
						EDebug.Log("Cutscene Editor: starting clip resize left");
					// Resize right
					} else if (resizeRight.Contains(Event.current.mousePosition)) {
						ed.dragEvent = DragEvent.ResizeRight;
						EDebug.Log("Cutscene Editor: starting clip resize right");
					}

					Event.current.Use();
				} else if (Event.current.type == EventType.MouseDrag && ed.dragClip == clip) {
					float shift = Event.current.delta.x / ed.timelineZoom;

					switch (ed.dragEvent) {
						case DragEvent.Move:
							float newPos = clip.timelineStart + shift;

							// Left collisions
							CutsceneClip leftCollision = track.ContainsClipAtTime(newPos, clip);
							if (leftCollision != null) {
								newPos = leftCollision.timelineStart + leftCollision.duration;
							}

							// Right collisions
							CutsceneClip rightCollision = track.ContainsClipAtTime(newPos + clip.duration, clip);
							if (rightCollision != null) {
								newPos = rightCollision.timelineStart - clip.duration;
							}

							if (newPos + clip.duration > ed.scene.duration) {
								newPos = ed.scene.duration - clip.duration;
							}

							clip.SetTimelineStart(newPos);
							break;
						case DragEvent.ResizeLeft:
							clip.SetTimelineStart(clip.timelineStart + shift);
							clip.SetInPoint(clip.inPoint + shift);

							// TODO Improve collision behaviour
							CutsceneClip leftResizeCollision = track.ContainsClipAtTime(clip.timelineStart, clip);
							if (leftResizeCollision != null) {


								clip.SetTimelineStart(leftResizeCollision.timelineStart + leftResizeCollision.duration);
							}

							break;
						case DragEvent.ResizeRight:
							float newOut = clip.outPoint + shift;

							// Right collisions
							CutsceneClip rightResizeCollision = track.ContainsClipAtTime(clip.timelineStart + clip.duration + shift, clip);
							if (rightResizeCollision != null) {
								newOut = rightResizeCollision.timelineStart - clip.timelineStart + clip.inPoint;
							}

							clip.SetOutPoint(newOut);
							break;
						default:
							break;
					}

					Event.current.Use();
				} else if (Event.current.type == EventType.MouseUp) {
					ed.dragClip = null;
					Event.current.Use();
				}

				break;
			case Tool.Scissors:
				// TODO Switch to something better than the text cursor, if possible
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.Text);

				if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
					SplitClip(track, clip, Event.current.mousePosition);
					Event.current.Use();
				}

				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Splits a clip into two separate ones.
	/// </summary>
	/// <param name="track">The track the clip is sitting on.</param>
	/// <param name="clip">The clip to split.</param>
	/// <param name="mousePosition">The position of the mouse when the split operation occurred.</param>
	/// <returns>The new clip.</returns>
	CutsceneClip SplitClip (CutsceneTrack track, CutsceneClip clip, Vector2 mousePosition) {
		float splitPoint = mousePosition.x / ed.timelineZoom;
		return CutsceneTimeline.SplitClipAtTime(splitPoint, track, clip);
	}

	/// <summary>
	/// Draws the in, out, and playhead lines.
	/// </summary>
	/// <param name="rect">The timeline's Rect.</param>
	void DrawLines (Rect rect) {
		DrawPlayhead(rect);

		Handles.color = inOutPointColour;
		DrawInLine(rect);
		DrawOutLine(rect);
	}

	/// <summary>
	/// Draws the playhead over the timeline.
	/// </summary>
	/// <param name="rect">The timeline's Rect.</param>
	void DrawPlayhead (Rect rect) {
		Handles.color = Color.black;
		float pos = ed.scene.playhead * ed.timelineZoom;

		Vector3 timelineTop    = new Vector3(pos, 0);
		Vector3 timelineBottom = new Vector3(pos, rect.yMax);
		Handles.DrawLine(timelineTop, timelineBottom);
	}

	/// <summary>
	/// Draws the in point line over the timeline.
	/// </summary>
	/// <param name="rect">The timeline's Rect.</param>
	void DrawInLine (Rect rect) {
		float pos = ed.scene.inPoint * ed.timelineZoom;

		Vector3 top    = new Vector3(pos, 0);
		Vector3 bottom = new Vector3(pos, rect.yMax);
		Handles.DrawLine(top, bottom);
	}

	/// <summary>
	/// Draws the out point line over the timeline.
	/// </summary>
	/// <param name="rect">The timeline's Rect.</param>
	void DrawOutLine (Rect rect) {
		float pos = ed.scene.outPoint * ed.timelineZoom;

		Vector3 top    = new Vector3(pos, 0);
		Vector3 bottom = new Vector3(pos, rect.yMax);
		Handles.DrawLine(top, bottom);
	}
}