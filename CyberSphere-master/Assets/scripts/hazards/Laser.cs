using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	LineRenderer linerenderer;

	public Transform beam;

	public Transform sparks;

	// Use this for initialization
	void Start () {
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
			sparks.gameObject.SetActive(true);
			
		}
		else {
			linerenderer.SetPosition (1, transform.position + transform.forward * 1000);
			sparks.gameObject.SetActive(false);
		}
		//damage sphere
		/*GameObject sphere = hitinfo.collider.gameObject;
		if (sphere.GetComponent("SphereController")) {
			SphereController sphereController = sphere.GetComponent("SphereController") as SphereController;
			GameObject graphics = sphereController.graphics.gameObject;
			Color newColor = graphics.renderer.material.color;
			newColor.r += Time.deltaTime * 10;
			graphics.renderer.material.color = newColor;
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		drawLine ();
	}
}
