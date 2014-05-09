using UnityEngine;
using System.Collections;

public class SphereController : MonoBehaviour {

	public bool spider = true;
	public Vector3 adhesionForce = Vector3.down;

	public float maxSpeed = 10;

	public float maxFallSpeed = 20;

	public float zeroToMaxTime = 0.5f;

	public float jumpHeight = 4;

	public Vector3 gravity = Vector3.down * 20;

	public Transform graphics;

	public float maxSlopeAngle = 45;

	bool onGround = false;

	void OnTriggerStay () {
		onGround = true;
	}

	void OnCollisionEnter (Collision collision) {
		//for "friction" moving platforms etc
		transform.parent = collision.collider.gameObject.transform;
	}

	void OnCollisionStay (Collision collision) {
		adhesionForce = -collision.contacts[0].normal;
		adhesionForce.Normalize();
		adhesionForce *= Physics.gravity.magnitude;
		//for testing
		/*Vector3 v = rigidbody.velocity;
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
		Debug.DrawLine(transform.position, transform.position + rigidbody.velocity/3, Color.magenta);*/
	}

	void OnTriggerExit () {
		onGround = false;
		transform.parent = null;
	}

	// Use this for initialization
	void Start () {
		rigidbody.useGravity = false;
	}

	void move () {
		Vector3 upVector = Vector3.up;
		if (spider) {
			upVector = -adhesionForce;
		}
		Vector3 forwardVector = Vector3.Cross(upVector, Camera.main.transform.right);
		Vector3 rightVector = Vector3.Cross(upVector, Camera.main.transform.forward);

		Vector3 inputDirection = (rightVector * Input.GetAxis("Horizontal") + forwardVector * -Input.GetAxis("Vertical")).normalized;
		inputDirection *= Mathf.Max(Mathf.Abs(Input.GetAxis("Vertical")), Mathf.Abs(Input.GetAxis("Horizontal")));

		//what exactly was i thinking here? - - < ??? -_- works, though
		if (Vector3.Angle(-inputDirection, -adhesionForce) < 90 - maxSlopeAngle || spider) {
			inputDirection = Vector3.zero;
		}

		forwardVector.Normalize();
		rightVector.Normalize();
		//pretty simple way to almost ignore momentum 
		Vector3 upwardVelocity = Vector3.Project(rigidbody.velocity, upVector);
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, inputDirection * maxSpeed + upwardVelocity, zeroToMaxTime);

		//jump orthogonal to ground
		//could just as well use impulse, just saying ... xD
		if (Input.GetButtonDown("Jump") && onGround) {
			Vector3 v0 = new Vector3(0, Mathf.Sqrt(-2 * jumpHeight * gravity.y), 0);
			v0 = -adhesionForce.normalized * v0.magnitude;
			rigidbody.AddForce(v0, ForceMode.VelocityChange);
		}
	}

	void spinGraphics () {
		float angle = 360 * ((rigidbody.velocity.magnitude * Time.deltaTime)/(2*Mathf.PI));
		Vector3 axis = Vector3.Cross(-adhesionForce, rigidbody.velocity);
		graphics.Rotate(axis, angle, Space.World);
	}

	void adjustFOV () {
		Vector3 upVector = Vector3.up;
		if (spider) {
			upVector = -adhesionForce;
		}
		Vector3 forwardVector = Vector3.Cross(Camera.main.transform.right, upVector);
		Vector3 rightVector = Vector3.Cross(Camera.main.transform.forward, upVector);

		if (Vector3.Angle(rigidbody.velocity, forwardVector) < 45) {
			float fov = (1 + Mathf.Sqrt(Mathf.Max(rigidbody.velocity.magnitude - maxSpeed, 0) / maxSpeed)/2) * 60;
			Camera.main.fieldOfView = fov;
		}
	}
	
	// Update is called once per frame
	void Update () {
		move ();
		spinGraphics ();
		//adjustFOV ();
		//turn off gravity when hanging on wall
		if (spider && onGround) {
			//make "stickier" somehow
			rigidbody.AddForce(adhesionForce);
		} 
		else {
			rigidbody.AddForce(gravity);
		}
		if (Input.GetButton("Fire2")) {
			spider = true;
		}
		else {
			spider = false;
		}
		//cool
		Color newColor = graphics.renderer.material.color;
		newColor.r -= Time.deltaTime * 2;
		graphics.renderer.material.color = newColor;
	}
}
