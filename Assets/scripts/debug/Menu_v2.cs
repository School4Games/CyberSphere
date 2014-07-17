using UnityEngine;
using System.Collections;

public class Menu_v2 : MonoBehaviour 
{
	public GUISkin guiSkin; // (?) Bestimmt GUI Skin (Aussehen) [Experimentell / Needs Research]
	public bool BeweglichesFenster = false; // Bool ob das Fenster per Drag n' Drop bewegt werden kann [CAREFULLLLLL]
	public string PlayLevel = "1"; // String der bestimmt welches Level beim klicken von Play gestartet wird. (WIP)
	public string[] TeamFenster = new string[0]; // Anzahl der Zeilen im Team Fenster
	
	
	private string clicked = "", ZeilenBeiTeam = "Team \n "; // \n not working, keep Zeile 1 (Element 0) empty to create space between head and content.
	private Rect WindowRect = new Rect((Screen.width / 8f) - 100, Screen.height / 20f, 200, 200); // Hauptmenü Fenster wird erstellt
	private float volume = 1.0f; // Standard Lautstärke
	
	private void Start()
	{
		for (int x = 0; x < TeamFenster.Length;x++ ) // Abhängig von den festgelegten Zeilen im Editor, werden \n im Team Fenster hinzugefügt.
		{
			ZeilenBeiTeam += TeamFenster[x] + " \n ";
		}
		ZeilenBeiTeam += "\n ESC to go back";
	}
	
	private void OnGUI()
	{
		GUI.skin = guiSkin;
		if (clicked == "")
		{
			WindowRect = GUI.Window(0, WindowRect, menuFunc, "Main Menu"); // Überschrift für Hauptmenü
		}
		else if (clicked == "options")
		{
			WindowRect = GUI.Window(1, WindowRect, optionsFunc, "Options");
		}
		else if (clicked == "team")
		{
			GUI.Box(new Rect (0,0,Screen.width,Screen.height), ZeilenBeiTeam);
		}else if (clicked == "resolution") // Auflösungen im Options Menü
		{
			GUILayout.BeginVertical(); // Funktion die alle möglichen Bildschirmauflösungen in einer Liste anzeigt
			for (int x = 0; x < Screen.resolutions.Length;x++ )
			{
				if (GUILayout.Button(Screen.resolutions[x].width + "X" + Screen.resolutions[x].height))
				{
					Screen.SetResolution(Screen.resolutions[x].width,Screen.resolutions[x].height,true);
				}
			}
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Back"))
			{
				clicked = "options";
			}
			GUILayout.EndHorizontal();
		}
	}
	
	private void optionsFunc(int id)
	{
		if (GUILayout.Button("Resolution"))
		{
			clicked = "resolution";
		}
		GUILayout.Box("Volume");
		volume = GUILayout.HorizontalSlider(volume ,0.0f,1.0f);
		AudioListener.volume = volume;
		if (GUILayout.Button("Back"))
		{
			clicked = "";
		}
		if (BeweglichesFenster)
			GUI.DragWindow(new Rect (0,0,Screen.width,Screen.height));
	}
	
	private void menuFunc(int id)
	{
		// All Menu Btn's
		if (GUILayout.Button("Play")) // Play Button
		{
			// Play Game wird angeklickt
			int LoadLevelint;
			int.TryParse (PlayLevel, out LoadLevelint); 
			Application.LoadLevel(LoadLevelint); // Level aus dem String "PlayLevel" wird geladen (0 - 10000)
		}
		if (GUILayout.Button("Options"))
		{
			clicked = "options";
		}
		if (GUILayout.Button("Team"))
		{
			clicked = "team";
		}
		if (GUILayout.Button("Close"))
		{
			Application.Quit();
		}
		if (BeweglichesFenster) // schaltet Dragable window an
			GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	}
	
	private void Update()
	{
		if (clicked == "team" && Input.GetKey (KeyCode.Escape)) // Wenn im Team Fenster, ermögliche die Verwendung von Escape um den clicked state auf 0 zu setzen (back to main)
			clicked = ""; // Empty = nothing clicked = back to main
	}
}