using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	LineRenderer linerenderer;

	public Transform beam;

	public Transform sparks;
	ParticleSystem sparticles;

	public float dps = 10;

	// Use this for initialization
	void Start () {
		sparticles = sparks.gameObject.GetComponent ("ParticleSystem") as ParticleSystem;
		linerenderer = beam.gameObject.GetComponent("LineRenderer") as LineRenderer;
		beam.parent = null;
		linerenderer.useWorldSpace = true;
	}

	void drawLine () {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hitinfo = new RaycastHit ();
		Physics.Raycast(ray, out hitinfo);
		linerenderer.SetPosition (0, transform.position);
		if (hitinfo.point != Vector3.zero) {
			linerenderer.SetPosition (1, hitinfo.point);
			sparks.position = hitinfo.point;
			sparticles.enableEmission = true;
			
		}
		else {
			linerenderer.SetPosition (1, transform.position + transform.forward * 1000);
			sparticles.enableEmission = false;
		}
		if (hitinfo.collider) {
			//damage sphere
			GameObject sphere = hitinfo.collider.gameObject;
			if (sphere.tag == "Player") {
				SphereController sphereController = sphere.GetComponent("SphereController") as SphereController;
				sphereController.health -= dps * Time.deltaTime;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		drawLine ();
	}
}
