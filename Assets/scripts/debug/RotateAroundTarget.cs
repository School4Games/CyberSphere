using UnityEngine;
using System.Collections;

public class RotateAroundTarget : MonoBehaviour {
	
	public GameObject target;//the target object
	private float speedMod = 2.5f;//a speed modifier
	private Vector3 point;//the coord to the point where the camera looks at
	private bool MoarSpeedBtn = false;
	float SW1 = Screen.width * 0.75f;
	float SH1 = Screen.height * 0.75f;

		
	void Start () {//Set up things on the start method
		point = target.transform.position;//get target's coords
		transform.LookAt(point);//makes the camera look to it
	}
	
	void Update () {//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
		transform.RotateAround (point,new Vector3(0.0f,1.0f,0.0f),20 * Time.deltaTime * speedMod);
		if (Input.GetKeyDown (KeyCode.F1)){
			if (MoarSpeedBtn == false) {
				MoarSpeedBtn = true;
			}
			else {
				MoarSpeedBtn = false;
			}
		}
	}
	
	void OnGUI () {
		if (MoarSpeedBtn == true) {
			if (GUI.Button (new Rect ((SW1+40), (SH1+22.5f), 20, 20), "+")) {
				speedMod += 0.125f;
			}
			if (GUI.Button (new Rect ((SW1+40), (SH1+47.5f), 20, 20), "-")) {
				speedMod -= 0.125f;
			}

			GUI.Box (new Rect (SW1, SH1, 100, 100), "" + speedMod);
			GUI.Label (new Rect ((SW1+5), (SH1-20), 100, 100), "Current Speed");


		}
	}
}
