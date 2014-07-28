using UnityEngine;
using System.Collections;

public class SplashImage : MonoBehaviour {

	public float time = 1;

	void OnPress () {
		startGame ();
	}

	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		if (time <= 0) {
			startGame();
		}
	}

	void startGame () {
		Application.LoadLevel (1);
	}
}
