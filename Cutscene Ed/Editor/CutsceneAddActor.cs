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
using System.Collections;
using System.Collections.Generic;

class CutsceneAddActor : ScriptableWizard
{
	GUISkin       style = EditorGUIUtility.LoadRequired("Cutscene Ed/cutscene_editor_style.guiskin") as GUISkin;
	AnimationClip selected;
	GameObject    selectedGO;
	
	/// <summary>
	/// Creates a wizard for adding a new actor.
	/// </summary>
	[MenuItem("Component/Cutscene/Add Media/Actor")]
	public static void CreateWizard () {
		DisplayWizard<CutsceneAddActor>("Add Actor", "Add");
	}

	/// <summary>
	/// Validates the menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Add Media/Actor", true)]
	static bool ValidateCreateWizard () {
		return CutsceneEditor.CutsceneSelected;
	}

	void OnGUI ()
	{
		OnWizardUpdate();
		
		Object[] animations = FindObjectsOfType(typeof(Animation));
		
		EditorGUILayout.BeginVertical(style.GetStyle("List Container"));

			foreach (Animation anim in animations) {
				GUILayout.Label(anim.gameObject.name, EditorStyles.largeLabel);

				foreach (AnimationClip clip in AnimationUtility.GetAnimationClips(anim)) {
					GUIStyle itemStyle = GUIStyle.none;
					if (clip == selected) {
						itemStyle = style.GetStyle("Selected List Item");
					}
					
					Rect rect = EditorGUILayout.BeginHorizontal(itemStyle);

						GUIContent itemLabel = new GUIContent(clip.name, EditorGUIUtility.ObjectContent(null, typeof(Animation)).image);
						GUILayout.Label(itemLabel);

					EditorGUILayout.EndHorizontal();

					// Select when clicked
					if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
						selected = clip;
						selectedGO = anim.gameObject;
						EditorGUIUtility.PingObject(anim);
						Event.current.Use();
						Repaint();
					}
				}
			}

		EditorGUILayout.EndVertical();

		GUI.enabled = isValid;
		EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Add", GUILayout.ExpandWidth(false))) {
				OnWizardCreate();
				Close();
			}

		EditorGUILayout.EndHorizontal();
		GUI.enabled = true;
	}

	void OnWizardUpdate ()
	{
		helpString = "Choose an animation to add.";
		// Only valid if an animation has been selected
		isValid = selected != null;
	}

	/// <summary>
	/// Adds the new actor to the cutscene.
	/// </summary>
	void OnWizardCreate ()
	{
		Selection.activeGameObject.GetComponent<Cutscene>().NewActor(selected, selectedGO);
	}
}