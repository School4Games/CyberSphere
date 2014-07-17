using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			SphereController sphereController = other.GetComponent("SphereController") as SphereController;
			sphereController.boost ();
		}
	}
}
