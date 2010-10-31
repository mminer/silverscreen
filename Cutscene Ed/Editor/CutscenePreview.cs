/**
 * Copyright (c) 2010 Matthew Miner
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEditor;

class CutscenePreview : ICutsceneGUI
{
	//readonly CutsceneEditor ed;

	public CutscenePreview (CutsceneEditor ed)
	{
		//this.ed = ed;
	}

	public void OnGUI (Rect rect)
	{
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