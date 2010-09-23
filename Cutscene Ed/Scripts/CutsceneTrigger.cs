using UnityEngine;
using System.Collections;

public class CutsceneTrigger : MonoBehaviour {
	public Cutscene scene;
	
	void Update () {
		//TEMP
		if (Input.GetKey("p")) {
			scene.PlayCutscene();
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		scene.PlayCutscene();
	}
}
