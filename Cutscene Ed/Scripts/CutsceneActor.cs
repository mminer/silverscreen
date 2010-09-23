using UnityEngine;

public class CutsceneActor : CutsceneMedia {
	public AnimationClip anim;
	public GameObject go;

	public new string name {
		get { return anim.name; }
	}
}