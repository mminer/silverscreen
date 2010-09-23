using UnityEditor;
using UnityEngine;

class CutsceneOptions : ICutsceneGUI {
	readonly CutsceneEditor ed;

	public CutsceneOptions (CutsceneEditor ed) {
		this.ed = ed;
	}

	/// <summary>
	/// Displays the options window.
	/// </summary>
	/// <param name="rect">The options window's Rect.</param>
	public void OnGUI (Rect rect) {
		GUILayout.BeginArea(rect);

		ed.scene.inPoint = GUILayout.HorizontalSlider(ed.scene.inPoint, 0, 100);

		GUILayout.EndArea();
	}
}