using UnityEngine;
using System.Collections;

public class SphereController : MonoBehaviour {

	public bool spider = true;
	public Vector3 adhesionForce = Vector3.down;

	public bool win = false;

	//[um/s] (unitymeters per second)
	public float maxSpeed = 10;
	float initialMaxSpeed;

	//[um/s]
	public float maxFallSpeed = 20;

	//[s]
	public float zeroToMaxTime = 0.5f;

	//[um]
	public float jumpHeight = 2;

	//additional height gained when jumping holding down space
	//[um]
	public float additionalJumpHeight = 2;

	//[um/s²]
	public Vector3 gravity = Vector3.down * 20;
	float gravityMultiplier = 1;

	public Transform graphics;
	public Trail trail;
	//time until sphere aligns with movement direction
	//[s]
	public float tumbleTime = 2;
	float tumbleCountDown;

	//not used yet
	public float maxSlopeAngle = 45;

	public float maxHealth = 100;
	public float health = 100;

	public Transform currentCheckpoint;

	public float boostMultiplier = 1.5f;
	//[s]
	public float boostDuration = 2;

	TriShatter effects;
	bool dead = false;

	bool onGround = false;

	void OnCollisionEnter (Collision collision) {
		//for "friction" moving platforms etc
		//transform.parent = collision.collider.gameObject.transform;
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
	// Use this for initialization
	void Start () {
		initialMaxSpeed = maxSpeed;
		effects = graphics.GetComponent("TriShatter") as TriShatter;
		tumbleCountDown = tumbleTime;
		rigidbody.useGravity = false;
	}

	void FixedUpdate () {
		//turn off gravity when hanging on wall
		if (spider && onGround) {
			//make "stickier" somehow
			rigidbody.AddForce(adhesionForce);
		} 
		else if (rigidbody.velocity.magnitude < maxFallSpeed || rigidbody.velocity.y >= 0) {
			rigidbody.AddForce(gravity*gravityMultiplier*Time.timeScale, ForceMode.Force);
		}
	}



	// Update is called once per frame
	void Update () {
		//check if touching ground
		int layermask = 1 << 0;
		if (Physics.CheckSphere(transform.position, 0.7f, layermask)) {
			onGround = true;
		}
		else {
			onGround = false;
		}
		move ();
		spinGraphics ();
		tint ();
		//adjustFOV ();
		if (Input.GetButton("Fire2")) {
			spider = true;
		}
		else {
			spider = false;
		}
		if (transform.position.y < 0) {
			health = 0;
		}
		if (health <= 0) {
			if (!dead) {
				//effects.playEffect("dissolve");
			}
			dead = true;
			//press key to respawn
			if (Input.anyKeyDown) {
				transform.position = currentCheckpoint.position;
				health = maxHealth;
				dead = false;
				//effects.playEffect("combine");
			}
		}
	}

	void OnGUI () {
		GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 - 30, 100, 60), onGround.ToString() + "\n" + (-adhesionForce).ToString());
		if (win) {
			//show Win Message
			GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 - 30, 100, 60), "You Win \n Congratulations :D");
		}
		if (health <= 0) {
			//show Game Over Message
			GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 - 30, 100, 60), "Game Over \n Try again? \n (Press any key)");
		}
	}

	public void boost () {
		StartCoroutine ("coBoost");
	}

	IEnumerator coBoost () {
		maxSpeed = initialMaxSpeed * boostMultiplier;
		yield return new WaitForSeconds (boostDuration);
		maxSpeed = initialMaxSpeed;
	}

	void tint () {
		Color newColor = graphics.renderer.material.color;
		newColor.g = health/maxHealth;
		newColor.b = health/maxHealth;
		graphics.renderer.material.color = newColor;
	}

	void move () {
		Vector3 upVector = Vector3.up;
		if (spider && onGround) {
			upVector = -adhesionForce;
		}
		Vector3 forwardVector = -Vector3.Cross(upVector, Camera.main.transform.right);
		Vector3 rightVector = Vector3.Cross(upVector, Camera.main.transform.forward);
		Vector3 inputDirection = (rightVector * Input.GetAxis("Horizontal") + forwardVector * Input.GetAxis("Vertical")).normalized;
		inputDirection *= Mathf.Max(Mathf.Abs(Input.GetAxis("Vertical")), Mathf.Abs(Input.GetAxis("Horizontal")));

		//what exactly was i thinking here? - - < ??? -_- works, though
		if (Vector3.Angle(-inputDirection, -adhesionForce) < 90 - maxSlopeAngle && !spider) {
			inputDirection = Vector3.zero;
		}

		upVector.Normalize();
		forwardVector.Normalize();
		rightVector.Normalize();
		Debug.DrawLine(transform.position, transform.position + upVector, Color.green);
		Debug.DrawLine(transform.position, transform.position + rightVector, Color.red);
		Debug.DrawLine(transform.position, transform.position + forwardVector, Color.blue);
		//pretty simple way to almost ignore momentum 
		Vector3 upwardVelocity = Vector3.Project(rigidbody.velocity, upVector);
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, inputDirection * maxSpeed + upwardVelocity, zeroToMaxTime);

		//jump orthogonal to ground
		if (Input.GetButtonDown("Jump") && onGround) {
			Debug.Log("jump");
			//do calculation at start? somehow buggy at low framerates, though, best fix first
			Vector3 v0 = new Vector3(0, Mathf.Sqrt(-2 * jumpHeight * gravity.y), 0);
			v0 = -adhesionForce.normalized * v0.magnitude;
			rigidbody.AddForce(new Vector3(0, 20, 0), ForceMode.VelocityChange);
			Debug.Log(v0);
		}
		/*if (Input.GetButton("Jump")) {
			float v0 = Mathf.Sqrt(-2 * jumpHeight * gravity.y);
			gravityMultiplier  = v0*v0/((jumpHeight+additionalJumpHeight)*gravity.magnitude)/2;
		}*/
		else {
			gravityMultiplier  = 1;
		}
	}

	void spinGraphics () {
		if (rigidbody.velocity.magnitude >= maxSpeed * 0.9f) {
			tumbleCountDown -= Time.deltaTime;
			if (tumbleCountDown <= 0) {
				trail.setActive(true);
				//make quaternion and lerp? (see cameraController.pointUp())
				//align with movement direction
				float straightAngle = Vector3.Angle(graphics.forward, Vector3.Cross(-adhesionForce, rigidbody.velocity));
				straightAngle = Mathf.Sqrt (straightAngle)/2;
				Vector3 straightAxis = rigidbody.velocity;
				int p = clockwisePrefix (straightAxis, graphics.forward, Vector3.Cross(rigidbody.velocity, -adhesionForce));
				graphics.Rotate(straightAxis, p * straightAngle, Space.World);
			}
		}
		else {
			trail.setActive(false);
			tumbleCountDown = tumbleTime;
		}
		float angle = 360 * ((rigidbody.velocity.magnitude * Time.deltaTime)/(2*Mathf.PI));
		Vector3 axis = Vector3.Cross(-adhesionForce, rigidbody.velocity);
		graphics.Rotate(axis, angle, Space.World);
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
}
