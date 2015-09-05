using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;

/// <summary>
/// Emofani GUI. Displays a GUI to set options or show debug output
/// </summary>
public class EmofaniGUI : EmofaniGlobal
{

	public GameObject menuObjectMain, menuObjectInfo, menuObjectOptions, menuObjectSliders;

	private string log = "";
	private int yStart, buttonHeight, textareaHeight, messageNo;
	private RectOffset zeroOffset;
	private Vector2 scrollPosition;
	private bool showDebug = false, showMainMenu = true;

	/// <summary>
	/// Start this instance. Set initial layout variables
	/// </summary>
	private void Start()
	{
		yStart = 0;
		buttonHeight = 32;
		textareaHeight = Screen.height - buttonHeight;
		zeroOffset = new RectOffset(0, 0, 0, 0);
		HideAllMenuPanels();
	}

	/// <summary>
	/// React to key presses
	/// </summary>
	private void Update()
	{
		// register Keypresses

		if (Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.F1)) {
			ToggleMenuPanel("Info");
		}
		if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.F2)) {
			ToggleMenuPanel("Options");
		}
		if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.F3)) {
			ToggleMenuPanel("Sliders");
		}

		if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.F4)) {
			ToggleMenuPanel("Debug");
		}

		if (Input.GetKeyUp(KeyCode.H) || Input.GetKeyUp(KeyCode.F5)) {
			ToggleMainMenu();
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
	/// Sets the breathing weight.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetBreathingWeight(float value) {
		FaceAnim.SetBreathingWeight(value/100);
	}

	/// <summary>
	/// Toggles the main menu.
	/// </summary>
	public void ToggleMainMenu(){
		if (menuObjectMain != null) menuObjectMain.SetActive(showMainMenu = !showMainMenu);
	}

	/// <summary>
	/// Hides all menu panels.
	/// </summary>
	public void HideAllMenuPanels() {
		if (menuObjectInfo != null) menuObjectInfo.SetActive(false);
		if (menuObjectSliders != null) menuObjectSliders.SetActive(false);
		if (menuObjectOptions != null) menuObjectOptions.SetActive(false);
		showDebug = false;
	}

	/// <summary>
	/// Toggles certain menu panels (and hides the others).
	/// </summary>
	/// <param name="target">Target.</param>
	public void ToggleMenuPanel(string target){
		bool currentVisibility = false;
		if (target == "Debug") {
			// Debug-"Panel" is a special case at the moment
			currentVisibility = showDebug;
			HideAllMenuPanels();
			showDebug = !currentVisibility;
		} else {
			GameObject panelObject = null;
			try {
				panelObject = (GameObject)this.GetType().GetField("menuObject" + target).GetValue(this);
			} catch(Exception e) {
				Debug.Log (e.Message);
			}
			if (panelObject != null) {
				currentVisibility = panelObject.activeSelf;
				HideAllMenuPanels();
				panelObject.SetActive(!currentVisibility);
			}
		}
	}

	/// <summary>
	/// Sets the "pleasure" parameter of the face animation
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetAnimationValuePleasure(float value) {
		SetAnimationValue("pleasure", value.ToString());
	}

	/// <summary>
	/// Sets the "arousal" parameter of the face animation
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetAnimationValueArousal(float value) {
		SetAnimationValue("arousal", value.ToString());
    }

	/// <summary>
	/// Sets the "talking" parameter of the face animation
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void SetAnimationValueTalking(bool value) {
		SetAnimationValue("talking", (value)?"true":"false");
	}

	/// <summary>
	/// Quit Button Press
	/// </summary>
	public void QuitButton(){
		Application.Quit();
	}

	/// <summary>
	/// Sets a parameter for the face animation
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	private void SetAnimationValue(string key, string value) {
		string message = "t:" + (messageNo++);
		message += ";s:127.0.0.1";
		message += ";p:0";
		message += ";d:" + key + "=" + value;
		Debug.Log (message);
		FaceAnim.HandleMessage(message);
	}

}
