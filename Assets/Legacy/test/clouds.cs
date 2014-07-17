using UnityEngine;
using System.Collections;

public class clouds : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.SetTextureOffset("_MainTex", Vector2.up * Time.timeSinceLevelLoad/100);
	}
}
