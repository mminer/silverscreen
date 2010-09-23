using UnityEngine;

public class CutsceneSubtitle : CutsceneMedia {
	public string dialog;
	
	public new string name {
		get {
			int maxTitleLength = 25;
			string _name = dialog;
			if (_name.Length > maxTitleLength) {
				_name = _name.Substring(0, maxTitleLength) + " ...";
			}
			return _name;
		}
	}
}