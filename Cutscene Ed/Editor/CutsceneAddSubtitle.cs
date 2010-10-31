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

public class CutsceneAddSubtitle : ScriptableWizard {
	string dialog = "";

	/// <summary>
	/// Creates a wizard for adding a new subtitle.
	/// </summary>
	[MenuItem("Component/Cutscene/Add Media/Subtitle")]
	public static void CreateWizard () {
		DisplayWizard<CutsceneAddSubtitle>("Add Subtitle", "Add");
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
		isValid = dialog != "";
	}

	/// <summary>
	/// Adds the new subtitle to the cutscene.
	/// </summary>
	void OnWizardCreate () {
		Selection.activeGameObject.GetComponent<Cutscene>().NewSubtitle(dialog);
	}
}