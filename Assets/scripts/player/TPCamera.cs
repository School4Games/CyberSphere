using UnityEngine;
using System.Collections;

public class TPCamera : MonoBehaviour {

	public Transform player;

	public float XSensitivity = 100;
	public float YSensitivity = 100;

	public float distanceFromSphere = 5;

	//changes when spidershere activates
	Vector3 upVector = Vector3.up;

	SphereController sphereController;

	//lolz @ distance :D makes sense, though
	void follow () {
		//follow (move directly towards sphere)
		//camera won't go below 0 for some reason ... or does strange things under unknown circumstances
		//kills spidercam ...
		Vector3 distance = transform.position - player.position; 
		Vector3 heightDifference = Vector3.Project (distance, upVector);
		float horizontalDifferenceLength = Mathf.Sqrt (distanceFromSphere * distanceFromSphere - heightDifference.magnitude * heightDifference.magnitude);
		Vector3 horizontalDifference = transform.position - (player.position + heightDifference);
		horizontalDifference.Normalize();
		horizontalDifference *= horizontalDifferenceLength;
		transform.position = player.position + horizontalDifference + heightDifference;
		//check if sth is obstructing view
		distance = transform.position - player.position;
		Ray ray = new Ray (player.position, distance);
		RaycastHit hitinfo = new RaycastHit();
		if (Physics.Raycast (ray, out hitinfo, distance.magnitude)) {
			transform.position = hitinfo.point;
		}
		//point towards sphere
		transform.forward = -distance;
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
		sphereController = player.GetComponent ("SphereController") as SphereController;
	}

	void OnPreRender () {
		rotate ();
		follow ();
	}

	void pointUp () {
		if (sphereController.spider && sphereController.adhesionForce != Vector3.zero) {
			upVector = -sphereController.adhesionForce;
			//if upside down -> turn around x first
			if (Vector3.Angle(transform.up, upVector) > 90) {
				float angle0 = Vector3.Angle(transform.up, upVector);
				int p0 = clockwisePrefix (transform.right, transform.up, upVector);
				transform.RotateAround(transform.position, p0 * -transform.right, Mathf.Sqrt(angle0/10));
			}
			else {
				float angle = Vector3.Angle(Vector3.Cross(transform.forward, upVector), Vector3.Cross(transform.forward, transform.up));
				if (angle > 0 && upVector.normalized != transform.up && transform.forward != upVector.normalized) {
					int p = clockwisePrefix (transform.forward, Vector3.Cross(transform.forward, upVector), Vector3.Cross(transform.forward, transform.up));
					transform.RotateAround(transform.position, p * transform.forward, Mathf.Min(Mathf.Sqrt(angle/10), 2));
				}
			}
		}
		else if (!sphereController.spider && upVector.normalized != transform.up) {
			upVector = Vector3.up;
			//if upside down
			if (Vector3.Angle(transform.up, upVector) > 90) {
				float angle0 = Vector3.Angle(transform.up, upVector);
				int p0 = clockwisePrefix (transform.right, transform.up, upVector);
				transform.RotateAround(transform.position, p0 * -transform.right, Mathf.Sqrt(angle0/10));
			}
			else {
				float angle = Vector3.Angle(Vector3.Cross(transform.forward, Vector3.up), Vector3.Cross(transform.forward, transform.up));
				int p = clockwisePrefix (transform.forward, Vector3.Cross(transform.forward, Vector3.up), Vector3.Cross(transform.forward, transform.up));
				transform.RotateAround(transform.position, p * transform.forward, Mathf.Sqrt(angle/10));
			}
		}
	}

	//returns 1 if clockwise, -1 if counterclockwise
	//can you put stuff like this in some sort of library?
	int clockwisePrefix (Vector3 axis, Vector3 direction, Vector3 targetDirection) {
		if (Vector3.Angle(Vector3.Cross(targetDirection, direction), axis) < 90) {
			return 1;
		}
		else {
			return -1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//broken for some reason xD
		//rotates the camera
		pointUp ();
	}
}
