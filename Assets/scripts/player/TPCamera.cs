using UnityEngine;
using System.Collections;

public class TPCamera : MonoBehaviour {

	public Transform player;

	public float XSensitivity = 10;
	public float YSensitivity = 10;

	public float distanceFromSphere = 5;

	public float heightOffset;

	public float turnSluggishness = 5;

	//changes when spidershere activates
	Vector3 upVector = Vector3.up;

	SphereController sphereController;

	Quaternion rotation;
	Vector3 relativePosition;

	// Use this for initialization
	void Start () {
		relativePosition = (transform.position - player.position).normalized;
		rotation = transform.rotation;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		sphereController = player.GetComponent ("SphereController") as SphereController;
	}
	
	void OnPostRender () {
		//distanceBySpeed ();
		rotate ();
		follow ();
		pointUp ();
		checkCollision ();
	}

	void checkCollision () {
		Ray ray = new Ray(player.position, relativePosition);
		RaycastHit hitinfo;
		if (Physics.Raycast(ray, out hitinfo, Vector3.Distance(player.position, transform.position))) {
			transform.position = hitinfo.point;
		}
	}

	void distanceBySpeed () {
		distanceFromSphere = 5 * (1 + player.rigidbody.velocity.magnitude/20);
	}

	void follow () {
		transform.position = player.position + relativePosition * distanceFromSphere;
	}

	void rotate () {
		//mouse look
		if (Input.GetAxis("Mouse X") != 0) {
			relativePosition = Vector3.Lerp(relativePosition, Mathc.sign(Input.GetAxis("Mouse X")) * -transform.right, Time.deltaTime * Mathf.Abs(Input.GetAxis("Mouse X")) * XSensitivity).normalized;
		}
		if (Input.GetAxis("Mouse Y") != 0) {
			relativePosition = Vector3.Lerp(relativePosition, Mathc.sign(Input.GetAxis("Mouse Y")) * -upVector, Time.deltaTime * Mathf.Abs(Input.GetAxis("Mouse Y")) * YSensitivity).normalized;
		}

		//orbit
		//should probably put in own parameter controling "sensitivity"
		if (Input.GetAxis("Horizontal") != 0) {
			relativePosition = Vector3.Lerp(relativePosition, Mathc.sign(Input.GetAxis("Horizontal")) * -transform.right, Time.deltaTime * Mathf.Abs(Input.GetAxis("Horizontal")) * XSensitivity).normalized;
		}
	}

	void pointUp () {
		if (sphereController.spider && sphereController.adhesionForce != Vector3.zero) {
			upVector = -sphereController.adhesionForce;

		}
		else if (!sphereController.spider && upVector.normalized != transform.up) {
			upVector = Vector3.up;
		}
		Vector3 forwardVector = player.position + transform.up * heightOffset - transform.position;
		rotation.SetLookRotation (forwardVector, upVector);
		transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * turnSluggishness);
	}
}
