using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			if (Application.levelCount > Application.loadedLevel + 1) {
				//load next level
				Application.LoadLevel (Application.loadedLevel + 1);
			}
			else {
				SphereController sphereController = other.GetComponent("SphereController") as SphereController;
				sphereController.win = true;
			}
		}
	}

	void OnGUI () {

	}
	
	// Update is called once per frame
	void Update () {

	}
}
