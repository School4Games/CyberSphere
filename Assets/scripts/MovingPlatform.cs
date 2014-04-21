using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public bool pingPong;
	//which way does it go through the waypoints
	bool ping = true;

	//in unitymeters/second
	public float speed = 1;

	//make automatic for comfort!
	Vector3[] waypoints;
	//index of next waypoint
	int target = 1;

	void setWaypoints () {
		waypoints = new Vector3[transform.childCount+1];
		waypoints [0] = transform.position;
		//set positions of children as waypoints
		for (int i = 0; i < waypoints.Length-1; i++) {
			waypoints[i+1] = transform.GetChild(i).position;
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	void followWaypoints () {
		Vector3 direction = waypoints[target] - transform.position;
		//check if it would pass the waypoint in the next frame
		//move one step towards target if it doesn't
		if (direction.magnitude > (direction.normalized * Time.deltaTime * speed).magnitude) {
			transform.position += direction.normalized * Time.deltaTime * speed;
		}
		else {
			float outstandingMovement = (direction.normalized * Time.deltaTime * speed).magnitude - direction.magnitude;
			//set to position
			transform.position = waypoints[target];
			//change target
			//not pingpong-style
			if (!pingPong) {
				if (target+1 <= waypoints.Length-1) {
					target++;
				}
				else {
					target = 0;
				}
			}
			//change target
			//pingpong-style
			else {
				if (ping) {
					if (target+1 <= waypoints.Length-1) {
						target++;
					}
					else {
						ping = false;
						target--;
					}
				}
				else if (!ping) {
					if (target-1 >= 0) {
						target--;
					}
					else {
						ping = true;
						target++;
					}
				}
			}
			//add missing part of movement
			direction = waypoints[target] - transform.position;
			transform.position += direction.normalized * Time.deltaTime * outstandingMovement;
		}
	}

	// Use this for initialization
	void Start () {
		setWaypoints ();
	}
	
	// Update is called once per frame
	void Update () {
		followWaypoints ();
	}
}
