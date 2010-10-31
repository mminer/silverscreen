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
using System;

public class CutsceneClip : ScriptableObject
{
	public CutsceneMedia master;

	public float timelineStart = 0f;
	public float inPoint       = 0f;
	public float outPoint      = 5f; // Default 5 seconds

	public float timelineEnd {
		get { return timelineStart + duration; }
	}

	float maxOutPoint {
		get {
			float max = Mathf.Infinity;

			if (master is CutsceneActor) {
				max = ((CutsceneActor)master).anim.length;
			} else if (master is CutsceneAudio) {
				max = ((CutsceneAudio)master).gameObject.audio.clip.length;
			}

			return max;
		}
	}

	public void SetTimelineStart (float value)
	{
		timelineStart = Mathf.Clamp(value, 0f, Mathf.Infinity);
	}

	public void SetInPoint (float value)
	{
		inPoint = Mathf.Clamp(value, 0f, outPoint);
	}

	public void SetOutPoint (float value)
	{
		outPoint = Mathf.Clamp(value, inPoint, maxOutPoint);
	}

	[HideInInspector]
	public bool setToDelete = false; // An ugly workaround used for deleting clips

	// Read only
	public float duration {
		get { return outPoint - inPoint; }
	}
	public string startFunction {
		get {
			// Determine which function to call based on the master object type
			if (master is CutsceneShot) {
				return "PlayShot";
			} else if (master is CutsceneActor) {
				return "PlayActor";
			} else if (master is CutsceneAudio) {
				return "PlayAudio";
			} else if (master is CutsceneSubtitle) {
				return "PlaySubtitle";
			} else {
				return "UnknownFunction";
			}
		}
	}
	public Cutscene.MediaType type {
		get {
			if (master is CutsceneShot) {
				return Cutscene.MediaType.Shots;
			} else if (master is CutsceneActor) {
				return Cutscene.MediaType.Actors;
			} else if (master is CutsceneAudio) {
				return Cutscene.MediaType.Audio;
			} else { // master is CutsceneSubtitles
				return Cutscene.MediaType.Subtitles;
			}
		}
	}
	
	public CutsceneClip (CutsceneMedia master)
	{
		this.master = master;
		if (master is CutsceneSubtitle) {
			name = ((CutsceneSubtitle)master).dialog;
		} else {
			name = master.name;
		}

		if (maxOutPoint != Mathf.Infinity) {
			outPoint = maxOutPoint;
		}
	}
	
	/// <summary>
	/// Gets a clone of the current clip.
	/// </summary>
	/// <returns>The clip copy.</returns>
	public CutsceneClip GetCopy ()
	{
		CutsceneClip copy = new CutsceneClip(master);
		copy.timelineStart = timelineStart;
		copy.SetInPoint(inPoint);
		copy.outPoint = outPoint;
		
		return copy;
	}

	/// <summary>
	/// Adds an effect to the clip.
	/// </summary>
	/// <param name="effect">The transitions or filter.</param>
	public void ApplyEffect (Type effect)
	{
		// Only add the effect if the master object is a camera shot
		if (master is CutsceneShot) {
			master.gameObject.AddComponent(effect);
		}
	}
}