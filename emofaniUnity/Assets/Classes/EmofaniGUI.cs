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

	public GameObject menuObjectMain, menuObjectInfo, menuObjectOptions, menuObjectSliders,
		settingObjectMirrorGaze, settingsObjectBgBrightness, settingsObjectScale, settingsObjectFacePosVert,
	settingsObjectRotation, settingsObjectHeadMovVert, settingsObjectHeadMovHor, settingsObjectBreathInt,
	settingsObjectPort;

	private string log = "";
	private int yStart, buttonHeight, textareaHeight;
	private RectOffset zeroOffset;
	private Vector2 scrollPosition;
	private bool showDebug = false, showMainMenu = true;
    private EmofaniConfig config;

    /// <summary>
    /// Get/Set the port to listen for UDP messages
    /// </summary>
	private int Port {
		get {
			return this.Listener.ReceivePort;
		}
		set {
			this.Listener.ReceivePort = value;
		}
	}

    /// <summary>
    /// Get/Set the brightness of the background
    /// </summary>
	private float BackgroundBrightness {
		get {
			return MainCamera.backgroundColor.r;
		}
		set {
			MainCamera.backgroundColor = new Color(value, value, value);
		}
	}

    /// <summary>
    /// Get/Set the scale of the scene
    /// </summary>
	private float Scale {
		get {
			return MainCamera.fieldOfView;
		}
		set {
			MainCamera.fieldOfView = value;
		}
	}

    /// <summary>
    /// Get/Set the vertical position of the scene
    /// </summary>
	private float FacePosVert {
		get {
			return MainCamera.transform.position.y;
		}
		set {
			MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, value, MainCamera.transform.position.z);
		}
	}

    /// <summary>
    /// Get/Set the rotation of the scene
    /// </summary>
	private float Rotation {
		get {
			float angle = MainCamera.transform.rotation.eulerAngles.z;
			if (Mathf.RoundToInt(angle) > 180) angle = -360 + angle;
			return angle;
		}
		set {
			MainCamera.transform.rotation = Quaternion.AngleAxis (value, Vector3.forward);
		}
	}

    /// <summary>
    /// Get/Set the amount of vertical head movement when looking at gaze coordinates
    /// </summary>
	private float HeadMovVert {
		get {
			return FaceAnim.VerticalHeadMovement*100;
		}
		set {
			FaceAnim.VerticalHeadMovement = value/100;
		}
	}

    /// <summary>
    /// Get/Set the amount of horizontal head movement when looking at gaze coordinates
    /// </summary>
	private float HeadMovHor {
		get {
			return FaceAnim.HorizontalHeadMovement*100;
		}
		set {
			FaceAnim.HorizontalHeadMovement = value/100;
		}
	}

    /// <summary>
    /// Get/Set the amount of up and down movement from breathing
    /// </summary>
	private float BreathInt {
		get {
			return FaceAnim.BreathingWeight*100;
		}
		set {
			FaceAnim.BreathingWeight = value/100;
		}
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
        config = EmofaniConfig.Instance;
		HideAllMenuPanels();
		InitInputs();
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

    private void OnApplicationQuit()
    {
        config.Save();
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
		Port = int.Parse(value);
        config.SetInt("port", Port);
	}

	/// <summary>
	/// Sets the background brightness.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetBackgroundBrightness(float value) {
		BackgroundBrightness = value;
        config.SetFloat("BgBrightness", value);
	}

	/// <summary>
	/// Sets the scale.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetScale(float value) {
		Scale = value;
        config.SetFloat("Scale", value);
	}

	/// <summary>
	/// Sets the vertical position of the face.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetVerticalPosition(float value){
		FacePosVert = value;
        config.SetFloat("FacePosVert", value);
	}

	/// <summary>
	/// Sets the rotation.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetRotation(float value){
		Rotation = value;
        config.SetFloat("Rotation", value);
	}

	/// <summary>
	/// Sets wether the horizontal gaze position should be flipped
	/// </summary>
	/// <param name="value">If set to <c>true</c>, flips the horizontal gaze.</param>
	public void SetFlipGaze(bool value) {
		FaceAnim.MirrorGaze = value;
        config.SetInt("MirrorGaze", (value)?1:0);
	}

	/// <summary>
	/// Sets the vertical head movement.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetVerticalHeadMovement(float value) {
		HeadMovVert = value;
        config.SetFloat ("HeadMovVert", value);
	}

	/// <summary>
	/// Sets the horizontal head movement.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetHorizontalHeadMovement(float value) {
		HeadMovHor = value;
        config.SetFloat ("HeadMovHor", value);
	}

	/// <summary>
	/// Sets the breathing weight.
	/// </summary>
	/// <param name="value">Value.</param>
	public void SetBreathingWeight(float value) {
		BreathInt = value;
        config.SetFloat ("BreathInt", value);
	}

	/// <summary>
	/// Toggles the main menu.
	/// </summary>
	public void ToggleMainMenu(){
		if (menuObjectMain != null) menuObjectMain.SetActive(showMainMenu = !showMainMenu);
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
		string message = "t:" + (FaceAnim.LastInputId+1);
		message += ";s:127.0.0.1";
		message += ";p:0";
		message += ";d:" + key + "=" + value;
		Debug.Log (message);
		FaceAnim.HandleMessage(message);
	}

	/// <summary>
	/// Hides all menu panels.
	/// </summary>
	private void HideAllMenuPanels() {
		if (menuObjectInfo != null) menuObjectInfo.SetActive(false);
		if (menuObjectSliders != null) menuObjectSliders.SetActive(false);
		if (menuObjectOptions != null) menuObjectOptions.SetActive(false);
		showDebug = false;
	}

    /// <summary>
    /// Initialize input elements with either default values or values loaded from the config file
    /// </summary>
	private void InitInputs(){

		if (settingsObjectPort != null) {
			Port = config.GetInt("port", 11000);
			settingsObjectPort.GetComponent<InputField>().text = Port.ToString();
		}

		if (settingsObjectBgBrightness != null) {
			BackgroundBrightness = config.GetFloat("BgBrightness", 0.1f);
			settingsObjectBgBrightness.GetComponent<Slider>().value = BackgroundBrightness;
		}

		if (settingsObjectScale != null) {
			Scale = config.GetFloat("Scale", 10f);
			settingsObjectScale.GetComponent<Slider>().value = Scale;
		}

		if (settingsObjectFacePosVert != null) {
			FacePosVert = config.GetFloat("FacePosVert", -1.8f);
			settingsObjectFacePosVert.GetComponent<Slider>().value = FacePosVert;
		}

		if (settingsObjectRotation != null) {
			Rotation = config.GetFloat ("Rotation", 0);
			settingsObjectRotation.GetComponent<Slider>().value = Rotation;
		}

		if (settingsObjectHeadMovVert != null) {
			HeadMovVert = config.GetFloat ("HeadMovVert", 50);
			settingsObjectHeadMovVert.GetComponent<Slider>().value = HeadMovVert;
		}

		if (settingsObjectHeadMovHor != null) {
			HeadMovHor = config.GetFloat ("HeadMovHor", 50);
			settingsObjectHeadMovVert.GetComponent<Slider>().value = HeadMovHor;
		}

		if (settingObjectMirrorGaze != null) {
			settingObjectMirrorGaze.GetComponent<Toggle>().isOn =
				FaceAnim.MirrorGaze = 
				(config.GetInt("MirrorGaze", 0) == 1);
		}

		if (settingsObjectBreathInt != null) {
			BreathInt = config.GetFloat ("BreathInt", 50);
			settingsObjectBreathInt.GetComponent<Slider>().value = BreathInt;
		}

	}

}
