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
using System.Collections.Generic;
using System;

public class CutsceneTrack : MonoBehaviour
{
	public bool locked;
	[HideInInspector]
	public Cutscene.MediaType type = Cutscene.MediaType.Shots;
	public new string name = DefaultName(Cutscene.MediaType.Shots);
	public List<CutsceneClip> clips = new List<CutsceneClip>();
	[HideInInspector]
	public int id = 0;
	
	public AnimationClip track {
		get {
			AnimationClip _track = new AnimationClip();
			
			foreach (CutsceneClip clip in clips) {
				AnimationEvent start = new AnimationEvent();
				start.time = clip.timelineStart;
				start.functionName = clip.startFunction;
				start.objectReferenceParameter = clip;
				_track.AddEvent(start);
			}
			
			return _track;
		}
	}

	public static string DefaultName (Cutscene.MediaType type)
	{
		switch (type) {
			case Cutscene.MediaType.Shots:
				return "Shots";
			case Cutscene.MediaType.Actors:
				return "Actors";
			case Cutscene.MediaType.Audio:
				return "Audio";
			default: // Cutscene.MediaType.Subtitles
				return "Subtitles";
		}
	}

	/// <summary>
	/// Checks to see if there's a clip at the given time, ignoring the given clip.
	/// </summary>
	/// <param name="time">The time to check for.</param>
	/// <returns>The CutsceneClip that is at the given time.</returns>
	public CutsceneClip ContainsClipAtTime (float time, CutsceneClip ignoreClip)
	{
		CutsceneClip contains = ContainsClipAtTime(time);

		if (contains != null && contains != ignoreClip) {
			return contains;
		} else {
			return null;
		}
	}

	/// <summary>
	/// Checks to see if there's a clip at the given time.
	/// </summary>
	/// <param name="time">The time to check for.</param>
	/// <returns>The CutsceneClip that is at the given time.</returns>
	public CutsceneClip ContainsClipAtTime (float time)
	{
		foreach (CutsceneClip clip in clips) {
			if (time >= clip.timelineStart && time <= clip.timelineStart + clip.duration) {
				return clip;
			}
		}

		// The timeline doesn't contain a clip at the specified time
		return null;
	}

	public float GetTimeOfPreviousSplit (float time)
	{
		float splitTime = -1f;

		foreach (CutsceneClip clip in clips) {
			if (clip.timelineEnd < time && clip.timelineEnd > splitTime) {
				splitTime = clip.timelineEnd;
			} else if (clip.timelineStart < time && clip.timelineStart > splitTime) {
				splitTime = clip.timelineStart;
			}
		}

		// If splitTime is still -1, just return the original time
		return splitTime == -1f ? time : splitTime;
	}
	
	public float GetTimeOfNextSplit (float time)
	{
		float splitTime = Mathf.Infinity;

		foreach (CutsceneClip clip in clips) {
			 if (clip.timelineStart > time && clip.timelineStart < splitTime) {
				splitTime = clip.timelineStart;
			} else if (clip.timelineEnd > time && clip.timelineEnd < splitTime) {
				splitTime = clip.timelineEnd;
			}
		}
		
		// If splitTime is still infinity, just return the original time
		return splitTime == Mathf.Infinity ? time : splitTime;
	}
}