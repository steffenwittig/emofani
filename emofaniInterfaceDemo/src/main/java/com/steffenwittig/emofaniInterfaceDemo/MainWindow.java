package com.steffenwittig.emofaniInterfaceDemo;

import java.awt.Container;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.util.Hashtable;

import javax.swing.*;

/**
 * Singleton that contains the main control elements to send messages to Emofani
 * 
 * @author Steffen Wittig
 */
public class MainWindow extends JFrame {

	private static final long serialVersionUID = 1;
	private static MainWindow instance;

	private static JTextArea textarea;
	private static JTextField hostField, sendPortField, receivePortField,
			timeoutField, talkField;

	private JSlider arousalSlider, pleasureSlider, gazeXSlider, gazeYSlider,
			gazeZSlider, expressionInstensitySlider;
	private JToggleButton talkingButton, idleButton;

	/**
	 * creates MainWindow. Puts Controls- and Communicator Panel in a two-column
	 * grid layout.
	 */
	private MainWindow() {

		/*
		 * Dimensions
		 */
		Dimension size = new Dimension(800, 600);
		this.setSize(size);
		this.setMinimumSize(size);

		/*
		 * close button listener
		 */
		this.addWindowListener(new java.awt.event.WindowAdapter() {
			@Override
			public void windowClosing(java.awt.event.WindowEvent windowEvent) {
				System.out.println("TryFace exit");
				System.exit(0);
			}
		});

		/*
		 * create Layout, add Control and Communicator panels
		 */
		Container cpane = this.getContentPane();
		cpane.setLayout(new GridLayout(1, 2));
		cpane.add(this.getControlsPanel());
		cpane.add(this.getCommunicatorPanel());

		this.setVisible(true);
	}

	/**
	 * Calls getInstance but does not return the instance
	 */
	public static void create() {
		MainWindow.getInstance();
	}

	/**
	 * Returns instance of MainWindow or creates it if none exists
	 * 
	 * @return instance
	 */
	public static MainWindow getInstance() {
		if (instance == null) {
			instance = new MainWindow();
		}
		return instance;
	}

	/**
	 * Appends a new line to the UDP log textarea
	 * 
	 * @param message
	 *            The content of the new line
	 */
	public void print(String message) {
		if (textarea != null) {
			textarea.append(message + "\n");
		}
	}

	/**
	 * get value of the expression intensity slider
	 * 
	 * @return the value
	 */
	public int getExpressionInstensity() {
		return expressionInstensitySlider.getValue();
	}

	public String getTalkMessage() {
		return talkField.getText();
	}

	/**
	 * Interprets a response string received from the Communicator class and
	 * updates the values of all sliders as well as the talking and idle
	 * switches with the contained values
	 * 
	 * @param data
	 *            response string received via UDP
	 */
	public void update(String data) {

		Hashtable<String, String> keyValues = new Hashtable<String, String>();

		// try to interpret the message and put the parameters into an array
		try {

			String[] params = data.split(";");
			for (String param : params) {
				String[] keyValue = param.split(":");
				keyValues.put(keyValue[0], keyValue[1]);
			}

		} catch (Exception e) {
			print("Error while interpreting a received message.");
		}

		// try to set the contained values to the parameters
		try {

			arousalSlider.setValue(Integer.parseInt(keyValues.get("arousal")));
			pleasureSlider
					.setValue(Integer.parseInt(keyValues.get("pleasure")));
			gazeXSlider.setValue(Integer.parseInt(keyValues.get("gazex")));
			gazeYSlider.setValue(Integer.parseInt(keyValues.get("gazey")));
			gazeZSlider.setValue(Integer.parseInt(keyValues.get("gazez")));
			System.out.println("Incoming:-" + keyValues.get("talking") + "-");
			talkingButton.setSelected(keyValues.get("talking")
					.equalsIgnoreCase("true"));
			idleButton.setSelected(keyValues.get("idle").equalsIgnoreCase(
					"true"));

		} catch (Exception e) {
			print("Received message was missing at least one parameter: "
					+ e.getMessage());
		}
	}

	/**
	 * returns the content of the host field. This field contains the address or
	 * name of the host where Emofani is running
	 * 
	 * @return string with host's name or address
	 */
	public String getHost() {
		return hostField.getText();
	}

	/**
	 * returns content of the timeout field. This field is used by the
	 * Communicator class to determine how long it should lock up (it's not
	 * multi threaded, yet) before it stops listening
	 * 
	 * @return integer for timeout in milliseconds
	 */
	public int getTimeout() {
		try {
			return Integer.parseInt(timeoutField.getText());
		} catch (Exception e) {
			print("Timeout Input Error: " + e.getMessage());
			return 0;
		}
	}

	/**
	 * returns the content of the output port field. This field contains the
	 * port number of the host where Emofani is running
	 * 
	 * @return integer for port number
	 */
	public int getSendPort() {
		try {
			return Integer.parseInt(sendPortField.getText());
		} catch (Exception e) {
			print("Port Input Error: " + e.getMessage());
			return -1;
		}
	}

	/**
	 * returns the content of the input port field. This field contains the port
	 * number where Emofani should send status messages to
	 * 
	 * @return integer for port number
	 */
	public int getReceivePort() {
		try {
			return Integer.parseInt(receivePortField.getText());
		} catch (Exception e) {
			print("Port Input Error: " + e.getMessage());
			return -1;
		}
	}

	/**
	 * creates a panel which contains the expressions and slider panel
	 * 
	 * @return the panel
	 */
	private JPanel getControlsPanel() {
		JPanel panel = new JPanel();
		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));
		panel.add(this.getExpressionsPanel());
		panel.add(this.getTalkPanel());
		panel.add(this.getSliderPanel());
		return panel;
	}

	/**
	 * creates a panel which contains the UDP log and target and scenario panel
	 * 
	 * @return the panel
	 */
	private JPanel getCommunicatorPanel() {
		JPanel panel = new JPanel();
		panel.setLayout(new GridBagLayout());

		GridBagConstraints c = new GridBagConstraints();
		c.weightx = 1;
		c.fill = GridBagConstraints.BOTH;
		c.gridx = GridBagConstraints.REMAINDER;

		c.weighty = 24;
		panel.add(getUdpLogPanel(), c);

		c.weighty = 1;
		panel.add(getTargetPanel(), c);
		panel.add(getScenarioPanel(), c);

		return panel;
	}

	/**
	 * Creates the scenario panel which contains the "Open Scenario Window"
	 * button
	 * 
	 * @return the panel
	 */
	private JPanel getScenarioPanel() {

		JPanel panel = new JPanel();

		JButton button = new JButton("Open Scenario Window");
		button.addMouseListener(new MouseListener() {

			@Override
			public void mouseReleased(MouseEvent e) {
				ScenarioWindow.getInstance().setVisible(true);
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

		return panel;
	}

	/**
	 * Creates target panel, which contains input fields for host, output and
	 * input port, and timeout
	 * 
	 * @return the panel
	 */
	private JPanel getTargetPanel() {
		JPanel panel = new JPanel();
		panel.setBorder(BorderFactory.createTitledBorder("Target"));

		panel.setLayout(new GridLayout(1, 2));

		/*
		 * Host field
		 */
		hostField = new JTextField("localhost", 16);
		hostField.setBorder(BorderFactory.createTitledBorder("Host"));
		panel.add(hostField);

		/*
		 * SendPort field
		 */
		sendPortField = new JTextField("11000", 16);
		sendPortField
				.setBorder(BorderFactory.createTitledBorder("Output Port"));
		panel.add(sendPortField);

		/*
		 * ReceivePort field
		 */
		receivePortField = new JTextField("11001", 16);
		receivePortField.setBorder(BorderFactory
				.createTitledBorder("Input Port"));
		panel.add(receivePortField);

		/*
		 * Timeout field
		 */
		timeoutField = new JTextField("50", 3);
		timeoutField.setBorder(BorderFactory.createTitledBorder("Timeout"));
		panel.add(timeoutField);

		return panel;
	}

	/**
	 * Creates UDP log panel which contains a non editable textarea
	 * 
	 * @return the panel
	 */
	private JScrollPane getUdpLogPanel() {

		textarea = new JTextArea();
		textarea.setEditable(false);

		JScrollPane panel = new JScrollPane(textarea);
		panel.setBorder(BorderFactory.createTitledBorder("UDP Log"));

		return panel;
	}

	/**
	 * Creates a panel with sliders for arousal, pleasure and gaze coordinates
	 * 
	 * @return the panel
	 */
	private JPanel getSliderPanel() {

		/*
		 * create Emotion panel
		 */
		JPanel emotionPanel = new JPanel();
		emotionPanel.setBorder(BorderFactory
				.createTitledBorder("Emotion Parameters"));
		emotionPanel.setLayout(new BoxLayout(emotionPanel, BoxLayout.Y_AXIS));

		arousalSlider = addParamSlider("arousal", -100, 100, emotionPanel);
		pleasureSlider = addParamSlider("pleasure", -100, 100, emotionPanel);

		/*
		 * create Gaze panel
		 */
		JPanel gazePanel = new JPanel();
		gazePanel
				.setBorder(BorderFactory.createTitledBorder("Gaze Parameters"));
		gazePanel.setLayout(new BoxLayout(gazePanel, BoxLayout.Y_AXIS));
		gazeXSlider = addParamSlider("gazex", -200, 200, gazePanel);
		gazeYSlider = addParamSlider("gazey", -200, 200, gazePanel);
		gazeZSlider = addParamSlider("gazez", 0, 400, gazePanel);

		/*
		 * create Slider panel and add Gaze and Emotion panels
		 */
		JPanel panel = new JPanel();
		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));
		panel.add(emotionPanel);
		panel.add(gazePanel);

		return panel;
	}

	/**
	 * Creates a panel which contains buttons for all expressions as well as
	 * switches for the "talking" and "idle" parameters
	 * 
	 * @return the panel
	 */
	private JPanel getExpressionsPanel() {
		JPanel panel = new JPanel();
		panel.setBorder(BorderFactory.createTitledBorder("Expressions"));
		panel.setSize(new Dimension(100, 100));
		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));

		expressionInstensitySlider = addNiceSlider("intensity", 0, 100, panel);
		expressionInstensitySlider.setValue(100);

		JPanel buttons = new JPanel();
		buttons.setLayout(new GridLayout(3, 3));
		buttons.add(getExpressionButton("neutral"));
		buttons.add(getExpressionButton("happy"));
		buttons.add(getExpressionButton("sad"));
		buttons.add(getExpressionButton("attentive"));
		buttons.add(getExpressionButton("exited"));
		buttons.add(getExpressionButton("relaxed"));
		buttons.add(getExpressionButton("sleepy"));
		buttons.add(getExpressionButton("frustrated"));
		buttons.add(talkingButton = getToggleButton("talking"));
		buttons.add(idleButton = getToggleButton("idle"));

		panel.add(buttons);

		return panel;
	}

	/**
	 * Add a slider to a panel without having to write lots of lines of code
	 * 
	 * @param name
	 *            title of the slider
	 * @param min
	 *            lowest possible value
	 * @param max
	 *            highest possible value
	 * @param targetPanel
	 *            the panel to which the slider will be added
	 * @return reference to the slider object for setting up callbacks, etc.
	 */
	private JSlider addNiceSlider(String name, int min, int max,
			JPanel targetPanel) {
		JPanel panel = new JPanel();

		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));

		JLabel label = new JLabel(name);
		label.setAlignmentX(CENTER_ALIGNMENT);
		JSlider slider = new JSlider();

		slider.setName(name);
		slider.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
		slider.setMaximum(max);
		slider.setMinimum(min);
		slider.setValue(0);

		int range = max - min;
		if (range <= 40) {
			slider.setMajorTickSpacing(10);
			slider.setMinorTickSpacing(1);
		} else {
			slider.setMajorTickSpacing(range / 4);
		}
		slider.setPaintLabels(true);
		slider.setPaintTicks(true);

		panel.add(label);
		panel.add(slider);

		targetPanel.add(panel);

		return slider;
	}

	/**
	 * Add a parameter slider to a panel. Changing it will send a message where
	 * the name of the slider will be used as the parameter-key and its value as
	 * the parameter-value. MouseListener attached.
	 * 
	 * @param name
	 *            title of the slider
	 * @param min
	 *            lowest possible value
	 * @param max
	 *            highest possible value
	 * @param targetPanel
	 *            the panel to which the slider will be added
	 * @return reference to the slider object for setting up callbacks, etc.
	 */
	private JSlider addParamSlider(String name, int min, int max,
			JPanel targetPanel) {

		JSlider slider = addNiceSlider(name, min, max, targetPanel);

		slider.addMouseListener(new MouseListener() {

			@Override
			public void mouseReleased(MouseEvent e) {
				JSlider source = (JSlider) e.getSource();
				String value = String.valueOf(source.getValue());
				Communicator.getInstance().send(source.getName(), value);
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

		return slider;
	}

	/**
	 * Creates a new button and attaches a mouse listener that will send a
	 * message which uses the expression string as parameter-key and the value
	 * from the intensity slider as parameter-value
	 * 
	 * @param expression
	 *            the name of the expression
	 * @return the button
	 */
	private JButton getExpressionButton(String expression) {

		JButton button = new JButton(expression);

		button.addMouseListener(new MouseListener() {

			@Override
			public void mouseReleased(MouseEvent e) {
				JButton button = (JButton) e.getSource();
				MainWindow mw = MainWindow.getInstance();
				Communicator.getInstance().send("expression",
						button.getText() + "%" + mw.getExpressionInstensity());

			}

			@Override
			public void mouseClicked(MouseEvent e) {
				// no event
			}

			@Override
			public void mouseEntered(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseExited(MouseEvent e) {
				// no event

			}

			@Override
			public void mousePressed(MouseEvent e) {
				// no event

			}

		});

		return button;
	}

	/**
	 * Creates a simple ToggleButton which sends messages on change, containing
	 * the name of the button as parameter-key and its state (true/false) as
	 * parameter-key
	 * 
	 * @param name
	 *            the name of the parameter
	 * @return the button
	 */
	private JToggleButton getToggleButton(String name) {

		JToggleButton button = new JToggleButton(name);

		button.addMouseListener(new MouseListener() {

			@Override
			public void mouseReleased(MouseEvent e) {
				JToggleButton button = (JToggleButton) e.getSource();
				System.out.println(button.isSelected());
				Communicator.getInstance().send(button.getText(),
						String.valueOf(button.isSelected()));
			}

			@Override
			public void mousePressed(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseExited(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseEntered(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseClicked(MouseEvent e) {
				// no event

			}
		});

		return button;

	}

	private JPanel getTalkPanel() {
		JPanel panel = new JPanel();
		panel.setBorder(BorderFactory.createTitledBorder("Talk"));
		panel.setSize(new Dimension(100, 100));
		panel.setLayout(new BoxLayout(panel, BoxLayout.X_AXIS));

		talkField = new JTextField("message", 16);
		talkField.setBorder(BorderFactory.createTitledBorder("Message"));
		panel.add(talkField);

		JButton button = new JButton("say");

		button.addMouseListener(new MouseListener() {

			@Override
			public void mouseReleased(MouseEvent e) {
				String message = MainWindow.getInstance().getTalkMessage();
				message = message.toLowerCase();
				// ugh...
				// message = message.replaceAll("�", "ue");
				// message = message.replaceAll("�", "ae");
				// message = message.replaceAll("�", "oe");

				System.out.println("Say " + message);
				Communicator.getInstance().send("say", message);
			}

			@Override
			public void mousePressed(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseExited(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseEntered(MouseEvent e) {
				// no event

			}

			@Override
			public void mouseClicked(MouseEvent e) {
				// no event

			}
		});

		panel.add(button);

		return panel;
	}

}
