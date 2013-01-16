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

/// <summary>
/// A simple class for toggle-able debug messages. Debug messages for editor tools are useful when developing the tool itself, but annoying for end users.
/// </summary>
public class EDebug
{
	static bool debug = false;

	public static void Break ()
	{
		if (debug) Debug.Break();
	}

	public static void Log (object message)
	{
		if (debug) Debug.Log(message);
	}

	public static void Log (object message, Object context)
	{
		if (debug) Debug.Log(message, context);
	}

	public static void LogError (object message)
	{
		if (debug) Debug.LogError(message);
	}

	public static void LogError (object message, Object context)
	{
		if (debug) Debug.LogError(message, context);
	}

	public static void LogWarning (object message)
	{
		if (debug) Debug.LogWarning(message);
	}

	public static void LogWarning (object message, Object context)
	{
		if (debug) Debug.LogWarning(message, context);
	}
}