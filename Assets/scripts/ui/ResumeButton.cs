using UnityEngine;
using System.Collections;

public class ResumeButton : MonoBehaviour {

	public GameObject menu;

	void OnPress () {
		Time.timeScale = 1;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		NGUITools.SetActive(menu, false);
	}
}
