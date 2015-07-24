using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Emofani GUI. Displays a GUI to set options or show debug output
/// </summary>
public class EmofaniGUI : EmofaniGlobal
{

	public GameObject optionsObject;

	private string log = "";
	private int yStart, buttonHeight, textareaHeight;
	private RectOffset zeroOffset;
	private Vector2 scrollPosition;
	private bool showDebug = false, showOptions = true;

	/// <summary>
	/// Log the specified text in the Debug Log scroll window
	/// </summary>
	/// <param name="text">Text.</param>
	public void Log(string text)
	{
		log += text + "\n";
		scrollPosition = new Vector2(0, Mathf.Infinity);

		Debug.Log(text);
	}

	/// <summary>
	/// Start this instance. Set initial layout variables
	/// </summary>
	private void Start()
	{
		yStart = 0;
		buttonHeight = 32;
		textareaHeight = Screen.height - buttonHeight;
		zeroOffset = new RectOffset(0, 0, 0, 0);
	}

	/// <summary>
	/// React to key presses
	/// </summary>
	private void Update()
	{
		// register Keypresses
		if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.F1)) {
			showDebug = !showDebug;
			if (showDebug) ShowOptions(false);
		}
		if (Input.GetKeyUp(KeyCode.M) || Input.GetKeyUp(KeyCode.F2)) {
			ToggleOptions();
			if (showOptions) showDebug = false;
		}
		if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.F12)) {
			Application.Quit();
		}
	}

	/// <summary>
	/// Display Debug Log
	/// </summary>
	private void OnGUI()
	{
	if (showDebug) {

			// Shows a big ScrollView with the debug output
			GUI.skin.button.margin = zeroOffset;
			GUI.skin.textArea.margin = zeroOffset;
			GUI.skin.textArea.fixedHeight = textareaHeight;

			GUILayout.Space(yStart);

			scrollPosition = GUILayout.BeginScrollView(
				scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(textareaHeight));

			GUILayout.Label(log);

			GUILayout.EndScrollView();

			// Button to clear the log
			if (GUILayout.Button("Clear Log", GUILayout.Height(buttonHeight))) {
				log = "";
			}

		}
	}

	/// <summary>
	/// Sets the port.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetPort(string value) {
		this.Listener.ReceivePort = int.Parse(value);
	}

	/// <summary>
	/// Sets the background brightness.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetBackgroundBrightness(float value) {
		MainCamera.backgroundColor = new Color(value, value, value);
	}

	/// <summary>
	/// Sets the scale.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetScale(float value) {
		MainCamera.fieldOfView = value;
	}

	/// <summary>
	/// Sets the vertical position.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetVerticalPosition(float value){
		MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, value, MainCamera.transform.position.z);
	}

	/// <summary>
	/// Sets the rotation.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetRotation(float value){
		MainCamera.transform.rotation = Quaternion.AngleAxis (value, Vector3.forward);
	}

	/// <summary>
	/// Sets wether the horizontal gaze position should be flipped
	/// </summary>
	/// <param name="value">If set to <c>true</c>, flips the horizontal gaze.</param>
	public void SetFlipGaze(bool value) {
		FaceAnim.MirrorGaze = value;
	}

	/// <summary>
	/// Sets the vertical head movement.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetVerticalHeadMovement(float value) {
		FaceAnim.VerticalHeadMovement = value/100;
	}

	/// <summary>
	/// Sets the horizontal head movement.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetHorizontalHeadMovement(float value) {
		FaceAnim.HorizontalHeadMovement = value/100;
	}

	/// <summary>
	/// Shows wether the options menu should be shown
	/// </summary>
	/// <param name="value">If set to <c>true</c>, shows the options menu.</param>
	public void ShowOptions(bool value){
		if (optionsObject != null) optionsObject.SetActive(value);
		showOptions = value;
	}

	/// <summary>
	/// Toggles the options menu.
	/// </summary>
	public void ToggleOptions(){
		ShowOptions(!showOptions);
	}

}
