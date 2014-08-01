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
		vertices.Add((parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0));
		vertices.Add((parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0));
		uv.Add(new Vector2(0, 1));
		uv.Add(new Vector2(0, 0));
		//test
		createSegment ();
	}

	public void setActive (bool active) {
		if (active == true) {
			if (isActive) {
				return;
			}
			newTrail ();
			transform.parent = null;
			isActive = true;
		}
		else {
			isActive = false;
		}
	}

	void newTrail () {
		vertices.Add((parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0));
		vertices.Add((parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0));
		uv.Add(new Vector2(0, 1));
		uv.Add(new Vector2(0, 0));
		//test
		createSegment ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
			//when segmentWidth is reached, create new segment
			if (Vector3.Distance(mesh.vertices[vert1], mesh.vertices[vert1-2]) >= segmentWidth) {
				createSegment ();
			}
			//else pull segment
			else pull ();
		}
	}

	void pull () {
		newVertices[vert1] = (parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0);
		newVertices[vert2] = (parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0);
		mesh.vertices = newVertices;
	}

	void createSegment () {
		while (vertices.Count >= 2048) {
			vertices.RemoveRange(0, 2);
			uv.RemoveRange(uv.Count-2, 2);
			triangles.RemoveRange(triangles.Count-6, 6);
			//Debug.Log (triangles[triangles.Count-7], triangles[triangles.Count-6], triangles[triangles.Count-5], triangles[triangles.Count-4], triangles[triangles.Count-3], triangles[triangles.Count-2]);
		}
		Vector3 vertOffset = Vector3.Cross(sphereController.graphics.forward, sphereController.rigidbody.velocity).normalized * 0.5f;
		vertices.Add((parent.position - transform.position) + vertOffset * segmentHeight);
		vert1 = vertices.Count-1;
		vertices.Add((parent.position - transform.position) + vertOffset * -segmentHeight);
		vert2 = vertices.Count-1;

		uv.Add(new Vector2(uv.Count-3 + segmentWidth/segmentHeight, 1));
		uv.Add(new Vector2(uv.Count-3 + segmentWidth/segmentHeight, 0));

		triangles.Add(vertices.Count-1);
		triangles.Add(vertices.Count-2);
		triangles.Add(vertices.Count-3);

		triangles.Add(vertices.Count-2);
		triangles.Add(vertices.Count-4);
		triangles.Add(vertices.Count-3);

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
