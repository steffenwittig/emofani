using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Creates a thread to listen to UDP messages on a specified port
/// </summary>
public class UdpListener : EmofaniGlobal
{
	
	private Thread listenerThread;
	private UdpClient listener;
	private int receivePort = 11000;
	private List<string> messages;

	/// <summary>
	/// Gets or sets the port to listen on
	/// </summary>
	/// <value>The receive port.</value>
	public int ReceivePort {
		get {
			return this.receivePort;
		}
		set {
			if (value != receivePort) {
				receivePort = value;
				if (this.Listening) {
					Close();
					StartListening();
				}
			}
		}
	}

	private bool Listening {
		get {
			return listenerThread != null && listenerThread.IsAlive;
		}
	}

	/// <summary>
	/// Initiate a new thread to listen for UDP messages. Terminate the old listener thread if there is one.
	/// </summary>
	public void StartListening()
	{
		
		Debug.Log("Starting UDP Listener on Port " + receivePort);
		
		if (listenerThread != null && listenerThread.IsAlive) {
			Close();
		}
		
		listenerThread = new Thread(
			new ThreadStart(Listen));
		
		listenerThread.IsBackground = true;
		listenerThread.Start();
		
	}


	/// <summary>
	/// Start this instance. Automatically begins to listen to the default port.
	/// </summary>
	private void Start()
	{
		
		Application.runInBackground = true;
		
		messages = new List<string>();
		
		StartListening();
	}

	/// <summary>
	/// Check if there are messages in the cue. If there are, send them to the FaceAnimator object's
	/// HandleMessages() function.
	/// </summary>
	private void Update()
	{

		if (messages.Count > 0) {

			try {
				// try block prevents dropping of messages if something goes wrong

				foreach (string message in messages) {
					GuiScript.Log(message);
					FaceAnim.HandleMessage(message);
				}

				// clears only if no errors occured
				messages.Clear();
				Debug.Log("Messages cleared");

			} catch (Exception e) {
				Debug.Log(e.Message + ":" + e.StackTrace);
			}
		}

	}

	/// <summary>
	/// Method to be used in the listener threads. Waits for UDP messages on the previously specified port.
	/// If a message arrives, it will be put into the messages cue
	/// </summary>
	private void Listen()
	{
		try {
			listener = new UdpClient(receivePort);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, receivePort);

			while (true) {
				byte[] bytes = listener.Receive(ref groupEP);
				string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
				Debug.Log(message);
				messages.Add(message);
			}
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}

	}

	/// <summary>
	/// Close this instance. Aborts the current listener thread.
	/// </summary>
	private void Close()
	{
		try {
			if (listenerThread != null && listenerThread.IsAlive) {
				Debug.Log("Kill listener thread.");
				listenerThread.Abort();
			}
			listener.Close();
		} catch (Exception e) {
			Debug.Log(e.Message);
		}
	}

	/// <summary>
	/// Makes sure that the listener thread is closed. Otherwise the program actually crashes when quitting which
	/// is really annoying in the Unity3D editor, as this causes it to freeze.
	/// </summary>
	private void OnDestroy()
	{
		Close();
	}
    
}
