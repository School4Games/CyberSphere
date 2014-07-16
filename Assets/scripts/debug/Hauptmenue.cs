using UnityEngine;
using System.Collections;

public class Hauptmenue : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		guiEnable = true;
	}
	
	bool isFullscreen;
	bool guiEnable = false;
	bool videoEnable = false;
	bool soundEnable = false;
	bool levelEnable = false;
	
	// Update is called once per frame
	void Update () {
		CheckFullscreen ();
		EscapeToMenu ();
	}
	
	void EscapeToMenu () {
		//		if (Input.GetKeyDown (KeyCode.Escape)) {
		//			if (guiEnable == false){
		//				guiEnable = true;
		//			}
		//			else {
		//				guiEnable = false;
		//			}
		//		}
	}
	
	void CheckFullscreen () {
		if (Screen.fullScreen == true) {
			isFullscreen = true;
		}
		if (Screen.fullScreen == false) {
			isFullscreen = false;
		}
	}
	
	void OnGUI() { // Buttons for Resolution Options / Values (new SHAPE (Left, Top, Width, Heigth), Text)
		if (guiEnable == false) {
			videoEnable = false;
			soundEnable = false;
			levelEnable = false;
		}
		
		if (guiEnable == true) {
			if (GUI.Button (new Rect (10, 70, 100, 50), "Video")) { 
				videoEnable = true;
				levelEnable = false;
			}
			if (videoEnable == true) {
				soundEnable = false;
				levelEnable = false;
			}
			if (GUI.Button (new Rect (10, 130, 100, 50), "Sound")) {
				soundEnable = true;
			}
			if (soundEnable == true) {
				videoEnable = false;
				levelEnable = false;
			}
			if (GUI.Button (new Rect (10, 10, 100, 50), "Level")) {
				levelEnable = true;
			}
			if (levelEnable == true) {
				soundEnable = false;
				videoEnable = false;
			}
		}
		if (levelEnable == true) {
			if (GUI.Button (new Rect (120, 10, 100, 50), "Level 1")) {
				Application.LoadLevel(1);
			}
			if (GUI.Button (new Rect (120, 70, 100, 50), "Level 2")) {
				Application.LoadLevel(2);
			}
			if (GUI.Button (new Rect (120, 130, 100, 50), "Level 3")) {
				Application.LoadLevel(3);
			}
		}
		
		
		if (videoEnable == true) {
			if (GUI.Button (new Rect (120, 10, 150, 50), "1280x720 / 60Hz")) { // ResoBtn1
				Reso1 (); // Call function Reso1
			}
			if (GUI.Button (new Rect (120, 70, 150, 50), "1920x1080 / 60Hz")) { // ResoBtn2
				Reso2 (); // Call function Reso2
			}
			if (GUI.Button (new Rect (120, 130, 150, 50), "1600x900 / 60Hz")) { // ResoBtn3
				Reso3 (); // Call function Reso3
			}
			if (GUI.Button (new Rect (280, 10, 100, 50), "VSync On")) {
				VSync1 ();
			}
			if (GUI.Button (new Rect (390, 10, 100, 50), "VSync Off")) {
				VSync0 ();
			}
			if (GUI.Button (new Rect (280, 70, 100, 50), "Fullscreen")) {
				FScreen ();
			}
			
			GUI.Label (new Rect (280, 120, 100, 50), ("Fullscreen: " + isFullscreen));
		}
		
		if (soundEnable == true) {
			if (GUI.Button (new Rect (120, 10, 150, 50), "Mute ALL")) { // ResoBtn1
				Reso1 (); // Call function Reso1
			}
		}
	}
	
	void Reso1() {
		Screen.SetResolution (1280, 720, true, 60);
	}
	
	void Reso2() {
		Screen.SetResolution (1920, 1080, true, 60);
	}
	void Reso3() {
		Screen.SetResolution (1600, 900, true, 60);
	}
	
	void VSync1 () {
		QualitySettings.vSyncCount = 1;
	}
	void VSync0 () {
		QualitySettings.vSyncCount = 0;
	}
	void FScreen() {
		Screen.fullScreen = !Screen.fullScreen;
	}
	
	
}
