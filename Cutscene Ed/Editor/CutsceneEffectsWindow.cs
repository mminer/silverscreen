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
using System;
using System.Collections.Generic;

class CutsceneEffectsWindow : ICutsceneGUI
{
	readonly CutsceneEditor ed;

	static Dictionary<string, Type> filters = new Dictionary<string, Type>
	{
		{ CutsceneBlurFilter.name,   typeof(CutsceneBlurFilter) },
		{ CutsceneInvertFilter.name, typeof(CutsceneInvertFilter) }
	};

	static Dictionary<string, Type> transitions = new Dictionary<string, Type> {};

	static readonly GUIContent filtersLabel     = new GUIContent("Filters",     "Full screen filters.");
	static readonly GUIContent transitionsLabel = new GUIContent("Transitions", "Camera transitions.");

	readonly GUIContent[] effectsTabs = CutsceneEditor.HasPro ? new GUIContent[] { filtersLabel, transitionsLabel } : new GUIContent[] { transitionsLabel };
	Cutscene.EffectType currentEffectsTab = Cutscene.EffectType.Filters;

	Type selectedEffect;

	readonly Texture[] effectsIcons = {
		EditorGUIUtility.LoadRequired("Cutscene Ed/effects_filter.png")     as Texture,
		EditorGUIUtility.LoadRequired("Cutscene Ed/effects_transition.png") as Texture
	};

	public CutsceneEffectsWindow (CutsceneEditor ed)
	{
		this.ed = ed;
	}

	/// <summary>
	/// Displays the effects pane's GUI.
	/// </summary>
	/// <param name="rect">The effects pane's Rect.</param>
	public void OnGUI (Rect rect)
	{
		GUILayout.BeginArea(rect, ed.style.GetStyle("Pane"));

		EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();
			currentEffectsTab = (Cutscene.EffectType)GUILayout.Toolbar((int)currentEffectsTab, effectsTabs, GUILayout.Width(CutsceneEditor.PaneTabsWidth(effectsTabs.Length)));
			GUILayout.FlexibleSpace();

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

			GUI.enabled = ed.selectedClip != null && ed.selectedClip.type == Cutscene.MediaType.Shots && selectedEffect != null;

			GUIContent applyLabel = new GUIContent("Apply", "Apply the selected effect to the selected clip.");
			if (GUILayout.Button(applyLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
				if (ed.selectedClip != null) {
					ed.selectedClip.ApplyEffect(selectedEffect);
				}
			}

			GUI.enabled = true;

		EditorGUILayout.EndHorizontal();

		switch (currentEffectsTab) {
			case Cutscene.EffectType.Filters:
				foreach (KeyValuePair<string, Type> item in filters) {

					Rect itemRect = EditorGUILayout.BeginHorizontal(selectedEffect == item.Value ? ed.style.GetStyle("Selected List Item") : GUIStyle.none);
						
						GUIContent filterLabel = new GUIContent(item.Key, effectsIcons[(int)Cutscene.EffectType.Filters]);
						GUILayout.Label(filterLabel);

					EditorGUILayout.EndHorizontal();

					// Handle clicks
					if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition)) {
						selectedEffect = item.Value;
						Event.current.Use();
					}
				}
				break;
			
			case Cutscene.EffectType.Transitions:
				foreach (KeyValuePair<string, Type> item in transitions) {

					Rect itemRect = EditorGUILayout.BeginHorizontal(selectedEffect == item.Value ? ed.style.GetStyle("Selected List Item") : GUIStyle.none);
						GUIContent transitionLabel = new GUIContent(item.Key, effectsIcons[(int)Cutscene.EffectType.Transitions]);
						GUILayout.Label(transitionLabel);

					EditorGUILayout.EndHorizontal();

					// Handle clicks
					if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition)) {
						selectedEffect = item.Value;
						Event.current.Use();
					}
				}
				break;
			
			default:
				break;
		}

		GUILayout.EndArea();
	}
}