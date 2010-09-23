using UnityEngine;
using UnityEditor;

class CutscenePreview : ICutsceneGUI {
	//readonly CutsceneEditor ed;

	public CutscenePreview (CutsceneEditor ed) {
		//this.ed = ed;
	}

	public void OnGUI (Rect rect) {
		//GUI.DrawTexture(rect, );
		Camera cam = GameObject.Find("Some Cam").GetComponent<Camera>();
		EDebug.Log(cam.name);

		cam.targetTexture = new RenderTexture(128, 128, 32);
		cam.targetTexture.isPowerOfTwo = true;
		cam.targetTexture.Create();
		
		

		GUI.DrawTexture(rect, cam.targetTexture);

		cam.targetTexture.Release();
		cam.targetTexture = null;

		/*if (Event.current.type == EventType.repaint) {
			MovieTexture target = base.target as MovieTexture;
			float num = Mathf.Min(Mathf.Min((float)(r.width / ((float)target.width)), (float)(r.height / ((float)target.height))), 1f);
			Rect viewRect = new Rect(r.x, r.y, target.width * num, target.height * num);
			PreviewGUI.BeginScrollView(r, this.m_Pos, viewRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
			GUI.DrawTexture(viewRect, target, ScaleMode.StretchToFill, false);
			this.m_Pos = PreviewGUI.EndScrollView();
			if (target.isPlaying) {
				GUIView.current.Repaint();
			}
			if (Application.isPlaying) {
				if (target.isPlaying) {
					Rect position = new Rect(r.x, r.y + 10f, r.width, 20f);
					EditorGUI.DropShadowLabel(position, "Can't pause preview when in play mode");
				} else {
					Rect rect3 = new Rect(r.x, r.y + 10f, r.width, 20f);
					EditorGUI.DropShadowLabel(rect3, "Can't start preview when in play mode");
				}
			}
		}*/
	}
}