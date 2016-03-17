using UnityEngine;
using System.Collections;

public class GUI_Button : MonoBehaviour {

	public Texture2D buttonIcon = null;

	void OnGUI() {

		if (GUI.Button (new Rect (50, 15, 100, 100), buttonIcon)) {
			
		}
	}
}
