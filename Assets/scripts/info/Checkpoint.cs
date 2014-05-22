using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	//for ms2
	public Transform player;
	
	public KeyCode button;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(button)) {
			player.position = transform.position;
		}
	}
}
