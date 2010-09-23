using UnityEngine;

/// <summary>
/// A simple class for toggle-able debug messages. Debug messages for editor tools are useful when developing the tool itself, but annoying for end users.
/// </summary>
public class EDebug {
	static bool debug = false;

	public static void Break () {
		if (debug) Debug.Break();
	}

	public static void Log (object message) {
		if (debug) Debug.Log(message);
	}

	public static void Log (object message, UnityEngine.Object context) {
		if (debug) Debug.Log(message, context);
	}

	public static void LogError (object message) {
		if (debug) Debug.LogError(message);
	}

	public static void LogError (object message, UnityEngine.Object context) {
		if (debug) Debug.LogError(message, context);
	}

	public static void LogWarning (object message) {
		if (debug) Debug.LogWarning(message);
	}

	public static void LogWarning (object message, UnityEngine.Object context) {
		if (debug) Debug.LogWarning(message, context);
	}
}