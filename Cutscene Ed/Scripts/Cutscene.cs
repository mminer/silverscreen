using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Functions to be run when the cutscene starts and finishes
public delegate void CutsceneStart();
public delegate void CutscenePause();
public delegate void CutsceneEnd();

[RequireComponent(typeof(Animation))]
public class Cutscene : MonoBehaviour {
	public float duration  = 30f;
	public float inPoint   = 0f;
	public float outPoint  = 30f;
	public bool stopPlayer = true;
	public GameObject player;

	public GUIStyle subtitleStyle;
	public Rect subtitlePosition = new Rect(0, 0, 400, 200);
	
	public CutsceneStart startFunction { get; set; }
	public CutscenePause pauseFunction { get; set; }
	public CutsceneEnd   endFunction   { get; set; }

	public enum MediaType {
		Shots,
		Actors,
		Audio,
		Subtitles
	}
	public enum EffectType {
		Filters,
		Transitions
	}
	public CutsceneTrack[] tracks {
		get { return GetComponentsInChildren<CutsceneTrack>(); }
	}
	public CutsceneShot[] shots {
		get { return GetComponentsInChildren<CutsceneShot>(); }
	}
	public CutsceneActor[] actors {
		get { return GetComponentsInChildren<CutsceneActor>(); }
	}
	public CutsceneAudio[] audioSources {
		get { return GetComponentsInChildren<CutsceneAudio>(); }
	}
	public CutsceneSubtitle[] subtitles {
		get { return GetComponentsInChildren<CutsceneSubtitle>(); }
	}

	CutsceneSubtitle currentSubtitle;

	public float playhead {
		get { return animation["master"].time; }
		set { animation["master"].time = Mathf.Clamp(value, 0f, duration); }
	}

	[HideInInspector]
	public AnimationClip masterClip;

	void Start () {
		SetupMasterAnimationClip();
		SetupTrackAnimationClips();
		
		DisableCameras();
		DisableAudio();
	}
	
	void OnGUI () {
		/// Displays the current subtitle if there is one
		if (currentSubtitle == null) {
			return;
		}
		
		GUI.BeginGroup(subtitlePosition, subtitleStyle);
			
			GUILayout.Label(currentSubtitle.dialog, subtitleStyle);

		GUI.EndGroup();
	}
	
	/// <summary>
	/// Visually shows the cutscene in the scene view.
	/// </summary>
	void OnDrawGizmos () {
		Gizmos.DrawIcon(transform.position, "Cutscene.png");
	}

	/// <summary>
	/// Sets the in and out points of the master animation clip.
	/// </summary>
	void SetupMasterAnimationClip () {
		animation.RemoveClip("master");

		// Create a new event for when the scene starts
		AnimationEvent start = new AnimationEvent();
		start.time = inPoint;
		start.functionName = "SceneStart";
		masterClip.AddEvent(start);

		// Create a new event for when the scene finishes
		AnimationEvent finish = new AnimationEvent();
		finish.time = outPoint;
		finish.functionName = "SceneFinish";
		masterClip.AddEvent(finish);
		
		animation.AddClip(masterClip, "master");
		animation["master"].time = inPoint;
	}

	/// <summary>
	/// Adds each track's animation clip to the main animation.
	/// </summary>
	void SetupTrackAnimationClips () {
		foreach (CutsceneTrack t in tracks) {
			if (t.enabled) {
				AnimationClip trackAnimationClip = t.track;
				string clipName = "track" + t.id;
				animation.AddClip(trackAnimationClip, clipName);
				animation[clipName].time = inPoint;
			}
		}
	}
	
	/// <summary>
	/// Turns off all child cameras so that they don't display before the cutscene starts.
	/// </summary>
	void DisableCameras () {
		Camera[] childCams = GetComponentsInChildren<Camera>();
		foreach (Camera cam in childCams) {
			cam.enabled = false;
		}
	}
	
	/// <summary>
	/// Turns off all child cameras except for the one specified.
	/// </summary>
	/// <param name="exemptCam">The camera to stay enabled.</param>
	void DisableOtherCameras (Camera exemptCam) {
		Camera[] childCams = GetComponentsInChildren<Camera>();
		foreach (Camera cam in childCams) {
			if (cam != exemptCam) {
				cam.enabled = false;
			}
		}
	}
	
	/// <summary>
	/// Keeps all child audio sources from playing once the game starts.
	/// </summary>
	void DisableAudio () {
		AudioSource[] childAudio = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource audio in childAudio) {
			audio.playOnAwake = false;
		}
	}

	/// <summary>
	/// Starts playing the cutscene.
	/// </summary>
	public void PlayCutscene () {
		// Set up and play the master animation
		animation["master"].layer = 0;
		animation.Play("master");

		// Set up and play each individual track
		for (int i = 0; i < tracks.Length; i++) {
			if (tracks[i].enabled) {
				animation["track" + tracks[i].id].layer = i + 1;
				animation.Play("track" + tracks[i].id);
			}
		}
	}
	
	/// <summary>
	/// Pauses the cutscene.
	/// </summary>
	public void PauseCutscene () {
		pauseFunction();
		// TODO actually pause the cutscene
	}

	/// <summary>
	/// Called when the scene starts.
	/// </summary>
	void SceneStart () {
		if (startFunction != null) {
			startFunction();
		}

		// Stop the player from being able to move
		if (player != null && stopPlayer) {
			EDebug.Log("Cutscene: deactivating player");
			player.active = false;
		}
		
		DisableCameras();
		currentSubtitle = null;

		EDebug.Log("Cutscene: scene started at " + animation["master"].time);
	}
	
	/// <summary>
	/// Called when the scene ends.
	/// </summary>
	void SceneFinish () {
		if (endFunction != null) {
			endFunction();
		}

		// Allow the player to move again
		if (player != null) {
			EDebug.Log("Cutscene: activating player");
			player.active = true;
		}
		
		EDebug.Log("Cutscene: scene finished at " + animation["master"].time);
	}
	
	/// <summary>
	/// Shows the specified shot.
	/// </summary>
	/// <param name="clip">The shot to show.</summary>
	void PlayShot (CutsceneClip clip) {
		Camera cam = ((CutsceneShot)clip.master).camera;
		cam.enabled = true;
		
		StartCoroutine(StopShot(cam, clip.duration));

		EDebug.Log("Cutscene: showing camera " + clip.name + " at " + animation["master"].time);
	}
	
	/// <summary>
	/// Stops the shot from playing at its out point.
	/// </summary>
	/// <param name="shot">The shot to stop.</param>
	/// <param name="duration">The time at which to stop the shot.</param>
	IEnumerator StopShot (Camera cam, float duration) {
		yield return new WaitForSeconds(duration);
		cam.enabled = false;
		EDebug.Log("Cutscene: stopping shot at " + animation["master"].time);
	}
	
	/// <summary>
	/// Plays the specified actor.
	/// </summary>
	/// <param name="clip">The actor to play.</summary>
	void PlayActor (CutsceneClip clip) {
		CutsceneActor actor = ((CutsceneActor)clip.master);
		AnimationClip anim = actor.anim;
		GameObject go = ((CutsceneActor)clip.master).go;
		
		go.animation[anim.name].time = clip.inPoint;

		go.animation.Play(anim.name);
		StartCoroutine(StopActor(actor, clip.duration));
		
		EDebug.Log("Cutscene: showing actor " + clip.name + " at " + animation["master"].time);
	}
	
	/// <summary>
	/// Stops the actor from playing at its out point.
	/// </summary>
	/// <param name="actor">The actor to stop.</param>
	/// <param name="duration">The time at which to stop the actor.</param>
	IEnumerator StopActor (CutsceneActor actor, float duration) {
		yield return new WaitForSeconds(duration);
		actor.go.animation.Stop(actor.anim.name);
		EDebug.Log("Cutscene: stopping actor at " + animation["master"].time);
	}
	
	/// <summary>
	/// Plays the specified audio.
	/// </summary>
	/// <param name="clip">The audio to play.</summary>
	void PlayAudio (CutsceneClip clip) {
		AudioSource aud = ((CutsceneAudio)clip.master).audio;
		aud.Play();
		aud.time = clip.inPoint; // Set the point at which the clip plays
		StartCoroutine(StopAudio(aud, clip.duration)); // Set the point at which the clip stops

		EDebug.Log("Playing audio " + clip.name + " at " + animation["master"].time);
	}
	
	/// <summary>
	/// Stops the audio from playing at its out point.
	/// </summary>
	/// <param name="aud">The audio source to stop.</param>
	/// <param name="duration">The time at which to stop the audio.</param>
	IEnumerator StopAudio (AudioSource aud, float duration) {
		yield return new WaitForSeconds(duration);
		aud.Stop();
	}
	
	/// <summary>
	/// Displays the specified subtitle.
	/// </summary>
	/// <param name="clip">The subtitle to display.</summary>
	void PlaySubtitle (CutsceneClip clip) {
		currentSubtitle = (CutsceneSubtitle)clip.master;
		EDebug.Log("Displaying subtitle " + clip.name + " at " + animation["master"].time);
		
		StartCoroutine(StopSubtitle(clip.duration));
	}
	
	/// <summary>
	/// Stops the subtitle from displaying at its out point.
	/// </summary>
	/// <param name="duration">The time at which to stop the audio.</param>
	IEnumerator StopSubtitle (float duration) {
		yield return new WaitForSeconds(duration);
		currentSubtitle = null;
	}
	
	/// <summary>
	/// Stops all subtitles from displaying by setting the current subtitle to null.
	/// </summary>
	void StopSubtitle () {
		currentSubtitle = null;
	}
	
	/// <summary>
	/// Called when the clip type is unknown.
	/// </summary>
	/// <remarks>For debugging only; ideally this will never be called.</remarks>
	void UnknownFunction (CutsceneClip clip) {
		EDebug.Log("Cutscene: unknown function call from clip " + clip.name);
	}

	/// <summary>
	/// Creates a new CutsceneShot object and attaches it to a new game object as a child of the Shots game object.
	/// </summary>
	/// <returns>The new CutsceneShot object.</returns>
	public CutsceneShot NewShot () {
		GameObject shot = new GameObject("Camera", typeof(Camera), typeof(CutsceneShot));
		// Keep the camera from displaying before it's placed on the timeline
		shot.camera.enabled = false;
		// Set the parent of the new shot to the Shots object
		shot.transform.parent = transform.Find("Shots");

		EDebug.Log("Cutscene Editor: added new shot");
		return shot.GetComponent<CutsceneShot>();
	}

	/// <summary>
	/// Creates a new CutsceneActor object and attaches it to a new game object as a child of the Shots game object.
	/// </summary>
	/// <returns>The new CutsceneActor object.</returns>
	public CutsceneActor NewActor (AnimationClip anim, GameObject go) {
		CutsceneActor actor = GameObject.Find("Actors").AddComponent<CutsceneActor>();
		actor.name = anim.name;
		actor.anim = anim;
		actor.go = go;

		EDebug.Log("Cutscene Editor: adding new actor");
		return actor;
	}

	/// <summary>
	/// Creates a new CutsceneAudio object and attaches it to a new game object as a child of the Audio game object.
	/// </summary>
	/// <param name="clip">The audio clip to be attached the CutsceneAudio object.</param>
	/// <returns>The new CutsceneAudio object.</returns>
	public CutsceneAudio NewAudio (AudioClip clip) {
		GameObject aud = new GameObject(clip.name, typeof(AudioSource), typeof(CutsceneAudio));
		aud.audio.clip = clip;
		// Keep the audio from playing when the game starts
		aud.audio.playOnAwake = false;
		// Set the parent of the new audio to the "Audio" object
		aud.transform.parent = transform.Find("Audio");

		EDebug.Log("Cutscene Editor: added new audio");
		return aud.GetComponent<CutsceneAudio>();
	}

	/// <summary>
	/// Creates a new CutsceneSubtitle object and attaches it to the Subtitles game object.
	/// </summary>
	/// <param name="dialog">The dialog to be displayed.</param>
	/// <returns>The new CutsceneSubtitle object.</returns>
	public CutsceneSubtitle NewSubtitle (string dialog) {
		CutsceneSubtitle subtitle = GameObject.Find("Subtitles").AddComponent<CutsceneSubtitle>();
		subtitle.dialog = dialog;

		EDebug.Log("Cutscene Editor: added new subtitle");
		return subtitle;
	}

	/// <summary>
	/// Attaches a new track component to the cutscene.
	/// </summary>
	/// <returns>The new cutscene track.</returns>
	public CutsceneTrack AddTrack (Cutscene.MediaType type) {
		int id = 0;
		// Ensure the new track has a unique ID
		foreach (CutsceneTrack t in tracks) {
			if (id == t.id) {
				id++;
			} else {
				break;
			}
		}
		
		CutsceneTrack track = gameObject.AddComponent<CutsceneTrack>();
		track.id   = id;
		track.type = type;
		track.name = CutsceneTrack.DefaultName(type);
		
		EDebug.Log("Cutscene Editor: added new track of type " + type);
		return track;
	}
}