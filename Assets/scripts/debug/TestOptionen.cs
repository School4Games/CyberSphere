using UnityEngine;
using System.Collections;

public class TestOptionen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	bool isFullscreen;
	bool guiEnable = false;

	// Update is called once per frame
	void Update () {
		CheckFullscreen ();
		EscapeToMenu ();
	}
	
	void EscapeToMenu () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (guiEnable == false){
				guiEnable = true;
			}
			else {
				guiEnable = false;
			}
		}
	}

	void CheckFullscreen () {
		if (Screen.fullScreen == true) {
			isFullscreen = true;
				}
		if (Screen.fullScreen == false) {
			isFullscreen = false;
				}
		}

	public float hSliderValue = 60.0F;
	int TFrameInt = 60;

	void OnGUI() { // Buttons for Resoltion Options / Values (new SHAPE (Left, Top, Width, Heigth), Text)
		if (guiEnable == true) {
						hSliderValue = GUI.HorizontalSlider (new Rect (10, 300, 100, 30), hSliderValue, 10.0F, 300.0F);
						int TFrameInt = Mathf.RoundToInt (hSliderValue);
						var TFrame = TFrameInt.ToString ();
						GUI.Label (new Rect (130, 300, 100, 20), TFrame);
						var abc = isFullscreen.ToString ();
						GUI.Label (new Rect (170, 120, 100, 50), ("Fullscreen: " + abc));



						if (GUI.Button (new Rect (10, 10, 150, 50), "1280x720 / 60Hz")) { // ResoBtn1
								Debug.Log ("720p Windowed 144Hz"); 
								Reso1 (); // Call function Reso1
						}

						if (GUI.Button (new Rect (10, 70, 150, 50), "1920x1080 / 60Hz")) { // ResoBtn2
								Debug.Log ("1080p Windowed 144Hz");
								Reso2 (); // Call function Reso2
						}
						if (GUI.Button (new Rect (10, 130, 150, 50), "1600x900 / 60Hz")) { // ResoBtn3
								Debug.Log ("900 Windowed 144Hz");
								Reso3 (); // Call function Reso3
						}
						if (GUI.Button (new Rect (10, 200, 150, 50), "Set Framerate to Slider")) {
								TFramerate ();
						}
						if (GUI.Button (new Rect (170, 10, 100, 50), "VSync On")) {
								VSync1 ();
						}
						if (GUI.Button (new Rect (280, 10, 100, 50), "VSync Off")) {
								VSync0 ();
						}
						if (GUI.Button (new Rect (170, 70, 100, 50), "Fullscreen")) {
								FScreen ();
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

	void TFramerate () {
		Application.targetFrameRate = TFrameInt;
		}

	void VSync1 () {
		QualitySettings.vSyncCount = 2;
	}

	void VSync0 () {
		QualitySettings.vSyncCount = 0;
		}
	void FScreen() {
		Screen.fullScreen = !Screen.fullScreen;
		}


}
