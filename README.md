# emofani
**E**motion **Mo**del based **F**ace **Ani**mation

emofani was created as part of my Bachelor's Thesis *Parameterized Facial Animation for Human Robot Interaction* and is a simple program to render a lively animated face to enhance human-robot-interaction. emofani was formerly called *ParamFace*.

![emofani output](http://steffenwittig.com/wp-content/uploads/2015/07/emofani-v0.21.jpg "emofani output")

The displayed expression and other parameters can be controlled through interprocess communication, using the UDP protocol. The virtual face and its expressions are designed to raise the willingness of potential users to engage in interaction with a robotic system. The face displays signs of "being alive" (blinking, breathing, micro-movements) to show that it is active.

![emofani architecture](http://steffenwittig.com/wp-content/uploads/2015/07/emofani_architecture.png "emofani architecture")

emofani's expression parameters are based on James A. Russell's [*circumplex model of emotion*](https://en.wikipedia.org/wiki/Emotion_classification#Circumplex_model), a two-dimensional emotion model. The widely used [*FACS*](https://en.wikipedia.org/wiki/Facial_Action_Coding_System) system would be richer but less intuitive to use. However, using FACS values internally would help to create more precise expressions, instead of the current expressions, that are designed by heart. This is on the TODO list.

![emofani emotion space](http://steffenwittig.com/wp-content/uploads/2015/07/emofani_emotion_space.png "emofani emotion space")

I have created this repository to share emofani, to find collaborators to extend its features ([feel free to contact me](mailto:emofani@steffenwittig.com)), and hopefully turn it into a useful and professional software for human-robot-interaction.

You can find a bit more information and a video here: [http://steffenwittig.com/bsc-thesis](http://steffenwittig.com/bsc-thesis)

## Projects
The Unity engine was used to implement emofani. All project files reside in the "emofaniUnity" folder. A simple GUI to test emofani's UDP interface was implemented in Java 1.7 and can be found in the "InterfaceGUI" folder.

## UDP Interface
emofani listens for UDP messages (default port: 11000, can be changed in the options menu) and answers with status messages on the port specified in the message.

### To emofani

	t:[timestamp];s:[source];p:[port];d:[data]

- `[timestamp]`: An arbitrary integer that should be incremented with every message. ParamFace will use this number to detect when a message arrives out of order (smaller number than the last package) and ignores out of order messages. This is done to prevent accidental overriding of parameters that arrive out of order (which can happen if messages are send from another machine in the network).

- `[source]`: The IP address where ParamFace should send status messages to. The status
messages will contain the current state of all parameters or error messages

- `[port]`: The port of the source where ParamFace should send status messages to

- `[data]`: A string in the following format `[key]=[value]`

- `[key]`: The name of the parameter for which data will follow. This can be one of the following: `arousal`, `pleasure`, `gazex`, `gazey`, `gazez`, `expression`, `talking`, `action`

- `[values]`: An integer or string with the value for the given parameter.

If the key is `expression`, the value has to be an expression label followed by a % character followed by a value between 0 and 100 specifying the intensity of the expression.

Examples:

	t:1424766709104;s:192.168.2.145;p:11001;d:expression=happy%77
	t:1424766710790;s:192.168.2.145;p:11001;d:arousal=54
	t:1424766713509;s:192.168.2.145;p:11001;d:pleasure=-25
	t:1424766715566;s:192.168.2.145;p:11001;d:gazex=57
	t:1424766717783;s:192.168.2.145;p:11001;d:talking=true

Note: The gaze coordinates are referring to a point in space, that is relative to the center of the character's eyes, measured in centimeters. The character's head height is 25cm high and 20cm wide. Here's a diagram:

![emofani's gaze coordinate system](http://steffenwittig.com/wp-content/uploads/2015/07/emofani_gaze_coordinates.png "emofani's gaze coordinate system")

### From emofani

    status:[status];arousal:[arousal];pleasure:[pleasure];gazex:[gazex];gazey:[gazey];gazez:[gazez];talking:[talking];error:[error]

- `[status]`: `OK` or `ERROR`. If there was a problem, `[error]` will contain a description of the error. Otherwise the
following information can be found in the status message.

- `[arousal]`: An integer between -100 and 100, the currently set value of the `arousal` variable
- `[pleasure]`: An integer between -100 and 100, the currently set value of the `pleasure` variable
- `[gazex]`: An integer between -100 and 100 (current x-axis distance in centimeters)
- `[gazey]`: An integer between -100 and 100 (current y-axis distance in centimeters)
- `[gazez]`: An integer between -100 and 100 (current z-axis distance in centimeters)
- `[talking]`: 0 or 1

## Face design

emofani currently features a very simple, low-poly female face design. There are some minor issues with the eyebrows and eyes, but it gets the job done. Reworking it is on my TODO list. The character's name is Stefanie, by the way.

# TODO
To let you know about my plans and what is bugging me about emofani, here's my TODO list:

- implement options to control the amount of head movement for gaze changess
- save emofani's configuration in a config file to allow editing outside of emofani and storing values for the next start of emofani
- rework emofani's user interface
- implement TCP interface
- implement FACS instead of fixed expressions, designed by hand
- implement a more versatile face design, that can be parameterized to create trustworthy and sympathetic faces for users with varying cultural backgrounds (the current face design was mainly evaluated with German students)
- implement text-to-speech solution and phoneme based lip animation
- the `exited` expression looks wrong when `talking` is set to true
- implement measures for back-projection systems to correct the keystone effect and possibly other distortions