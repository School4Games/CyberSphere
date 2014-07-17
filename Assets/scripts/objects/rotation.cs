using UnityEngine;
using System.Collections;

public class rotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Time.deltaTime, 0, 0);
		transform.Rotate(1, Time.deltaTime, 0, Space.World);
	}
}
