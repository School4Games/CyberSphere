using UnityEngine;
using System.Collections;

public class SwitchCamera : MonoBehaviour {

	public bool RoCa;
	public bool NmCa = false;

	// Use this for initialization
	void Start () {
		if (NmCa == false) {
			gameObject.GetComponent<RotateAroundTarget> ().enabled = true;
			transform.parent.GetComponent<SphereController> ().enabled = false;
			RoCa = true;
		} 
		else {
			transform.parent.GetComponent<SphereController> ().enabled = true;
			gameObject.GetComponent<TPCamera>().enabled = true;
			gameObject.GetComponent<RotateAroundTarget> ().enabled = false;

		}

	}
	
	// Update is called once per frame
	void Update () {
		//RoCaF ();
		NmCaF ();
	}

//	private void RoCaF () {
//		if ((RoCa == false) && (Input.GetKeyDown (KeyCode.Q))) {
//			gameObject.GetComponent<TPCamera>().enabled = false;
//			gameObject.GetComponent<RotateAroundTarget>().enabled = true;
//			transform.parent.GetComponent<SphereController>().enabled = false;
//			RoCa = true;
//			NmCa = false;
//		}
//	}

	private void NmCaF () {
		if ((NmCa == false) && ((Input.GetKeyDown (KeyCode.W)) || (Input.GetKeyDown (KeyCode.A)) || (Input.GetKeyDown (KeyCode.S)) || (Input.GetKeyDown (KeyCode.D)) || (Input.GetKeyDown (KeyCode.Space)))) {
			transform.parent.GetComponent<SphereController>().enabled = true;
			gameObject.GetComponent<RotateAroundTarget>().enabled = false;
			NmCa = true;
			RoCa = false;
			gameObject.GetComponent<TPCamera>().enabled = true;
		}
	}
}
