﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint {
	public Vector3 position = Vector3.zero;
	public Vector3 rotation = Vector3.zero;
}

[RequireComponent(typeof (LineRenderer))]
[RequireComponent(typeof (MovingPlatformsEditorStuff))]
public class MovingPlatform : MonoBehaviour {
	
	public enum modes {pingPong, loop};
	public modes mode;

	LineRenderer linerenderer;

	//which way does it go through the waypoints
	bool ping = true;
	
	//in unitymeters/second
	public float speed = 1;
	
	Waypoint[] waypoints;
	
	public float[] waitForSeconds = {0, 0};
	float countdown = 1;
	
	//index of next waypoint
	int target = 1;
	//last waypoint, needed for waiting at said waypoint
	int lasttarget = 0;

	// Use this for initialization
	void Start () {
		linerenderer = GetComponent ("LineRenderer") as LineRenderer;
		setWaypoints ();
		countdown = waitForSeconds[lasttarget];
	}
	
	// Update is called once per frame
	void Update () {
		followWaypoints ();
	}
	
	void setWaypoints () {
		//count waypoints
		//then initialize field of that length
		int wpCount = 0;
		for (int i = 0; i < transform.childCount; i++) {
			string name = transform.GetChild(i).gameObject.name;
			bool isWayPoint = name.Contains("waypoint");
			if (isWayPoint) {
				wpCount++;
			}
		}
		waypoints = new Waypoint[wpCount+1];
		if (mode == modes.loop) {
			linerenderer.SetVertexCount(wpCount+2);
			linerenderer.SetPosition(wpCount+1, transform.position);
		}
		else linerenderer.SetVertexCount(wpCount+1);
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints[i] = new Waypoint();
		}
		
		//fill with position and rotation of waypoints
		waypoints[0].position = transform.position;
		waypoints[0].rotation = transform.eulerAngles;
		linerenderer.SetPosition(0, transform.position);
		//set positions and rotations of children as waypoints
		int index = 0;
		for (int i = 1; i < waypoints.Length; i++) {
			string name = transform.GetChild(index).gameObject.name;
			bool isWayPoint = name.Contains("waypoint");
			while (!isWayPoint) {
				index++;
				if (!transform.GetChild(index)) {
					return;
				}
				name = transform.GetChild(index).gameObject.name;
				isWayPoint = name.Contains("waypoint");
			}
			if (isWayPoint) {
				waypoints[i].position = transform.GetChild(index).position;
				waypoints[i].rotation = transform.GetChild(index).eulerAngles;
				linerenderer.SetPosition(i, transform.GetChild(index).position);
				Destroy(transform.GetChild(index).gameObject);
				index++;
			}
		}
	}
	
	void followWaypoints () {
		countdown = Mathf.Max(countdown - Time.deltaTime, 0);
		Vector3 direction = waypoints[target].position - transform.position;
		//check if it would pass the waypoint in the next frame
		//move one step towards target if it doesn't
		if (direction.magnitude > (direction.normalized * Time.deltaTime * speed).magnitude || countdown > 0) {
			Vector3 step = direction.normalized * Time.deltaTime * speed;
			if (countdown > 0) {
				step = Vector3.zero;
			}
			transform.position += step;
			//calculate fraction of distance covered this frame
			float t = step.magnitude / direction.magnitude;
			if (countdown > 0) {
				t = Time.deltaTime / countdown;
			}
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, waypoints[target].rotation, t);
		}
		else if (countdown == 0) {
			//reset countdown
			countdown = waitForSeconds[target];
			lasttarget = target;
			
			float outstandingMovement = (direction.normalized * Time.deltaTime * speed).magnitude - direction.magnitude;
			//set to position
			transform.position = waypoints[target].position;
			transform.eulerAngles = waypoints[target].rotation;
			//change target
			//loop-style
			if (mode == modes.loop) {
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
			direction = waypoints[target].position - transform.position;
			transform.position += direction.normalized * Time.deltaTime * outstandingMovement;
		}
	}
}
