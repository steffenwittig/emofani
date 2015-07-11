using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Emofani GUI. Displays a GUI to set options or show debug output
/// </summary>
public class EmofaniGUI : EmofaniGlobal
{

	private string log = "", portInput = "11000";
	private int yStart, buttonHeight, textareaHeight;
	private RectOffset zeroOffset;
	private Vector2 scrollPosition;
	private bool showDebug = false, showMenu = true;

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
	/// React to key presses for "debug" or "menu" keys (as specified in the start up window)
	/// </summary>
	private void Update()
	{
		// register Keypresses
		if (Input.GetButtonUp("debug")) {
			showDebug = !showDebug;
			showMenu = false;
		}
		if (Input.GetButtonUp("menu")) {
			showMenu = !showMenu;
			showDebug = false;
		}
		if (Input.GetButtonUp("rotate")) {
			MainCamera.transform.Rotate(Vector3.forward,90);
		}
		if (Input.GetButtonUp("quit")) {
			Application.Quit();
		}
	}

	/// <summary>
	/// Display the appropriate GUI elements or no elements, if neither debug nor menu should be shown.
	/// </summary>
	private void OnGUI()
	{

		if (showMenu) {

			GUILayout.BeginVertical();

			/*
			 * Port config
			 */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Listen to port:");
			string portInputField = GUILayout.TextField(portInput, GUILayout.Width(160));
			if (GUI.changed) {
				Debug.Log(portInputField);
				portInput = portInputField;
			}
			if (GUILayout.Button("Set")) {
				try {
					UDPListener.ReceivePort = int.Parse(portInput);
					UDPListener.StartListening();
				} catch (Exception e) {
					Debug.Log(e.Message);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(10f);

			/*
			 * Background brightness
			 */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Background brightness:");
			if (GUILayout.Button("-")) {
				float newValue = Mathf.Max(0f, MainCamera.backgroundColor.r - 0.1f);
				MainCamera.backgroundColor = new Color(newValue, newValue, newValue);
			}
			if (GUILayout.Button("+")) {
				float newValue = Mathf.Min(1f, MainCamera.backgroundColor.r + 0.1f);
				MainCamera.backgroundColor = new Color(newValue, newValue, newValue);
			}
			GUILayout.EndHorizontal();

			/*
			 * FOV
			 */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Scale:"); // Actually Field of View - Buttons are labeled counter-intuitive for FOV
			if (GUILayout.Button("-")) {
				MainCamera.fieldOfView = Mathf.Min(16f, MainCamera.fieldOfView += 0.25f);
			}
			if (GUILayout.Button("+")) {
				MainCamera.fieldOfView = Mathf.Max(1f, MainCamera.fieldOfView -= 0.25f);
			}
			GUILayout.EndHorizontal();

			/*
			 * Y position
			 */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Y position:"); // Actually Field of View - Buttons are labeled counter-intuitive for FOV
			if (GUILayout.Button("-")) {
				MainCamera.transform.Translate(Vector3.down*0.2f);
			}
			if (GUILayout.Button("+")) {
				MainCamera.transform.Translate(Vector3.up*0.2f);
			}
			GUILayout.EndHorizontal();

			/*
			 * Rotation
			 */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Rotate camera:");
			if (GUILayout.Button("left")) {
				MainCamera.transform.Rotate(Vector3.forward,-90);
			}
			if (GUILayout.Button("right")) {
				MainCamera.transform.Rotate(Vector3.forward,90);
			}
			GUILayout.EndHorizontal();

			/*
			 * Mirror gaze
			 */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Mirror gaze: ");
			if (GUILayout.Button("yes")) {
				FaceAnim.MirrorGaze = true;
			}
			if (GUILayout.Button("no")) {
				FaceAnim.MirrorGaze = false;
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

		} else if (showDebug) {

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

		GUI.Label(new Rect(0,Screen.height-20,Screen.width,20), "Emofani build 20150711");
	}

}
