using UnityEngine;
using System.Collections;


[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (MeshFilter))]
public class Trail : MonoBehaviour {

	public float segmentWidth = 0.1f;
	public float segmentHeight = 1;

	Transform parent;
	SphereController sphereController;

	public bool isActive = false;

	public Gradient colorOverTime;

	public int trailLength = 64;

	int vert1;
	int vert2;

	Mesh mesh;
	
	ArrayList vertices = new ArrayList();
	Vector3[] newVertices = new Vector3[4096];

	ArrayList triangles = new ArrayList();
	int[] newTriangles = new int[6144];

	ArrayList uv = new ArrayList();
	Vector2[] newUV = new Vector2[4096];

	// Use this for initialization
	void Start () {
		parent = transform.parent;
		sphereController = parent.GetComponent ("SphereController") as SphereController;
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.MarkDynamic();
		mesh.Clear();
		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		mesh.uv = newUV;
		//create starting vertices
		vertices.Insert(0, (parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0));
		vertices.Insert(0, (parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0));
		uv.Insert(0, new Vector2(0, 1));
		uv.Insert(0, new Vector2(0, 0));
		//test
		setColors();
		createSegment ();
	}

	// Update is called once per frame
	void Update () {
		if ((vertices.Count > trailLength || parent.rigidbody.velocity.magnitude < 0.5f) && vertices.Count > 2) {
			decay ();
		}
		if (isActive) {
			//when segmentWidth is reached, create new segment
			if (Vector3.Distance(mesh.vertices[vert1], mesh.vertices[vert1-2]) >= segmentWidth) {
				createSegment ();
			}
			//else pull segment
			else pull ();
		}
	}

	public void setActive (bool active) {
		if (active == true) {
			if (isActive) {
				return;
			}
			//newTrail ();
			transform.parent = null;
			isActive = true;
		}
		else {
			//clearTrail ();
			//isActive = false;
		}
	}

	void setColors () {
		Color[] colors = new Color[4096];
		for (int i=0; i<vertices.Count; i++) {
			colors[i] = colorOverTime.Evaluate((float)i/(float)vertices.Count);
		}
		mesh.colors = colors;
	}

	void clearTrail () {
		mesh.Clear();
		vertices = new ArrayList(); 
		triangles = new ArrayList();
		uv = new ArrayList();
		vert1 = 0;
		vert2 = 0;
		newVertices = new Vector3[4096];
		newTriangles = new int[6144];
		newUV = new Vector2[4096]; 
	}

	void newTrail () {
		vertices.Insert(0, (parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0));
		vertices.Insert(0, (parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0));
		uv.Insert(0, new Vector2(0, 1));
		uv.Insert(0, new Vector2(0, 0));
		createSegment ();
	}

	void pull () {
		Vector3 vertOffset = Vector3.Cross(sphereController.graphics.forward, sphereController.rigidbody.velocity).normalized * 0.5f;
		newVertices[vert1] = (parent.position - transform.position) + vertOffset * segmentHeight;
		newVertices[vert2] = (parent.position - transform.position) + vertOffset * -segmentHeight;
		mesh.vertices = newVertices;
	}

	void decay () {
		vertices.RemoveRange(vertices.Count-2, 2);
		uv.RemoveRange(0, 2);
		triangles.RemoveRange(0, 6);
		Debug.Log ((vertices.Count-2) + ", " + (vertices.Count-1));
		setColors();
	}

	void createSegment () {
		setColors();
		//just in case ...
		while (vertices.Count > 4000) {
			decay ();
		}
		Vector3 vertOffset = Vector3.Cross(sphereController.graphics.forward, sphereController.rigidbody.velocity).normalized * 0.5f;
		vertices.Insert(0, (parent.position - transform.position) + vertOffset * segmentHeight);
		vert1 = vertices.Count-1;
		vertices.Insert(0, (parent.position - transform.position) + vertOffset * -segmentHeight);
		vert2 = vertices.Count-1;

		uv.Insert(0, new Vector2(uv.Count-3 + segmentWidth/segmentHeight, 1));
		uv.Insert(0, new Vector2(uv.Count-3 + segmentWidth/segmentHeight, 0));

		triangles.Insert(0, vertices.Count-2);
		triangles.Insert(0, vertices.Count-3);
		triangles.Insert(0, vertices.Count-1);
		
		triangles.Insert(0, vertices.Count-2);
		triangles.Insert(0, vertices.Count-1);
		triangles.Insert(0, vertices.Count-0);

		vertices.CopyTo(newVertices);
		uv.CopyTo(newUV);
		triangles.CopyTo(newTriangles);
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}
