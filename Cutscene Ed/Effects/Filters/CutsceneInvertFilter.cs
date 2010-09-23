using UnityEngine;

// Based off of InvertCamera.js by Joachim Ante
// Retrieved from the Unify Community wiki (http://www.unifycommunity.com/wiki/index.php?title=InvertCamera)

public class CutsceneInvertFilter : CutsceneEffect {
	[HideInInspector]
	public static new string name = "Invert";

	void OnPreCull () {
		camera.ResetWorldToCameraMatrix();
		camera.ResetProjectionMatrix();
		camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1));
	}

	void OnPreRender () {
		GL.SetRevertBackfacing(true);
	}

	void OnPostRender () {
		GL.SetRevertBackfacing(false);
	}
}
