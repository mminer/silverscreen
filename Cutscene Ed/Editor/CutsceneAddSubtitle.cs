using UnityEngine;
using UnityEditor;

public class CutsceneAddSubtitle : ScriptableWizard {
	string dialog = "";

	/// <summary>
	/// Creates a wizard for adding a new subtitle.
	/// </summary>
	[MenuItem("Component/Cutscene/Add Media/Subtitle")]
	public static void CreateWizard () {
		DisplayWizard("Add Subtitle", typeof(CutsceneAddSubtitle), "Add");
	}

	/// <summary>
	/// Validates the menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Add Media/Subtitle", true)]
	static bool ValidateCreateWizard () {
		return CutsceneEditor.CutsceneSelected;
	}

	void OnGUI () {
		OnWizardUpdate();

		EditorGUILayout.BeginHorizontal();

			GUIContent itemLabel = new GUIContent("Dialog", EditorGUIUtility.ObjectContent(null, typeof(TextAsset)).image);
			GUILayout.Label(itemLabel, GUILayout.ExpandWidth(false));
			dialog = EditorGUILayout.TextArea(dialog);

		EditorGUILayout.EndHorizontal();
		
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
		helpString = "Type in some dialog to add.";
		// Only valid if some text has been entered in the text field
		isValid = dialog == "" ? false : true;
	}

	/// <summary>
	/// Adds the new subtitle to the cutscene.
	/// </summary>
	void OnWizardCreate () {
		Cutscene scene = Selection.activeGameObject.GetComponent<Cutscene>();
		scene.NewSubtitle(dialog);
	}
}