using UnityEngine;
using System.Collections;

public class DestroyOnTouch : MonoBehaviour {

	void OnCollisionEnter () {
		Destroy (this.gameObject);
	}
}
