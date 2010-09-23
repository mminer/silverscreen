using UnityEditor;
using UnityEngine;

class CutsceneMediaWindow : ICutsceneGUI {
	readonly CutsceneEditor ed;

	readonly GUIContent[] mediaTabs = {
		new GUIContent("Shots", "Camera views."),
		new GUIContent("Actors", "Animated game objects."),
		new GUIContent("Audio", "Dialog, background music and sound effects."),
		new GUIContent("Subtitles", "Textual versions of dialog.")
	};

	Cutscene.MediaType currentMediaTab = Cutscene.MediaType.Shots;
	Vector2[] mediaScrollPos = { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
	CutsceneMedia selectedMediaItem;

	readonly GUIContent newMediaLabel		= new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/icon_add.png") as Texture, "Add new media.");
	readonly GUIContent insertMediaLabel	= new GUIContent("Insert", "Insert the selected shot onto the timeline.");

	public CutsceneMediaWindow (CutsceneEditor ed) {
		this.ed = ed;
	}

	/// <summary>
	/// Displays the media pane's GUI.
	/// </summary>
	/// <param name="rect">The media pane's Rect.</param>
	public void OnGUI (Rect rect) {
		GUILayout.BeginArea(rect, ed.style.GetStyle("Pane"));

		EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();
			currentMediaTab = (Cutscene.MediaType)GUILayout.Toolbar((int)currentMediaTab, mediaTabs, GUILayout.Width(CutsceneEditor.PaneTabsWidth(mediaTabs.Length)));
			GUILayout.FlexibleSpace();

		EditorGUILayout.EndHorizontal();

		switch (currentMediaTab) {
			case Cutscene.MediaType.Shots:
				// If the selected item is in a different tab, set it to null
				if (selectedMediaItem != null && selectedMediaItem.type != currentMediaTab) {
					selectedMediaItem = null;
				}

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

					if (GUILayout.Button(newMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						ed.scene.NewShot();
					}

					GUI.enabled = IsMediaInsertable();
					if (GUILayout.Button(insertMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						Insert(selectedMediaItem);
					}
					GUI.enabled = true;

					GUILayout.FlexibleSpace();

				EditorGUILayout.EndHorizontal();

				mediaScrollPos[(int)currentMediaTab] = EditorGUILayout.BeginScrollView(mediaScrollPos[(int)currentMediaTab], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					foreach (CutsceneShot shot in ed.scene.shots) {

						Rect itemRect = EditorGUILayout.BeginHorizontal(selectedMediaItem == shot ? ed.style.GetStyle("Selected List Item") : GUIStyle.none);

							GUIContent itemLabel = new GUIContent(shot.name, ed.mediaIcons[(int)Cutscene.MediaType.Shots]);
							GUILayout.Label(itemLabel);

						EditorGUILayout.EndHorizontal();

						HandleMediaItemClicks(shot, itemRect);
					}
				EditorGUILayout.EndScrollView();

				break;
			case Cutscene.MediaType.Actors:
				// If the selected item is in a different tab, set it to null
				if (selectedMediaItem != null && selectedMediaItem.type != currentMediaTab) {
					selectedMediaItem = null;
				}

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

					if (GUILayout.Button(newMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						CutsceneAddActor.CreateWizard();
					}

					GUI.enabled = IsMediaInsertable();
					if (GUILayout.Button(insertMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						Insert(selectedMediaItem);
					}
					GUI.enabled = true;

					GUILayout.FlexibleSpace();

				EditorGUILayout.EndHorizontal();

				mediaScrollPos[(int)currentMediaTab] = EditorGUILayout.BeginScrollView(mediaScrollPos[(int)currentMediaTab], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					foreach (CutsceneActor actor in ed.scene.actors) {

						Rect itemRect = EditorGUILayout.BeginHorizontal(selectedMediaItem == actor ? ed.style.GetStyle("Selected List Item") : GUIStyle.none);

						GUIContent itemLabel = new GUIContent(actor.anim.name, ed.mediaIcons[(int)Cutscene.MediaType.Actors]);
						GUILayout.Label(itemLabel);

						EditorGUILayout.EndHorizontal();

						HandleMediaItemClicks(actor, itemRect);
					}
				EditorGUILayout.EndScrollView();

				break;
			case Cutscene.MediaType.Audio:
				// If the selected item is in a different tab, set it to null
				if (selectedMediaItem != null && selectedMediaItem.type != currentMediaTab) {
					selectedMediaItem = null;
				}

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

					if (GUILayout.Button(newMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						// Show a wizard that will take care of adding audio
						CutsceneAddAudio.CreateWizard();
					}

					GUI.enabled = IsMediaInsertable();
					if (GUILayout.Button(insertMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						Insert(selectedMediaItem);
					}
					GUI.enabled = true;

					GUILayout.FlexibleSpace();

				EditorGUILayout.EndHorizontal();

				mediaScrollPos[(int)currentMediaTab] = EditorGUILayout.BeginScrollView(mediaScrollPos[(int)currentMediaTab], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					foreach (CutsceneAudio aud in ed.scene.audioSources) {

						Rect itemRect = EditorGUILayout.BeginHorizontal(selectedMediaItem == aud ? ed.style.GetStyle("Selected List Item") : GUIStyle.none);

						GUIContent itemLabel = new GUIContent(aud.name, ed.mediaIcons[(int)Cutscene.MediaType.Audio]);
						GUILayout.Label(itemLabel);

						EditorGUILayout.EndHorizontal();

						HandleMediaItemClicks(aud, itemRect);
					}
				EditorGUILayout.EndScrollView();

				break;
			case Cutscene.MediaType.Subtitles:
				// If the selected item is in a different tab, set it to null
				if (selectedMediaItem != null && selectedMediaItem.type != currentMediaTab) {
					selectedMediaItem = null;
				}

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

					if (GUILayout.Button(newMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						// Show a wizard that will take care of adding audio
						CutsceneAddSubtitle.CreateWizard();
					}

					GUI.enabled = IsMediaInsertable();
					if (GUILayout.Button(insertMediaLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						Insert(selectedMediaItem);
					}
					GUI.enabled = true;

				GUILayout.FlexibleSpace();

				EditorGUILayout.EndHorizontal();

				mediaScrollPos[(int)currentMediaTab] = EditorGUILayout.BeginScrollView(mediaScrollPos[(int)currentMediaTab], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					foreach (CutsceneSubtitle subtitle in ed.scene.subtitles) {

						Rect itemRect = EditorGUILayout.BeginHorizontal(selectedMediaItem == subtitle ? ed.style.GetStyle("Selected List Item") : GUIStyle.none);

						GUIContent itemLabel = new GUIContent(ed.mediaIcons[(int)Cutscene.MediaType.Subtitles]);
						GUILayout.Label(itemLabel, GUILayout.ExpandWidth(false));
						subtitle.dialog = EditorGUILayout.TextField(subtitle.dialog);

						EditorGUILayout.EndHorizontal();

						HandleMediaItemClicks(subtitle, itemRect);
					}
				EditorGUILayout.EndScrollView();

				break;
			default:
				break;
		}
		GUILayout.EndArea();

		// Handle drag and drop events
		if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) {
			// Show a copy icon on the drag
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition)) {
				DragAndDrop.AcceptDrag();
				Object[] draggedObjects = DragAndDrop.objectReferences;

				// Create the appropriate cutscene objects for each dragged object
				foreach (Object obj in draggedObjects) {
					if (obj is AudioClip) {
						ed.scene.NewAudio((AudioClip)obj);
					} else if (obj is GameObject && ((GameObject)obj).animation != null) {
						GameObject go = obj as GameObject;
						foreach (AnimationClip anim in AnimationUtility.GetAnimationClips(go.animation)) {
							ed.scene.NewActor(anim, go);
						}

					}
					EDebug.Log("Cutscene Editor: dropping " + obj.GetType());
				}
			}
			Event.current.Use();
		}
	}

	/// <summary>
	/// Handles left and right mouse clicks of media items.
	/// </summary>
	/// <param name="item">The item clicked on.</param>
	/// <param name="rect">The item's Rect.</param>
	void HandleMediaItemClicks (CutsceneMedia item, Rect rect) {
		Vector2 mousePos = Event.current.mousePosition;
		if (Event.current.type == EventType.MouseDown && rect.Contains(mousePos)) {
			switch (Event.current.button) {
				case 0: // Left click
					selectedMediaItem = item;
					EditorGUIUtility.PingObject(item);
					break;
				case 1: // Right click
					EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "CONTEXT/CutsceneObject/", new MenuCommand(item));
					break;
				default:
					break;
			}
			Event.current.Use();
		}
	}

	/// <summary>
	/// Determines whether or not the currently selected cutscene object can be inserted into the selected timeline.
	/// </summary>
	/// <returns>True if the selected clip and the selected track are the same, false otherwise.</returns>
	bool IsMediaInsertable () {
		return selectedMediaItem != null && selectedMediaItem.type == ed.selectedTrack.type;
	}

	/// <summary>
	/// Inserts the given cutscene object into the timeline.
	/// </summary>
	/// <param name="obj">The cutscene object to insert.</param>
	void Insert (CutsceneMedia obj) {
		CutsceneClip newClip = new CutsceneClip(obj);
		newClip.timelineStart = ed.scene.playhead;

		// If there are no existing tracks, add a new one
		if (ed.scene.tracks.Length == 0) {
			ed.scene.AddTrack(newClip.type);
		}

		// Manage overlap with other clips
		CutsceneClip existingClip = ed.selectedTrack.ContainsClipAtTime(ed.scene.playhead);
		if (existingClip != null) {
			CutsceneClip middleOfSplit = CutsceneTimeline.SplitClipAtTime(ed.scene.playhead, ed.selectedTrack, existingClip);
			CutsceneTimeline.SplitClipAtTime(ed.scene.playhead + newClip.duration, ed.selectedTrack, middleOfSplit);
			ed.selectedTrack.clips.Remove(middleOfSplit);
		}

		ed.selectedTrack.clips.Add(newClip);

		EDebug.Log("Cutscene Editor: inserting " + newClip.name + " into timeline " + ed.selectedTrack + " at " + ed.scene.playhead);
	}
}