using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MovingPlatformsEditorStuff : MonoBehaviour {

	void waitArrayInitialize () {
		MovingPlatform movingPlatform = gameObject.GetComponent("MovingPlatform") as MovingPlatform;
		//count waypoints
		int wpCount = 0;
		for (int i = 0; i < transform.childCount; i++) {
			string name = transform.GetChild(i).gameObject.name;
			int isWayPoint = name.CompareTo("waypoint");
			if (isWayPoint == 1) {
				wpCount++;
			}
		}
		float[] oldArray = movingPlatform.waitForSeconds;
		movingPlatform.waitForSeconds = new float[wpCount+1];
		for (int i=0; i<oldArray.Length; i++) {
			if (i<movingPlatform.waitForSeconds.Length) {
				movingPlatform.waitForSeconds[i] = oldArray[i];
			}
		}
	}

	void Update () {
		if (!Application.isPlaying) {
			waitArrayInitialize ();
		}
	}
}
