using UnityEngine;

public abstract class CutsceneMedia : MonoBehaviour {
	public Cutscene.MediaType type {
		get {
			if (this.GetType() == typeof(CutsceneShot)) {
				return Cutscene.MediaType.Shots;
			} else if (this.GetType() == typeof(CutsceneActor)) {
				return Cutscene.MediaType.Actors;
			} else if (this.GetType() == typeof(CutsceneAudio)) {
				return Cutscene.MediaType.Audio;
			} else { // obj.GetType() == typeof(CutsceneSubtitle)
				return Cutscene.MediaType.Subtitles;
			}
		}
	}
}