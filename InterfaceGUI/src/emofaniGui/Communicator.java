package emofaniGui;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;

/**
 * Singleton that allows to send and listen for UDP messages. Ports and host addresses will be read from the MainWindow
 * Singleton.
 * @author Steffen Wittig
 *
 */
public class Communicator {

	private static Communicator instance;
	private DatagramSocket server = null;

	/**
	 * Constructor is private as this is a singleton
	 */
	private Communicator() {			
		
	}
	
	/**
	 * Get an instance of the singleton (or create it)
	 * @return Communicator
	 */
	public static Communicator getInstance() {
		if (instance == null){
			instance = new Communicator();
		}
		return instance;
	}
	
	/**
	 * Listen for UDP messages on the port specified in MainWindow
	 */
	public void listen(){
		MainWindow mw = MainWindow.getInstance();
		try {
			DatagramPacket packet = new DatagramPacket(new byte[1024],1024);
			server = new DatagramSocket(mw.getReceivePort());
			server.setSoTimeout(mw.getTimeout());
			server.receive(packet);
			String data = new String(packet.getData()).trim();
			mw.print("In: \"" + data + "\"");
			mw.update(data);
		} catch (SocketTimeoutException e) {
			mw.print("Timeout: Emofani didn't answer in time.");
		}catch (IOException e) {
			mw.print("Error: " + e.getMessage());
		} finally {
			server.close();
		}
	}
	
	/**
	 * Send an UDP messages containing a parameter and value to the host and port specified in MainWindow
	 * @param param The name of the parameter, e.g. "expression"
	 * @param value The value of the parameter, e.g. "happy%50"
	 */
	public void send(String param, String value){
		
		MainWindow mw = MainWindow.getInstance();
		
		// message format: 
		// t:<timestamp>;s:<source>;p:<port>;d:<parameter>=<value>
		String message = "t:" + System.currentTimeMillis() + ";";

		try {
			message += "s:" + InetAddress.getLocalHost().getHostAddress() + ";";
		} catch (UnknownHostException e1) {
			message += "s:localhost;";
		}
		message += "p:" + mw.getReceivePort() + ";";
		message += "d:" + param + "=" + value;
		DatagramSocket s = null;
		
		try {
			DatagramPacket p = new DatagramPacket(
					message.getBytes(),
					message.length(),
					InetAddress.getByName(mw.getHost()),
					mw.getSendPort());
			s = new DatagramSocket();
			s.send(p);
			mw.print("Out: \"" + message + "\" to " + mw.getHost() + " on port " + mw.getSendPort());
		} catch (IOException e) {
			mw.print("Error: " + e.getMessage());
		} finally {
			try {
				s.close();
			} catch (Exception e) {
				mw.print("Error: " + e.getMessage()); 
			}
		}
		
		// listen for OK
		listen();

	}

	/**
	 * Closes the UDP connection.
	 */
	protected void finalize() {
		server.close();
	}
	
}
