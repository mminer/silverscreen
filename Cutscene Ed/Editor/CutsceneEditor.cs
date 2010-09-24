using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Cutscene Editor, where tracks and clips can be managed in a fashion similar to non-linear video editors.
/// </summary>
public class CutsceneEditor : EditorWindow {
	public static readonly System.Version version = new System.Version(0, 2);

	public Cutscene scene {
		get {
			Object[] scenes = Selection.GetFiltered(typeof(Cutscene), SelectionMode.TopLevel);
			
			if (scenes.Length == 0) {
				return null;
			}
			
			return scenes[0] as Cutscene;
		}
	}

	public GUISkin style { get; private set; }

	//ICutsceneGUI options;
	ICutsceneGUI media;
	ICutsceneGUI effects;
	ICutsceneGUI tools;
	ICutsceneGUI timeline;

	public static bool CutsceneSelected {
		get { return Selection.GetFiltered(typeof(Cutscene), SelectionMode.TopLevel).Length > 0; }
	}

	public static bool HasPro = SystemInfo.supportsImageEffects;

	// Icons:

	public readonly Texture[] mediaIcons = {
		EditorGUIUtility.LoadRequired("Cutscene Ed/media_shot.png")		as Texture,
		EditorGUIUtility.LoadRequired("Cutscene Ed/media_actor.png")	as Texture,
		EditorGUIUtility.LoadRequired("Cutscene Ed/media_audio.png")	as Texture,
		EditorGUIUtility.LoadRequired("Cutscene Ed/media_subtitle.png")	as Texture
	};

	// Timeline:
	
	public Tool currentTool = Tool.MoveResize;
	
	public Vector2 timelineScrollPos;

	public float timelineMin { get; set; }

	float _timelineZoom = CutsceneTimeline.timelineZoomMin;
	public float timelineZoom {
		get { return _timelineZoom; }
		set { _timelineZoom = Mathf.Clamp(value, CutsceneTimeline.timelineZoomMin, CutsceneTimeline.timelineZoomMax); }
	}
	
	CutsceneTrack _selectedTrack;
	public CutsceneTrack selectedTrack {
		get {
			if (_selectedTrack == null) {

				// If there are no tracks, add a new one
				if (scene.tracks.Length == 0) {
					scene.AddTrack(Cutscene.MediaType.Shots);
				}

				_selectedTrack = scene.tracks[0];
			}

			return _selectedTrack;
		}
		set { _selectedTrack = value; }
	}

	public CutsceneClip selectedClip;
	public DragEvent dragEvent = DragEvent.Move;
	public CutsceneClip dragClip;

	// Menu items:

	/// <summary>
	/// Adds "Cutscene Editor" to the Window menu.
	/// </summary>
	[MenuItem("Window/Cutscene Editor %9")]
	public static void OpenEditor () {
		// Get existing open window or if none, make a new one
		GetWindow<CutsceneEditor>(false, "Cutscene Editor").Show();
	}

	/// <summary>
	/// Validates the Cutscene Editor menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Window/Cutscene Editor %9", true)]
	static bool ValidateOpenEditor () {
		return CutsceneSelected;
	}

	/// <summary>
	/// Adds the option to create a cutscene from the GameObject > Create Other menu.
	/// </summary>
	/// <returns>The new cutscene.</returns>
	[MenuItem("GameObject/Create Other/Cutscene")]
	static void CreateCutscene () {
		// Create the new cutscene game object
		GameObject newSceneGO = new GameObject("Cutscene", typeof(Cutscene));
		Cutscene newScene = newSceneGO.GetComponent<Cutscene>();
		
		// Add some tracks to get the user started
		newScene.AddTrack(Cutscene.MediaType.Shots);
		newScene.AddTrack(Cutscene.MediaType.Actors);
		newScene.AddTrack(Cutscene.MediaType.Audio);
		newScene.AddTrack(Cutscene.MediaType.Subtitles);

		// Create the cutscene's media game objects
		GameObject shots		= new GameObject("Shots");
		GameObject actors		= new GameObject("Actors");
		GameObject audio		= new GameObject("Audio");
		GameObject subtitles	= new GameObject("Subtitles");

		// Make the media game objects a child of the cutscene
		shots.transform.parent		= newScene.transform;
		actors.transform.parent		= newScene.transform;
		audio.transform.parent		= newScene.transform;
		subtitles.transform.parent	= newScene.transform;

		// Create the master animation clip
		AnimationClip masterClip = new AnimationClip();
		newScene.masterClip = masterClip;
		newScene.animation.AddClip(masterClip, "master");

		newScene.animation.playAutomatically = false;
		newScene.animation.wrapMode = WrapMode.Once;

		EDebug.Log("Cutscene Editor: created a new cutscene");
	}

	/// <summary>
	/// Adds the option to create a cutscene trigger to the GameObject > Create Other menu.
	/// </summary>
	/// <returns>The trigger's collider.</returns>
	[MenuItem("GameObject/Create Other/Cutscene Trigger")]
	static Collider CreateCutsceneTrigger () {
		// Create the new cutscene trigger game object
		GameObject triggerGO = new GameObject("Cutscene Trigger", typeof(BoxCollider), typeof(CutsceneTrigger));
		triggerGO.collider.isTrigger = true;
		return triggerGO.collider;
	}

	/// <summary>
	/// Adds the option to create a new shot track in the Component > Cutscene > Track menu.
	/// </summary>
	[MenuItem("Component/Cutscene/Track/Shot")]
	static void CreateShotTrack () {
		Selection.activeGameObject.GetComponent<Cutscene>().AddTrack(Cutscene.MediaType.Shots);
	}

	/// <summary>
	/// Validates the Shot menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Track/Shot", true)]
	static bool ValidateCreateShotTrack () {
		return CutsceneSelected;
	}

	/// <summary>
	/// Adds the option to create a new actor track in the Component > Cutscene > Track menu.
	/// </summary>
	[MenuItem("Component/Cutscene/Track/Actor")]
	static void CreateActorTrack () {
		Selection.activeGameObject.GetComponent<Cutscene>().AddTrack(Cutscene.MediaType.Actors);
	}

	/// <summary>
	/// Validates the Actor menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Track/Actor", true)]
	static bool ValidateCreateActorTrack () {
		return CutsceneSelected;
	}

	/// <summary>
	/// Adds the option to create a new audio track in the Component > Cutscene > Track menu.
	/// </summary>
	[MenuItem("Component/Cutscene/Track/Audio")]
	static void CreateAudioTrack () {
		Selection.activeGameObject.GetComponent<Cutscene>().AddTrack(Cutscene.MediaType.Audio);
	}

	/// <summary>
	/// Validates the Audio menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Track/Audio", true)]
	static bool ValidateCreateAudioTrack () {
		return CutsceneSelected;
	}

	/// <summary>
	/// Adds the option to create a new subtitle track in the Component > Cutscene > Track menu.
	/// </summary>
	[MenuItem("Component/Cutscene/Track/Subtitle")]
	static void CreateSubtitleTrack () {
		Selection.activeGameObject.GetComponent<Cutscene>().AddTrack(Cutscene.MediaType.Subtitles);
	}

	/// <summary>
	/// Validates the Subtitle menu item.
	/// </summary>
	/// <remarks>The item will be disabled if no cutscene is selected.</remarks>
	[MenuItem("Component/Cutscene/Track/Subtitle", true)]
	static bool ValidateCreateSubtitleTrack () {
		return CutsceneSelected;
	}

	void OnEnable () {
		style = EditorGUIUtility.LoadRequired("Cutscene Ed/cutscene_editor_style.guiskin") as GUISkin;

		if (style == null) {
			Debug.LogError("GUISkin for Cutscene Editor missing");
			return;
		}

		//options		= new CutsceneOptions(this);
		media		= new CutsceneMediaWindow(this);
		effects		= new CutsceneEffectsWindow(this);
		tools		= new CutsceneTools(this);
		timeline	= new CutsceneTimeline(this);
	}

	/// <summary>
	/// Displays the editor GUI.
	/// </summary>
	void OnGUI () {
		if (style == null) {
			style = EditorGUIUtility.LoadRequired("Cutscene Ed/cutscene_editor_style.guiskin") as GUISkin;
		}

		// If no cutscene is selected, present the user with the option to create one
		if (scene == null) {
			if (GUILayout.Button("Create New Cutscene", GUILayout.ExpandWidth(false))) {
				CreateCutscene();
			}
		} else { // Otherwise present the cutscene editor
			float windowHeight = style.GetStyle("Pane").fixedHeight;

			// Options window
			//Rect optionsRect = new Rect(0, 0, position.width / 3, windowHeight);
			//options.OnGUI(optionsRect);

			// Media window
			Rect mediaRect = new Rect(2, 2, position.width / 2, windowHeight);
			media.OnGUI(mediaRect);

			// Effects window
			Rect effectsRect = new Rect(mediaRect.xMax + 2, 2, position.width - mediaRect.xMax - 4, windowHeight);
			effects.OnGUI(effectsRect);

			// Cutting tools
			Rect toolsRect = new Rect(0, mediaRect.yMax, position.width, 25);
			tools.OnGUI(toolsRect);

			// Timeline
			Rect timelineRect = new Rect(0, toolsRect.yMax, position.width, position.height - toolsRect.yMax);
			timeline.OnGUI(timelineRect);
		}
	}

	// Context menus:

	/// <summary>
	/// Deletes a piece of media by invoking a context menu item.
	/// </summary>
	/// <param name="command">The context menu command.</param>
	[MenuItem("CONTEXT/CutsceneObject/Delete")]
	static void DeleteCutsceneMedia (MenuCommand command) {
		DeleteCutsceneMedia(command.context as CutsceneMedia);
	}

	/// <summary>
	/// Deletes a piece of media.
	/// </summary>
	/// <param name="obj">The media to delete.</param>
	/// <remarks>This has to be in CutsceneEditor rather than CutsceneTimeline because it uses the DestroyImmediate function, which is only available to classes which inherit from UnityEngine.Object.</remarks>
	static void DeleteCutsceneMedia (CutsceneMedia obj) {
		bool delete = true;

		// Display a dialog to prevent accidental deletions
		if (EditorPrefs.GetBool("Cutscene Warn Before Delete", true)) {
			delete = EditorUtility.DisplayDialog("Delete Object", "Are you sure you wish to delete this object? Changes cannot be undone.", "Delete", "Cancel");
		}

		// Only delete the cutscene object if the user chose to
		if (delete) {
			Debug.Log("Cutscene Editor: deleting media " + obj.name);
			if (obj.type == Cutscene.MediaType.Actors || obj.type == Cutscene.MediaType.Subtitles) { // Delete the CutsceneObject component
				DestroyImmediate(obj);
			} else { // Delete the actual game object
				DestroyImmediate(obj.gameObject);
			}
		}
	}

	/// <summary>
	/// Deletes a track by invoking a context menu item.
	/// </summary>
	/// <param name="command">The context menu command.</param>
	/// <returns>True if the track was successfully deleted, false otherwise.</returns>
	[MenuItem("CONTEXT/CutsceneTrack/Delete Track")]
	static bool DeleteTrack (MenuCommand command) {
		return DeleteTrack(command.context as CutsceneTrack);
	}

	/// <summary>
	/// Deletes a track.
	/// </summary>
	/// <param name="track">The track to delete.</param>
	/// <returns>True if the track was successfully deleted, false otherwise.</returns>
	static bool DeleteTrack (CutsceneTrack track) {
		bool delete = true;

		// Display a dialog to prevent accidental deletions
		if (EditorPrefs.GetBool("Cutscene Warn Before Delete", true)) {
			delete = EditorUtility.DisplayDialog("Delete Track", "Are you sure you wish to delete this track? Changes cannot be undone.", "Delete", "Cancel");
		}

		if (delete) {
			DestroyImmediate(track);
		}

		return delete;
	}

	/// <summary>
	/// Deletes a clip by invoking a context menu item.
	/// </summary>
	/// <param name="command">The context menu command.</param>
	/// <returns>True if the clip was successfully deleted, false otherwise.</returns>
	[MenuItem("CONTEXT/CutsceneClip/Delete Clip")]
	static bool DeleteClip (MenuCommand command) {
		return DeleteClip(command.context as CutsceneClip);
	}

	/// <summary>
	/// Deletes a clip.
	/// </summary>
	/// <param name="clip">The clip to delete.</param>
	/// <returns>True if the clip was successfully deleted, false otherwise.</returns>
	static bool DeleteClip (CutsceneClip clip) {
		bool delete = true;

		// Display a dialog to prevent accidental deletions
		if (EditorPrefs.GetBool("Cutscene Warn Before Delete", true)) {
			delete = EditorUtility.DisplayDialog("Delete Clip", "Are you sure you wish to delete this clip? Changes cannot be undone.", "Delete", "Cancel");
		}

		clip.setToDelete = delete;
		return delete;
	}

	/// <summary>
	/// Selects the track at the specified index.
	/// </summary>
	/// <param name="index">The track to select.</param>
	void SelectTrackAtIndex (int index) {
		if (scene.tracks.Length > 0 && index > 0 && index <= scene.tracks.Length) {
			selectedTrack = scene.tracks[index - 1];
			EDebug.Log("Cutscene Editor: track " + index + " is selected");
		}
	}

	/// <summary>
	/// Determines which key command is pressed and responds accordingly.
	/// </summary>
	/// <param name="keyDownEvent">The keyboard event.</param>
	/// <returns>True if the keyboard shortcut exists, false otherwise.</returns>
	public void HandleKeyboardShortcuts (Event keyDownEvent) {
		KeyCode key = keyDownEvent.keyCode;

		// Tools:
		
		// Move/resize
		if (key == CutsceneHotkeys.MoveResizeTool.key) {
			currentTool = Tool.MoveResize;
			EDebug.Log("Cutscene Editor: switching to Move/Resize tool");
		// Scissors
		} else if (key == CutsceneHotkeys.ScissorsTool.key) {
			currentTool = Tool.Scissors;
			EDebug.Log("Cutscene Editor: switching to Scissors tool");
		// Zoom
		} else if (key == CutsceneHotkeys.ZoomTool.key) {
			currentTool = Tool.Zoom;
			EDebug.Log("Cutscene Editor: switching to Zoom tool");
		}

		// Timeline navigation:
		
		// Set in point
		else if (key == CutsceneHotkeys.SetInPont.key) {
			scene.inPoint = scene.playhead;
			EDebug.Log("Cutscene Editor: setting in point");
		// Set out point
		} else if (key == CutsceneHotkeys.SetOutPoint.key) {
			scene.outPoint = scene.playhead;
			EDebug.Log("Cutscene Editor: setting out point");
		// Scrub left
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("left"))) {
			scene.playhead -= CutsceneTimeline.scrubSmallJump;
			EDebug.Log("Cutscene Editor: moving playhead left");
		// Scrub left large
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("#left"))) {
			scene.playhead -= CutsceneTimeline.scrubLargeJump;
			EDebug.Log("Cutscene Editor: moving playhead left");
		// Scrub right
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("right"))) {
			scene.playhead += CutsceneTimeline.scrubSmallJump;
			EDebug.Log("Cutscene Editor: moving playhead right");
		// Scrub right large
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("#right"))) {
			scene.playhead += CutsceneTimeline.scrubLargeJump;
			EDebug.Log("Cutscene Editor: moving playhead right");
		// Go to previous split point
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("up"))) {
			scene.playhead = selectedTrack.GetTimeOfNextSplit(scene.playhead);
			EDebug.Log("Cutscene Editor: moving playhead to previous split point");
		// Go to next split point
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("down"))) {
			scene.playhead = selectedTrack.GetTimeOfPreviousSplit(scene.playhead);
			EDebug.Log("Cutscene Editor: moving playhead to next split point");
		// Go to in point
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("#up"))) {
			scene.playhead = scene.inPoint;
			EDebug.Log("Cutscene Editor: moving playhead to next split point");
		// Go to out point
		} else if (keyDownEvent.Equals(Event.KeyboardEvent("#down"))) {
			scene.playhead = scene.outPoint;
			EDebug.Log("Cutscene Editor: moving playhead to next split point");
		}

		// Track selection:
		
		// Select track 1
		  else if (key == CutsceneHotkeys.SelectTrack1.key) {
			SelectTrackAtIndex(1);
		// Select track 2
		} else if (key == CutsceneHotkeys.SelectTrack2.key) {
			SelectTrackAtIndex(2);
		// Select track 3
		} else if (key == CutsceneHotkeys.SelectTrack3.key) {
			SelectTrackAtIndex(3);
		// Select track 4
		} else if (key == CutsceneHotkeys.SelectTrack4.key) {
			SelectTrackAtIndex(4);
		// Select track 5
		} else if (key == CutsceneHotkeys.SelectTrack5.key) {
			SelectTrackAtIndex(5);
		// Select track 6
		} else if (key == CutsceneHotkeys.SelectTrack6.key) {
			SelectTrackAtIndex(6);
		// Select track 7
		} else if (key == CutsceneHotkeys.SelectTrack7.key) {
			SelectTrackAtIndex(7);
		// Select track 8
		} else if (key == CutsceneHotkeys.SelectTrack8.key) {
			SelectTrackAtIndex(8);
		// Select track 9
		} else if (key == CutsceneHotkeys.SelectTrack9.key) {
			SelectTrackAtIndex(9);
		}
		
		// Other:

		else {
			EDebug.Log("Cutscene Editor: unknown keyboard shortcut " + keyDownEvent);
			return;
		}

		// If we get to this point, a shortcut matching the user's keystroke has been found
		Event.current.Use();
	}

	public static float PaneTabsWidth (int count) {
		return count * 80f;
	}
}