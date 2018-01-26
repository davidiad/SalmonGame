#pragma strict

var textSize : int = 120;
var healthString = "";
// JavaScript, using Unity's built-in system for creating Buttons.
function OnGUI () {
	// Make a background box
	GUI.Box (Rect (10,10,210,90), "");
	GUI.skin.button.fontSize = textSize;
	GUI.skin.box.fontSize = textSize;
	GUI.skin.box.alignment = TextAnchor.MiddleLeft;
	// If button is pressed, the level is reloaded, resetting all variables
	if (GUI.Button (Rect (15,15,200,80), "Reset")) {
		Application.LoadLevel (0);
		FishScript.health = 100.0;
	}
	// another bg box for health
	GUI.Box (Rect (240,10,250,90), " Health: ");
	healthString = Mathf.Round(FishScript.health).ToString();
	GUI.Button (Rect (375,15,110,80), healthString );
	var textPos : int = Screen.width - 250;
	var instructionsStyle : GUIStyle = new GUIStyle();
	instructionsStyle.fontSize = 19;
	GUI.Label (Rect (textPos, 15, 400,60), "drag here to rotate the view", instructionsStyle );
	GUI.Label (Rect (textPos, 980, 400,60), "drag to rotate the fish", instructionsStyle );
	GUI.Label (Rect (textPos, 1000, 400,60), "tap the fish to jump", instructionsStyle );
}