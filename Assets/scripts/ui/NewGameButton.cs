using UnityEngine;
using System.Collections;

public class NewGameButton : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject menu;

	void OnPress () {
		if (Application.loadedLevel != 1 || menu.name != "Start Menu") {
			Application.LoadLevel(0);
			Destroy(mainMenu);
		}
		Time.timeScale = 1;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		NGUITools.SetActive(menu, false);
	}
}
