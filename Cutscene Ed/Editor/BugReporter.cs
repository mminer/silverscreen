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

public class BugReporter : EditorWindow {
	string email   = "";
	string details = "";

	readonly GUIContent emailLabel  = new GUIContent("Your Email", "If we require additional information, we'll contact you at the supplied address (optional).");
	readonly GUIContent submitLabel = new GUIContent("Submit",     "Send this bug report.");

	/// <summary>
	/// Adds "Bug Reporter" to the Window menu.
	/// </summary>
	[MenuItem("Window/Bug Reporter")]
	public static void OpenEditor () {
		// Get existing open window or if none, make a new one
		GetWindow<BugReporter>(true, "Bug Reporter").Show();
	}

	void OnGUI () {
		GUILayout.Label("Please take a minute to tell us what happened in as much detail as possible. This makes it possible for us to fix the problem.");

		EditorGUILayout.Separator();

		email = EditorGUILayout.TextField(emailLabel, email);

		EditorGUILayout.Separator();

		GUILayout.Label("Problem Details:");
		details = EditorGUILayout.TextArea(details);

		EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button(submitLabel, GUILayout.ExpandWidth(false))) {
				// TODO Send bug report
				Debug.Log("Bug report sent.");
			}
		EditorGUILayout.EndHorizontal();
	}
}