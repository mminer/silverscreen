using UnityEngine;
using UnityEditor;

class CutsceneAddAudio : ScriptableWizard {
	//GUISkin		style		= EditorGUIUtility.LoadRequired("Cutscene Ed/cutscene_editor_style.guiskin") as GUISkin;

	AudioClip[]	audioClips;
	AudioClip	selected	= null;

	/// <summary>
	/// Creates a wizard for adding a new audio clip.
	/// </summary>
	[MenuItem("Component/Cutscene/Add Media/Audio")]
	public static void CreateWizard () {
		DisplayWizard("Add Audio", typeof(CutsceneAddAudio), "Add");
	}

	/// <summary>
	/// Validates the menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Add Media/Audio", true)]
	static bool ValidateCreateWizard () {
		return CutsceneEditor.CutsceneSelected;
	}

	void OnGUI () {
		OnWizardUpdate();

		// Display temporary workaround info
		GUILayout.Label("To add an audio clip from the Project view, drag and drop it to the audio pane. This is a temporary workaround and will hopefully be fixed soon.");

		// TODO Make LoadAllAssetsAtPath work
		/*
		audioClips = (AudioClip[])AssetDatabase.LoadAllAssetsAtPath("Assets");

		Debug.Log("Objects length: " + audioClips.Length);

		EditorGUILayout.BeginVertical(style.GetStyle("List Container"));

			foreach (AudioClip aud in audioClips) {
				GUIStyle itemStyle = aud == selected ? style.GetStyle("Selected List Item") : GUIStyle.none;

				Rect rect = EditorGUILayout.BeginHorizontal(itemStyle);

					GUIContent itemLabel = new GUIContent(aud.name, EditorGUIUtility.ObjectContent(null, typeof(AudioClip)).image);
					GUILayout.Label(itemLabel);
					
				EditorGUILayout.EndHorizontal();

				// Select when clicked
				if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
					selected = aud;
					EditorGUIUtility.PingObject(aud);
					Event.current.Use();
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
		GUI.enabled = true;*/
	}

	void OnWizardUpdate () {
		helpString = "Choose an audio clip to add.";
		// Only valid if an audio clip has been selected
		isValid = selected == null ? false : true;
	}

	/// <summary>
	/// Adds a new audio clip to the cutscene.
	/// </summary>
	void OnWizardCreate () {
		Cutscene scene = Selection.activeGameObject.GetComponent<Cutscene>();
		scene.NewAudio(selected);
	}
}