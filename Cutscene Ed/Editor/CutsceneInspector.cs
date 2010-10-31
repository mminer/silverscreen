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

/// <summary>
/// A custom inspector for a cutscene.
/// </summary>
[CustomEditor(typeof(Cutscene))]
public class CutsceneInspector : Editor
{
	Cutscene scene {
		get { return target as Cutscene; }
	}
	
	public override void OnInspectorGUI ()
	{
		/*
		GUILayout.Label("Timing", EditorStyles.boldLabel);

		GUIContent durationLabel = new GUIContent("Duration", "The entire length of the cutscene.");
		scene.duration = EditorGUILayout.FloatField(durationLabel, scene.duration);

		// Instead of trying to clamp the values of the in and out points by slider leftValue and rightValue of the slider, we leave it to the Cutscene class
		GUIContent inPointLabel = new GUIContent("In", "The point at which the cutscene starts.");
		scene.inPoint = EditorGUILayout.Slider(inPointLabel, scene.inPoint, 0f, scene.duration);

		GUIContent outPointLabel = new GUIContent("Out", "The point at which the cutscene ends.");
		scene.outPoint = EditorGUILayout.Slider(outPointLabel, scene.outPoint, 0f, scene.duration);

		GUILayout.Label("Options", EditorStyles.boldLabel);

		GUIContent stopPlayerLabel = new GUIContent("Stop Player", "Deactivate the player when the scene starts.");
		scene.stopPlayer = EditorGUILayout.Toggle(stopPlayerLabel, scene.stopPlayer);

		// Disable the player reference box if the stop player toggle in unchecked
		GUI.enabled = scene.stopPlayer ? true : false;

		GUIContent playerLabel = new GUIContent("Player", "The player to deactivate.");
		scene.player = EditorGUILayout.ObjectField(playerLabel, scene.player, typeof(GameObject)) as GameObject;

		GUI.enabled = true;

		EditorGUILayout.Separator();*/
		
		DrawDefaultInspector();

		if (GUILayout.Button("Edit")) {
			CutsceneEditor.OpenEditor();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}

	public void OnSceneGUI ()
	{
		Handles.BeginGUI();
			
			GUI.Box(new Rect(0, Screen.height - 300, 200, 30), "Cutscene Preview");
			
			//Rect camRect = GUILayoutUtility.GetRect(100, 100);
			if (scene != null) {

				Camera cam = null;

				foreach (CutsceneTrack track in scene.tracks) {
					if (track.type == Cutscene.MediaType.Shots) {
						CutsceneClip clip = track.ContainsClipAtTime(scene.playhead);
						if (clip != null) {
							cam = ((CutsceneShot)clip.master).camera;
							break;
						}
					}
				}

				if (cam != null) {
					DrawCamera(new Rect(0, 0, 200, 200), cam);
				}
			}

		Handles.EndGUI();
	}

	void DrawCamera (Rect previewRect, Camera camera)
	{
		if (Event.current.type == EventType.Repaint) {
			Rect cameraOriginalRect = camera.pixelRect;
			camera.pixelRect = previewRect;
			camera.Render();
			camera.pixelRect = cameraOriginalRect;
		}
	}
}
