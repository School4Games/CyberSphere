using UnityEngine;
using System.Collections;

public class TriShatter : MonoBehaviour {

	Mesh mesh;
	
	Vector3[] vertices;
	Vector3[] splitVertices;
	Vector3[] newVertices;

	int[] triangles;
	int[] newTriangles;
	int[] doubleTriangles;

	Vector2[] uv;
	Vector2[] newUV;

	int speed = 7;

	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		triangles = mesh.triangles;
		uv = mesh.uv;
		newVertices = new Vector3[triangles.Length];
		newTriangles = new int[triangles.Length];
		newUV = new Vector2[triangles.Length];
		splitTriangles ();
		makeDoubleSided ();
		restore ();
	}

	//strange things happen when you assign mesh verts from array (array changes with mesh)
	public void playEffect (string name) {
		StopAllCoroutines ();
		restore ();
		newVertices = splitVertices;
		mesh.vertices = newVertices;
		mesh.triangles = doubleTriangles;
		StartCoroutine (name);
	}

	void splitTriangles () {
		for (int i=0; i<newVertices.Length; i++) {
			newVertices[i] = vertices[triangles[i]];
			newTriangles[i] = i;
			newUV[i] = uv[triangles[i]];
		}
		splitVertices = newVertices;
		mesh.Clear ();
		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		mesh.uv = newUV;
		mesh.uv1 = newUV;
		mesh.uv2 = newUV;
		mesh.RecalculateNormals ();
		mesh.MarkDynamic ();
	}

	void makeDoubleSided () {
		doubleTriangles = new int[mesh.triangles.Length*2];
		for (int i=0; i<mesh.triangles.Length; i++) {
			doubleTriangles[i] = mesh.triangles[i];
			if ((i+1)%3 == 1) {
				doubleTriangles[i + mesh.triangles.Length + 2] = mesh.triangles[i];
			}
			else if ((i+1)%3 == 2) {
				doubleTriangles[i + mesh.triangles.Length] = mesh.triangles[i];
			}
			else if ((i+1)%3 == 0) {
				doubleTriangles[i + mesh.triangles.Length - 2] = mesh.triangles[i];
			}
		}
		mesh.triangles = doubleTriangles;
		mesh.RecalculateNormals ();
	}

	void restore () {
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.uv1 = uv;
		mesh.uv2 = uv;
		mesh.RecalculateNormals();
	}

	IEnumerator shatter () {
		for (float t=0; t<5; t+=Time.deltaTime) {
			for (int i=0; i<30; i++) {
				Vector3 normal = mesh.normals[i/3];
				newVertices[i] += normal * Time.deltaTime;
			}
			mesh.vertices = newVertices;
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator blackHole () {
		for (int i=0; i<newVertices.Length; i++) {
			//newVertices[i] = Vector3.zero;
			mesh.vertices[i] = Vector3.zero;
			if ((i+1) % (3*speed) == 0) {
				yield return new WaitForEndOfFrame ();
			}
		}
	}

	IEnumerator dissolve () {
		for (int i=0; i<newVertices.Length; i++) {
			newTriangles[i] = 0;
			mesh.triangles = newTriangles;
			if ((i+1) % (3*speed) == 0) {
				yield return new WaitForEndOfFrame ();
			}
		}
	}

	IEnumerator combine () {
		mesh.Clear ();
		newVertices = splitVertices;
		mesh.vertices = newVertices;
		//newTriangles = doubleTriangles;
		int[] newNewTriangles = new int[doubleTriangles.Length];
		mesh.triangles = doubleTriangles;
		for (int i=0; i<doubleTriangles.Length/2; i++) {
			newNewTriangles[i] = doubleTriangles[i];
			//other side of triangle
			if ((i+1)%3 == 1) {
				newNewTriangles[i + newNewTriangles.Length/2 + 2] = newTriangles[i];
			}
			else if ((i+1)%3 == 2) {
				newNewTriangles[i + newNewTriangles.Length/2] = newTriangles[i];
			}
			else if ((i+1)%3 == 0) {
				newNewTriangles[i + newNewTriangles.Length/2 - 2] = newTriangles[i];
			}
			mesh.triangles = newNewTriangles;
			if ((i+1) % (3*speed) == 0) {
				yield return new WaitForEndOfFrame ();
			}
		}
		restore ();
		yield return new WaitForEndOfFrame ();
	}
}
