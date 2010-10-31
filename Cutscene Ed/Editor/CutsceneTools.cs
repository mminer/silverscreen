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

using UnityEditor;
using UnityEngine;

public enum Tool {
	MoveResize,
	Scissors,
	Zoom
}

class CutsceneTools : ICutsceneGUI
{
	readonly CutsceneEditor ed;

	readonly GUIContent[] tools = {
		new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/tool_move.png")     as Texture, "Move/Resize"),
		new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/tool_scissors.png") as Texture, "Scissors"),
		new GUIContent(EditorGUIUtility.LoadRequired("Cutscene Ed/tool_zoom.png")     as Texture, "Zoom")
	};

	public CutsceneTools (CutsceneEditor ed)
	{
		this.ed = ed;
	}

	public void OnGUI (Rect rect)
	{
		GUILayout.BeginArea(rect);
		
		EditorGUILayout.BeginHorizontal(ed.style.GetStyle("Tools Bar"), GUILayout.Width(rect.width));

		GUILayout.FlexibleSpace();
		ed.currentTool = (Tool)GUILayout.Toolbar((int)ed.currentTool, tools);

		EditorGUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}