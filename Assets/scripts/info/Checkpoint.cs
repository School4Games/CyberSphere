using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			SphereController sphereController = other.GetComponent("SphereController") as SphereController;
			sphereController.currentCheckpoint = transform;
			renderer.enabled = false;
			collider.enabled = false;
		}
	}
}
