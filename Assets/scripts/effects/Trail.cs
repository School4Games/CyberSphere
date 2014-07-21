using UnityEngine;
using System.Collections;


[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (MeshFilter))]
public class Trail : MonoBehaviour {

	public float segmentWidth = 0.1f;
	public float segmentHeight = 1;

	Transform parent;

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
		transform.parent = null;
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
	
	// Update is called once per frame
	void Update () {
		//when segmentWidth is reached, create new segment
		if (Vector3.Distance(mesh.vertices[vert1], mesh.vertices[vert1-2]) >= segmentWidth) {
			Debug.Log(Vector3.Distance(mesh.vertices[vert1], mesh.vertices[vert1-2]));
			createSegment ();
		}
		//else pull segment
		else pull ();
	}

	void pull () {
		newVertices[vert1] = (parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0);
		newVertices[vert2] = (parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0);
		mesh.vertices = newVertices;
	}

	void createSegment () {
		if (vertices.Count >= 2048) {
			vertices.RemoveRange(0, 2);
			uv.RemoveRange(0, 2);
			triangles.RemoveRange(triangles.Count-7, 6);
		}
		vertices.Add((parent.position - transform.position) + new Vector3(0,0.5f * segmentHeight,0));
		vert1 = vertices.Count-1;
		vertices.Add((parent.position - transform.position) + new Vector3(0,-0.5f * segmentHeight,0));
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
