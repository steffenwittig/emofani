package emofaniGui;

import java.awt.Container;
import java.awt.Dimension;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.io.BufferedInputStream;
import java.io.InputStream;

import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.swing.*;

/**
 * Singleton that contains the scenario control elements to send messages to
 * Emofani. The buttons in this window will send a set of messages to ParamFace
 * and will cause the playback of sound files which will lock up the window
 * until the playback's finished
 * 
 * @author Steffen Wittig
 */
public class ScenarioWindow extends JFrame {

	private static final long serialVersionUID = 1L;
	private static ScenarioWindow instance;
	private Clip clip;

	/**
	 * creates ScenarioWindow. Puts all the necessary buttons into a GridBag
	 * layout.
	 */
	private ScenarioWindow() {

		/*
		 * Dimensions
		 */
		Dimension size = new Dimension(800, 600);
		this.setSize(size);
		this.setMinimumSize(size);

		/*
		 * create Layout, add Control and Communicator panels
		 */
		Container cpane = this.getContentPane();
		cpane.setLayout(new GridBagLayout());

		GridBagConstraints c = new GridBagConstraints();
		c.weightx = 1;
		c.fill = GridBagConstraints.BOTH;
		c.gridx = GridBagConstraints.REMAINDER;

		c.weighty = 1;
		cpane.add(new JLabel("Emofani Wizard of Oz"), c);

		JPanel buttonPanel = new JPanel();
		buttonPanel.setLayout(new GridLayout(4, 1));
		addActionButton("Introduction", buttonPanel);
		addActionButton("What are you looking for", buttonPanel);
		addActionButton("Did not understand", buttonPanel);
		addActionButton("OK and follow", buttonPanel);
		addActionButton("Look here", buttonPanel);

		c.weighty = 6;
		cpane.add(buttonPanel, c);

		this.setVisible(true);
	}

	/**
	 * Returns instance of Scenario or creates it if none exists
	 * 
	 * @return instance
	 */
	public static ScenarioWindow getInstance() {
		if (instance == null) {
			instance = new ScenarioWindow();
		}
		return instance;
	}

	/**
	 * React to an action string passed to this method
	 * 
	 * @param action
	 *            Action to be performed.
	 */
	private void performAction(String action) {

		switch (action) {
		case "Introduction":
			performIntroduction();
			break;
		case "What are you looking for":
			performQuestion();
			break;
		case "Did not understand":
			performError();
			break;
		case "OK and follow":
			performFollow();
			break;
		case "Look here":
			performPoint();
			break;
		}

	}

	/**
	 * ask what the user is looking for
	 */
	private void performQuestion() {
		Communicator.getInstance().send("expression", "attentive%100");
		playMessage("/message_whatareyoulookingfor.wav");
	}

	/**
	 * look at article
	 */
	private void performPoint() {
		Communicator.getInstance().send("expression", "happy%100");
		Communicator.getInstance().send("gazex", "130");
		Communicator.getInstance().send("gazey", "-80");
		playMessage("/message_lookhere.wav");
		try {
			Thread.sleep(500);
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
		Communicator.getInstance().send("expression", "happy%50");
		Communicator.getInstance().send("gazex", "0");
		Communicator.getInstance().send("gazey", "0");
	}

	/**
	 * ask user to follow
	 */
	private void performFollow() {
		Communicator.getInstance().send("expression", "happy%80");
		playMessage("/message_followme.wav");
		Communicator.getInstance().send("expression", "relaxed%60");
	}

	/**
	 * perform the question action
	 */
	private void performError() {
		Communicator.getInstance().send("expression", "sad%100");
		playMessage("/message_didnotunderstand.wav");
		Communicator.getInstance().send("expression", "sad%60");
	}

	/**
	 * introduce the robot
	 */
	private void performIntroduction() {
		Communicator.getInstance().send("expression", "exited%50");
		playMessage("/message_hello.wav");
		Communicator.getInstance().send("expression", "happy%40");
	}

	/**
	 * Try to play the specified sound file. Set "talking" parameter to true
	 * before starting the clip and set it to false once the clip has finished
	 * 
	 * @param file
	 *            name of the soundfile in the /res folder
	 */
	private void playMessage(String file) {
		try {
			InputStream in = getClass().getResourceAsStream(file);
			InputStream bufferedIn = new BufferedInputStream(in);
			AudioInputStream sound = AudioSystem
					.getAudioInputStream(bufferedIn);
			clip = AudioSystem.getClip();
			clip.open(sound);
			clip.setFramePosition(0);

			Communicator.getInstance().send("talking", "true");
			clip.start();
			Thread.sleep(clip.getMicrosecondLength() / 1000);
			Communicator.getInstance().send("talking", "false");
		} catch (Exception e) {
			MainWindow.getInstance().print(e.getClass() + ":" + e.getMessage());
		}
	}

	/**
	 * Create a button for the specified action and add it to the specified
	 * panel. On clicking the button the performAction() method will be called
	 * with the text of the button as parameter
	 * 
	 * @param action
	 *            name of the the action to be performed
	 * @param panel
	 *            panel to which the button shall be added
	 * @return Button object
	 */
	private JButton addActionButton(String action, JPanel panel) {

		JButton button = new JButton(action);

		button.addMouseListener(new MouseListener() {

			@Override
			public void mouseReleased(MouseEvent e) {
				JButton b = (JButton) e.getSource();
				ScenarioWindow.getInstance().performAction(b.getText());
			}

			@Override
			public void mousePressed(MouseEvent e) {
				// TODO Auto-generated method stub

			}

			@Override
			public void mouseExited(MouseEvent e) {
				// TODO Auto-generated method stub

			}

			@Override
			public void mouseEntered(MouseEvent e) {
				// TODO Auto-generated method stub

			}

			@Override
			public void mouseClicked(MouseEvent e) {
				// TODO Auto-generated method stub

			}
		});

		panel.add(button);
		return button;
	}

}
