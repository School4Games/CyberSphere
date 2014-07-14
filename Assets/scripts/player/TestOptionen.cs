using UnityEngine;
using System.Collections;

public class TestOptionen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	public float hSliderValue = 60.0F;
	int TFrameInt = 60;

	void OnGUI() { // Buttons for Resoltion Options / Values (new SHAPE (Left, Top, Width, Heigth), Text)

		hSliderValue = GUI.HorizontalSlider(new Rect(10, 300, 100, 30), hSliderValue, 10.0F, 300.0F);
		int TFrameInt = Mathf.RoundToInt (hSliderValue);
		var TFrame = TFrameInt.ToString ();
		GUI.Label (new Rect (130, 300, 100, 20), TFrame);



		if (GUI.Button (new Rect (10, 10, 150, 50), "1280x720 / 60Hz")) { // ResoBtn1
			Debug.Log ("720p Windowed 144Hz"); 
			Reso1 (); // Call function Reso1
		}

		if (GUI.Button (new Rect (10, 70, 150, 50), "1920x1080 / 60Hz")) { // ResoBtn2
			Debug.Log ("1080p Windowed 144Hz");
			Reso2(); // Call function Reso2
		}
		if (GUI.Button (new Rect (10, 130, 150, 50), "1600x900 / 60Hz")) { // ResoBtn3
			Debug.Log ("900 Windowed 144Hz");
			Reso3(); // Call function Reso3
		}
		if (GUI.Button (new Rect (10, 200, 150, 50), "Set Framerate to Slider")) {
			TFramerate();
		}
		if (GUI.Button (new Rect (170, 10, 100, 50), "VSync On")) {
			VSync1 ();
		}
		if (GUI.Button (new Rect (280, 10, 100, 50), "VSync Off")) {
			VSync0 ();
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
		QualitySettings.vSyncCount = 1;
	}

	void VSync0 () {
		QualitySettings.vSyncCount = 0;
		}


}
