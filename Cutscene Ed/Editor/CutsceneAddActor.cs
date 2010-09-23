using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class CutsceneAddActor : ScriptableWizard {
	GUISkin			style		= EditorGUIUtility.LoadRequired("Cutscene Ed/cutscene_editor_style.guiskin") as GUISkin;
	AnimationClip	selected	= null;
	GameObject		selectedGO	= null;
	
	/// <summary>
	/// Creates a wizard for adding a new actor.
	/// </summary>
	[MenuItem("Component/Cutscene/Add Media/Actor")]
	public static void CreateWizard () {
		DisplayWizard("Add Actor", typeof(CutsceneAddActor), "Add");
	}

	/// <summary>
	/// Validates the menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Add Media/Actor", true)]
	static bool ValidateCreateWizard () {
		return CutsceneEditor.CutsceneSelected;
	}

	void OnGUI () {
		OnWizardUpdate();

		// The generic version of FindObjectsOfType appears to be broken
		Animation[] animations = (Animation[])FindObjectsOfType(typeof(Animation));
		
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

	void OnWizardUpdate () {
		helpString = "Choose an animation to add.";
		// Only valid if an animation has been selected
		isValid = selected == null ? false : true;
	}

	/// <summary>
	/// Adds the new actor to the cutscene.
	/// </summary>
	void OnWizardCreate () {
		Cutscene scene = Selection.activeGameObject.GetComponent<Cutscene>();
		scene.NewActor(selected, selectedGO);
	}
}