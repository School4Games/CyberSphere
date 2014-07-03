using UnityEngine;
using System.Collections;

public class TriShatter : MonoBehaviour {

	Mesh mesh;

	Vector3[] vertices;
	Vector3[] newVertices;

	int[] triangles;
	int[] newTriangles;

	Vector2[] uv;
	Vector2[] newUV;

	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		triangles = mesh.triangles;
		uv = mesh.uv;
		newVertices = new Vector3[triangles.Length];
		newTriangles = new int[triangles.Length];
		newUV = new Vector2[triangles.Length];
		splitTriangles();
		//test
		makeDoubleSided ();
		StartCoroutine("blackHole");
	}

	void splitTriangles () {
		for (int i=0; i<newVertices.Length; i++) {
			newVertices[i] = vertices[triangles[i]];
			newTriangles[i] = i;
			newUV[i] = uv[triangles[i]];
		}
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
		int[] doubleTriangles = new int[mesh.triangles.Length*2];
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
			newVertices[i] = Vector3.zero;
			mesh.vertices = newVertices;
			if ((i+1) % 3 == 0) {
				yield return new WaitForEndOfFrame ();
			}
		}
	}

	IEnumerator dissolve () {
		for (int i=0; i<newVertices.Length; i++) {
			newTriangles[i] = 0;
			mesh.triangles = newTriangles;
			if ((i+1) % 3 == 0) {
				yield return new WaitForEndOfFrame ();
			}
		}
	}

	IEnumerator combine () {
		int[] newNewTriangles = new int[newTriangles.Length];
		mesh.triangles = newNewTriangles;
		for (int i=0; i<newVertices.Length; i++) {
			newNewTriangles[i] = newTriangles[i];
			mesh.triangles = newNewTriangles;
			if ((i+1) % 3 == 0) {
				yield return new WaitForEndOfFrame ();
			}
		}
	}
}
