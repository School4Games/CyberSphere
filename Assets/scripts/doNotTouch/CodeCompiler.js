#pragma strict

var code : String = "";

var showConsole = false;

public var player : Transform;

var playerScript;

function OnGUI () {
	if (showConsole) {
		code = GUI.TextArea (new Rect(10, 10, 800, 400), code);
		if (GUI.Button (new Rect(10, 420, 80, 20), "Compile")) {
			compile ();
		}
		if (Event.current.keyCode == KeyCode.Escape) {
			showConsole = false;
		}
	}
}

function compile () {
	eval (code);
}

function Update () {
	if (Input.GetKeyDown(KeyCode.Tab)) {
		showConsole = true;
	}
}
