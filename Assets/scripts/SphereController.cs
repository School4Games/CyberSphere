using UnityEngine;
using System.Collections;

public class SphereController : MonoBehaviour {

	public bool spider = true;
	public Vector3 adhesionForce = Vector3.down;

	public float speed = 10;

	public float acceleration = 100;

	public float jumpHeight = 5;

	public Transform graphics;

	bool onGround = false;

	void OnTriggerStay () {
		onGround = true;
	}

	void OnCollisionStay (Collision collision) {
		adhesionForce = -collision.contacts[0].normal;
		adhesionForce.Normalize();
		adhesionForce *= Physics.gravity.magnitude;

		Vector3 v = rigidbody.velocity;
		Vector3 j = -adhesionForce.normalized * jumpHeight;
		Vector3 a = Vector3.Cross(Vector3.up, j);
		a = Vector3.Project(v, a);
		Vector3 b = Vector3.Cross(a, j);
		b = Vector3.Project(v, b);
		v = a + b + j;
		Debug.DrawLine(transform.position, transform.position + j/3, Color.green);
		Debug.DrawLine(transform.position, transform.position + a/3, Color.red);
		Debug.DrawLine(transform.position, transform.position + b/3, Color.blue);
		Debug.DrawLine(transform.position, transform.position + v/3);
		Debug.DrawLine(transform.position, transform.position + rigidbody.velocity/3, Color.magenta);
	}

	void OnTriggerExit () {
		adhesionForce = Vector3.down;
		onGround = false;
	}

	// Use this for initialization
	void Start () {
	
	}

	void move () {
		//apply rotation to graphics!! (?)
		if (Input.GetAxis("Vertical") != 0) {
			//rigidbody.AddTorque (Camera.main.transform.forward * -Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed);
			Vector3 forwardVector = Vector3.Cross(-adhesionForce, Camera.main.transform.right);
			forwardVector.Normalize();
			Vector3 forwardVelocity = Vector3.Project(rigidbody.velocity, forwardVector);
			if (forwardVelocity.magnitude < speed) {
				rigidbody.AddForce(forwardVector * -Input.GetAxis("Vertical") * Time.fixedDeltaTime * acceleration);
			}
		}
		if (Input.GetAxis("Horizontal") != 0) {
			//rigidbody.AddTorque (Camera.main.transform.right * Input.GetAxis("Vertical") * Time.fixedDeltaTime * speed);
			Vector3 rightVector = Vector3.Cross(-adhesionForce, Camera.main.transform.forward);
			rightVector.Normalize();
			Vector3 rightVelocity = Vector3.Project(rigidbody.velocity, rightVector);
			if (rightVelocity.magnitude < speed) {
				rigidbody.AddForce(rightVector * Input.GetAxis("Horizontal") * Time.fixedDeltaTime * acceleration);
			}
		}
		//jump orthogonal to ground
		if (Input.GetButton("Jump") && onGround) {
			Vector3 v = rigidbody.velocity;
			//components of velocity: a, b, j(umpvector: already scaled by jumpheight), all orthogonal
			Vector3 j = -adhesionForce.normalized * jumpHeight;
			Vector3 a = Vector3.Cross(Vector3.up, j);
			a = Vector3.Project(v, a);
			Vector3 b = Vector3.Cross(a, j);
			b = Vector3.Project(v, b);
			//new velocity from components
			v = a + b + j;
			rigidbody.velocity = v;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		move ();
		//turn off gravity when hanging on wall
		if (spider && onGround) {
			rigidbody.useGravity = false;
			rigidbody.AddForce(adhesionForce);
		} 
		else {
			rigidbody.useGravity = true;
		}
		//test
		if (Input.GetKeyDown(KeyCode.T)) {
			spider = !spider;
		}
	}
}
