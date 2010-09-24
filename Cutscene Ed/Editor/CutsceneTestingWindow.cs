using UnityEngine;
using UnityEditor;
using System.Collections;

class CutsceneTestingWindow : EditorWindow {
	bool allowHorizontal = true;
	bool allowVertical = false;
	
	Rect[] boxes = {
		new Rect(0, 0, 100, 50),
		new Rect(100, 0, 100, 50),
		new Rect(300, 0, 100, 50)
	};
	
	static Rect noBox = new Rect(0, 0, Mathf.Infinity, Mathf.Infinity);
	
	const int NONE = -1;
	
	int dragBox = NONE;
	

	
	string dragEvent = "";
	
	public CutsceneTestingWindow () {
		title = "Testing Window";
	}
	
	[MenuItem ("Window/Testing Window")]
	public static void OpenEditor () {
		EditorWindow.GetWindow<CutsceneTestingWindow>().Show();
	}

	Rect stuff	= new Rect(10, 5, 1000, 500);
	Rect more	= new Rect(10, 10, 100, 100);
	Vector2 scrollPos;

	public void OnGUI () {


		/*scrollPos = GUI.BeginScrollView(more, scrollPos, stuff, true, true);
			GUI.Button(new Rect(0, 0, 100, 20), "Hello");

		GUI.EndScrollView();*/

		
		for (int i = 0; i < boxes.Length; i++) {
			EnforceBoundaries(ref boxes[i]);
			GUI.Box(boxes[i], "Box " + i);
		}

		for (int i = boxes.Length - 1; i >= 0; i--) {
			Rect dragRect2 = new Rect(boxes[i].x + 5, boxes[i].y, boxes[i].width - 10, boxes[i].height);
			EditorGUIUtility.AddCursorRect(dragRect2, MouseCursor.SlideArrow);

			Rect resizeLeftRect2 = new Rect(boxes[i].x, boxes[i].y, 5, boxes[i].height);
			EditorGUIUtility.AddCursorRect(resizeLeftRect2, MouseCursor.ResizeHorizontal);

			Rect resizeRightRect2 = new Rect(boxes[i].xMax - 5, boxes[i].y, 5, boxes[i].height);
			EditorGUIUtility.AddCursorRect(resizeRightRect2, MouseCursor.ResizeHorizontal);
		}
		
		if (Event.current.type == EventType.MouseDown) {
			// Go in reverse order so that we drag the topmost box
			for (int i = boxes.Length - 1; i >= 0; i--) {
				
				Rect dragRect = new Rect(boxes[i].x + 5, boxes[i].y, boxes[i].width - 10, boxes[i].height);

				Rect resizeLeftRect = new Rect(boxes[i].x, boxes[i].y, 5, boxes[i].height);
				
				Rect resizeRightRect = new Rect(boxes[i].xMax - 5, boxes[i].y, 5, boxes[i].height);
				
				if (dragRect.Contains(Event.current.mousePosition)) {
					dragBox = i;
					dragEvent = "move";
					Debug.Log("Starting drag");
					break;
				}
				
				if (resizeLeftRect.Contains(Event.current.mousePosition)) {
					dragBox = i;
					dragEvent = "resizeLeft";
					Debug.Log("Starting resize left");
					break;
				}
				
				if (resizeRightRect.Contains(Event.current.mousePosition)) {
					dragBox = i;
					dragEvent = "resizeRight";
					Debug.Log("Starting resize right");
					break;
				}
			}
		} else if (Event.current.type == EventType.MouseDrag && dragBox != NONE) {
			if (dragEvent == "resizeLeft") {
				boxes[dragBox].xMin += Event.current.delta.x;
			}
			
			if (dragEvent == "resizeRight") {
				//if (!DetectOverlap()) {
					boxes[dragBox].xMax += Event.current.delta.x;
				//}
			}
			
			
			if (allowHorizontal && dragEvent == "move") {
				boxes[dragBox].x += Event.current.delta.x;
			}
			
			for (int i = 0; i < boxes.Length; i++) {
				for (int j = 0; j < boxes.Length; j++) {
					// If first box is pushed into the second one
					if (boxes[i] != boxes[j] && boxes[j].Contains(new Vector2(boxes[i].xMax, boxes[i].y))) {
							
						boxes[j].x = boxes[i].xMax;
						
						// If boundaries had to be enforced, move the box back
						if (EnforceBoundaries(ref boxes[i])) {
							boxes[i].x = boxes[j].x - boxes[i].width;
						}
					}
				}
			}
			
			if (allowVertical) {
				boxes[dragBox].y += Event.current.delta.y;
			}
		} else if (Event.current.type == EventType.MouseUp) {
			dragBox = NONE;
			dragEvent = "";
			Debug.Log("Stopping drag");
		}
		
		Event.current.Use();
	}
	
	bool EnforceBoundaries (ref Rect box) {
		if (box.x < 0) { // Left
			box.x = 0;
		} else if (box.xMax > position.width) { // Right
			box.x = position.width - box.width;
		} else if (box.y < 0) { // Top
			box.y = 0;
		} else if (box.yMax > position.height) { // Bottom
			box.y = position.height - box.height;
		} else {
			// No boundaries were hit, so return false;
			return false;
		}
		
		return true;
	}
	
	bool DetectOverlap () {
		for (int i = 1; i < boxes.Length; i++) {
			
			if (boxes[i] != boxes[dragBox] && boxes[i].x > boxes[i - 1].xMax) {
				return false;
			}
		}
		/*
		foreach (Rect box in boxes) {
			if (box != boxes[dragBox] && box.Contains(Event.current.mousePosition)) {
				Debug.Log("Overlap detected");
				return true;
			}
		}*/
		
		// If we get to this point, no overlap has been detected
		return true;
	}
}
