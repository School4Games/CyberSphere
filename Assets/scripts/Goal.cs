using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter () {
		//load next level
		Application.LoadLevel (Application.loadedLevel + 1);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
