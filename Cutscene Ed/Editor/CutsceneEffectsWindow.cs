using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

class CutsceneEffectsWindow : ICutsceneGUI {
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

	public CutsceneEffectsWindow (CutsceneEditor ed) {
		this.ed = ed;
	}

	/// <summary>
	/// Displays the effects pane's GUI.
	/// </summary>
	/// <param name="rect">The effects pane's Rect.</param>
	public void OnGUI (Rect rect) {
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