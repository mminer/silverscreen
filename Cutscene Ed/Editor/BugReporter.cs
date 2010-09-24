using UnityEditor;
using UnityEngine;

public class BugReporter : EditorWindow {
	string email	= "";
	string details	= "";

	readonly GUIContent emailLabel	= new GUIContent("Your Email", "If we require additional information, we'll contact you at the supplied address (optional).");
	readonly GUIContent submitLabel = new GUIContent("Submit", "Send this bug report.");

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