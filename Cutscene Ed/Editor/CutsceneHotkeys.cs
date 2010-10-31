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

struct Hotkey {
	const string prefPrefix = "Cutscene Editor Hotkey ";

	string  id;
	KeyCode defaultKey;
	bool    assignable;

	public KeyCode key {
		get {
			if (assignable && EditorPrefs.HasKey(prefPrefix + id)) {
				return (KeyCode)EditorPrefs.GetInt(prefPrefix + id, (int)defaultKey);
			} else {
				return defaultKey;
			}
		}
		set {
			if (assignable) {
				EditorPrefs.SetInt(prefPrefix + id, (int)value);
			} else {
				EDebug.LogWarning("Cutscene Editor: Hotkey " + id + " cannot be reassigned");
			}
		}
	}
	
	public Hotkey (string id, KeyCode defaultKey, bool assignable) {
		this.id         = id;
		this.defaultKey = defaultKey;
		this.assignable = assignable;
	}

	/// <summary>
	/// Resets the key to its default value.
	/// </summary>
	public void Reset () {
		if (assignable) {
			key = defaultKey;
		}
		EDebug.Log("Cutscene Editor: Hotkey " + id + " reset to its default " + defaultKey);
	}
}

static class CutsceneHotkeys {
	// Assignable:
	public static Hotkey MoveResizeTool = new Hotkey("moveResizeTool", KeyCode.M, true);
	public static Hotkey ScissorsTool   = new Hotkey("scissorsTool",   KeyCode.S, true);
	public static Hotkey ZoomTool       = new Hotkey("zoomTool",       KeyCode.Z, true);
	public static Hotkey SetInPont      = new Hotkey("setInPoint",     KeyCode.I, true);
	public static Hotkey SetOutPoint    = new Hotkey("setOutPoint",    KeyCode.O, true);

	// Unassignable:
	public static readonly Hotkey SelectTrack1 = new Hotkey("selectTrack1", KeyCode.Alpha1, true);
	public static readonly Hotkey SelectTrack2 = new Hotkey("selectTrack2", KeyCode.Alpha2, true);
	public static readonly Hotkey SelectTrack3 = new Hotkey("selectTrack3", KeyCode.Alpha3, true);
	public static readonly Hotkey SelectTrack4 = new Hotkey("selectTrack4", KeyCode.Alpha4, true);
	public static readonly Hotkey SelectTrack5 = new Hotkey("selectTrack5", KeyCode.Alpha5, true);
	public static readonly Hotkey SelectTrack6 = new Hotkey("selectTrack6", KeyCode.Alpha6, true);
	public static readonly Hotkey SelectTrack7 = new Hotkey("selectTrack7", KeyCode.Alpha7, true);
	public static readonly Hotkey SelectTrack8 = new Hotkey("selectTrack8", KeyCode.Alpha8, true);
	public static readonly Hotkey SelectTrack9 = new Hotkey("selectTrack9", KeyCode.Alpha9, true);

	public static Hotkey[] assignable = new Hotkey[] {
		MoveResizeTool,
		ScissorsTool,
		ZoomTool,
		SetInPont,
		SetOutPoint
	};

	/// <summary>
	/// Resets all the assignable hotkeys to their default values.
	/// </summary>
	public static void ResetAll () {
		foreach (Hotkey key in assignable) {
			key.Reset();
		}
	}
}