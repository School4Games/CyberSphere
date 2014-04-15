using UnityEngine;
using System.Collections;

public class TPCamera : MonoBehaviour {

	public Transform player;

	public float XSensitivity = 100;
	public float YSensitivity = 100;

	//changes when spidershere activates
	Vector3 upVector = Vector3.up;

	void follow () {
		transform.position = player.position - transform.forward * 5;
	}

	void rotate () {
		if (Input.GetAxis("Mouse X") != 0) {
			transform.RotateAround (player.position, upVector, Input.GetAxis("Mouse X") * Time.deltaTime * XSensitivity);
		}
		if (Input.GetAxis("Mouse Y") != 0) {
			transform.RotateAround (player.position, -transform.right, Input.GetAxis("Mouse Y") * Time.deltaTime * YSensitivity);
		}
	}

	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}

	void OnPreRender () {
		rotate ();
		follow ();
	}

	void checkUp () {
		SphereController sphereController = player.GetComponent ("SphereController") as SphereController;
		if (sphereController.spider && sphereController.adhesionForce != Vector3.zero) {
			upVector = -sphereController.adhesionForce;
			float angle = Vector3.Angle(Vector3.Cross(transform.forward, upVector), Vector3.Cross(transform.forward, transform.up));
			if (angle > 0 && upVector.normalized != transform.up && transform.forward != upVector.normalized) {
				transform.RotateAround(transform.position, transform.forward, angle/10);
			}
		}
		else if (!sphereController.spider && upVector.normalized != transform.up) {
			float angle = Vector3.Angle(Vector3.Cross(transform.forward, Vector3.up), Vector3.Cross(transform.forward, transform.up));
			transform.RotateAround(transform.position, transform.forward, angle/10);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//broken for some reason xD
		checkUp ();
	}
}
