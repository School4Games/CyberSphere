using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject startMenu;
	public GameObject ingameMenu;

	// Use this for initialization
	void Start () {
		Time.timeScale = 0;
		DontDestroyOnLoad(transform.gameObject);
		Screen.showCursor = true;
		Screen.lockCursor = false;
		NGUITools.SetActive(startMenu, true);
		NGUITools.SetActive(ingameMenu, false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Time.timeScale = 0;
			NGUITools.SetActive(ingameMenu, true);
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
	}
}
