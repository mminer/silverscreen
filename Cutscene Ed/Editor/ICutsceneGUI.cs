using UnityEngine;

interface ICutsceneGUI {
	/// <summary>
	/// Displays the GUI for this element.
	/// </summary>
	/// <param name="rect">The bounding box this element is contained in.</param>
	void OnGUI (Rect rect);
}