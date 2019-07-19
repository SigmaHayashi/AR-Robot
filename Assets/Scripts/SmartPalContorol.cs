﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GoogleARCore;

public class SmartPalContorol : MonoBehaviour {

	private GameObject coordinates_adapter;

	private GameObject right_arm;
	private GameObject left_arm;

	//public Canvas ButtonCanvas;
	private Canvas ControllCanvas;

	public Text CameraPositionText;
	public Text SmartPalPositionText;

	private DebugText debug;

	/*
	private Button PosXPlusButton;
	private Button PosXMinusButton;
	private Button PosYPlusButton;
	private Button PosYMinusButton;
	private Button PosZPlusButton;
	private Button PosZMinusButton;

	private Button LeftButton;
	private Button RightButton;

	private Button ArmUpButton;
	private Button ArmDownButton;
	*/
	private Button RightArmUpButton;
	private Button RightArmDownButton;
	private Button LeftArmUpButton;
	private Button LeftArmDownButton;
	private Button MoveForwardButton;
	private Button MoveBackButton;
	private Button TurnRightButton;
	private Button TurnLeftButton;

	/*
	private bool push_x_plus = false;
	private bool push_x_minus = false;
	private bool push_y_plus = false;
	private bool push_y_minus = false;
	private bool push_z_plus = false;
	private bool push_z_minus = false;
	private bool push_left = false;
	private bool push_right = false;
	private bool push_arm_up = false;
	private bool push_arm_down = false;
	*/
	private bool push_right_arm_up = false;
	private bool push_right_arm_down = false;
	private bool push_left_arm_up = false;
	private bool push_left_arm_down = false;
	private bool push_move_forward = false;
	private bool push_move_back = false;
	private bool push_turn_right = false;
	private bool push_turn_left = false;

	private int init_state = 0;
	private RobotColorController ColorController;

	// Start is called before the first frame update
	void Start() {
		debug = GameObject.Find("Main System/Text Canvas/Debug Text").GetComponent<DebugText>();
		
		GameObject prefab = (GameObject)Resources.Load("Coordinates Adapter");
		coordinates_adapter = (GameObject)Instantiate(prefab, this.transform);
		coordinates_adapter.transform.parent = this.transform;

		right_arm = GameObject.Find("r_arm_j1_link");
		left_arm = GameObject.Find("l_arm_j1_link");

		/*
		PosXPlusButton = GameObject.Find("Main System/Button Canvas/X Plus Button").GetComponent<Button>();
		PosXMinusButton = GameObject.Find("Main System/Button Canvas/X Minus Button").GetComponent<Button>();
		PosYPlusButton = GameObject.Find("Main System/Button Canvas/Y Plus Button").GetComponent<Button>();
		PosYMinusButton = GameObject.Find("Main System/Button Canvas/Y Minus Button").GetComponent<Button>();
		PosZPlusButton = GameObject.Find("Main System/Button Canvas/Z Plus Button").GetComponent<Button>();
		PosZMinusButton = GameObject.Find("Main System/Button Canvas/Z Minus Button").GetComponent<Button>();
		LeftButton = GameObject.Find("Main System/Button Canvas/Left Button").GetComponent<Button>();
		RightButton = GameObject.Find("Main System/Button Canvas/Right Button").GetComponent<Button>();
		ArmUpButton = GameObject.Find("Main System/Button Canvas/Arm Up Button").GetComponent<Button>();
		ArmDownButton = GameObject.Find("Main System/Button Canvas/Arm Down Button").GetComponent<Button>();

		AddTrigger(PosXPlusButton);
		AddTrigger(PosXMinusButton);
		AddTrigger(PosYPlusButton);
		AddTrigger(PosYMinusButton);
		AddTrigger(PosZPlusButton);
		AddTrigger(PosZMinusButton);
		AddTrigger(LeftButton);
		AddTrigger(RightButton);
		AddTrigger(ArmUpButton);
		AddTrigger(ArmDownButton);
		*/
		RightArmUpButton = GameObject.Find("Main System/Robot Controll Canvas/Right Arm Up Button").GetComponent<Button>();
		RightArmDownButton = GameObject.Find("Main System/Robot Controll Canvas/Right Arm Down Button").GetComponent<Button>();
		LeftArmUpButton = GameObject.Find("Main System/Robot Controll Canvas/Left Arm Up Button").GetComponent<Button>();
		LeftArmDownButton = GameObject.Find("Main System/Robot Controll Canvas/Left Arm Down Button").GetComponent<Button>();
		MoveForwardButton = GameObject.Find("Main System/Robot Controll Canvas/Move Forward Button").GetComponent<Button>();
		MoveBackButton = GameObject.Find("Main System/Robot Controll Canvas/Move Back Button").GetComponent<Button>();
		TurnRightButton = GameObject.Find("Main System/Robot Controll Canvas/Turn Right Button").GetComponent<Button>();
		TurnLeftButton = GameObject.Find("Main System/Robot Controll Canvas/Turn Left Button").GetComponent<Button>();

		AddTrigger(RightArmUpButton);
		AddTrigger(RightArmDownButton);
		AddTrigger(LeftArmUpButton);
		AddTrigger(LeftArmDownButton);
		AddTrigger(MoveForwardButton);
		AddTrigger(MoveBackButton);
		AddTrigger(TurnRightButton);
		AddTrigger(TurnLeftButton);

		/*
		ButtonCanvas = GameObject.Find("Main System/Button Canvas").GetComponent<Canvas>();
		ButtonCanvas.gameObject.SetActive(false);
		*/
		ControllCanvas = GameObject.Find("Main System/Robot Controll Canvas").GetComponent<Canvas>();
		ControllCanvas.gameObject.SetActive(false);

		ColorController = transform.GetComponent<RobotColorController>();
	}

	// Update is called once per frame
	void Update() {
		CameraPositionText.text = "Camra : " + Camera.main.transform.position.ToString("f2") + Camera.main.transform.eulerAngles.ToString("f2");
		SmartPalPositionText.text = "SmartPal : " + transform.position.ToString("f2") + transform.eulerAngles.ToString("f2");

		RobotInit();

		if(init_state >= 2) {
			ButtonControl();
		}

		//debug.ClearDebug();
		//debug.Debug(left_arm.transform.localRotation.eulerAngles.y.ToString());
	}

	void RobotInit() {
		switch (init_state) {
			case 0:
			ColorController.robot_alpha = 0.0f;
			ColorController.ChangeRobotColors(ColorController.safety_color);
			init_state = 1;
			break;

			case 1:
			List<DetectedPlane> planes = new List<DetectedPlane>();
			Session.GetTrackables<DetectedPlane>(planes, TrackableQueryFilter.All);
			foreach(DetectedPlane plane in planes) {
				if(plane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing) {
					transform.position = plane.CenterPose.position;

					Vector3 camera_pos = Camera.main.transform.position;
					Vector3 robot_pos = transform.position;
					float rot2robot = Mathf.Atan2((robot_pos.x - camera_pos.x) * -1, robot_pos.z - camera_pos.z);
					rot2robot = rot2robot / Mathf.PI * 180.0f;
					Vector3 robot_euler = transform.eulerAngles;
					robot_euler.y -= rot2robot;
					transform.eulerAngles = robot_euler;

					ColorController.robot_alpha = 1.0f;
					ColorController.ChangeRobotColors(ColorController.safety_color);
					//ButtonCanvas.gameObject.SetActive(true);
					ControllCanvas.gameObject.SetActive(true);
					init_state = 2;
					break;
				}
			}
			break;
		}

	}

	void AddTrigger(Button button) {

		EventTrigger trigger = button.GetComponent<EventTrigger>();
		EventTrigger.Entry entry_down = new EventTrigger.Entry();
		entry_down.eventID = EventTriggerType.PointerDown;
		EventTrigger.Entry entry_up = new EventTrigger.Entry();
		entry_up.eventID = EventTriggerType.PointerUp;
		switch (button.name.ToString()) {
			/*
			case "X Plus Button":
			entry_down.callback.AddListener((x) => { push_x_plus = true; });
			entry_up.callback.AddListener((x) => { push_x_plus = false; });
			break;
			case "X Minus Button":
			entry_down.callback.AddListener((x) => { push_x_minus = true; });
			entry_up.callback.AddListener((x) => { push_x_minus = false; });
			break;
			case "Y Plus Button":
			entry_down.callback.AddListener((x) => { push_y_plus = true; });
			entry_up.callback.AddListener((x) => { push_y_plus = false; });
			break;
			case "Y Minus Button":
			entry_down.callback.AddListener((x) => { push_y_minus = true; });
			entry_up.callback.AddListener((x) => { push_y_minus = false; });
			break;
			case "Z Plus Button":
			entry_down.callback.AddListener((x) => { push_z_plus = true; });
			entry_up.callback.AddListener((x) => { push_z_plus = false; });
			break;
			case "Z Minus Button":
			entry_down.callback.AddListener((x) => { push_z_minus = true; });
			entry_up.callback.AddListener((x) => { push_z_minus = false; });
			break;
			case "Left Button":
			entry_down.callback.AddListener((x) => { push_left = true; });
			entry_up.callback.AddListener((x) => { push_left = false; });
			break;
			case "Right Button":
			entry_down.callback.AddListener((x) => { push_right = true; });
			entry_up.callback.AddListener((x) => { push_right = false; });
			break;
			case "Arm Up Button":
			entry_down.callback.AddListener((x) => { push_arm_up = true; });
			entry_up.callback.AddListener((x) => { push_arm_up = false; });
			break;
			case "Arm Down Button":
			entry_down.callback.AddListener((x) => { push_arm_down = true; });
			entry_up.callback.AddListener((x) => { push_arm_down = false; });
			break;
			*/
			case "Right Arm Up Button":
			entry_down.callback.AddListener((x) => { push_right_arm_up = true; });
			entry_up.callback.AddListener((x) => { push_right_arm_up = false; });
			break;
			case "Right Arm Down Button":
			entry_down.callback.AddListener((x) => { push_right_arm_down = true; });
			entry_up.callback.AddListener((x) => { push_right_arm_down = false; });
			break;
			case "Left Arm Up Button":
			entry_down.callback.AddListener((x) => { push_left_arm_up = true; });
			entry_up.callback.AddListener((x) => { push_left_arm_up = false; });
			break;
			case "Left Arm Down Button":
			entry_down.callback.AddListener((x) => { push_left_arm_down = true; });
			entry_up.callback.AddListener((x) => { push_left_arm_down = false; });
			break;
			case "Move Forward Button":
			entry_down.callback.AddListener((x) => { push_move_forward = true; });
			entry_up.callback.AddListener((x) => { push_move_forward = false; });
			break;
			case "Move Back Button":
			entry_down.callback.AddListener((x) => { push_move_back = true; });
			entry_up.callback.AddListener((x) => { push_move_back = false; });
			break;
			case "Turn Right Button":
			entry_down.callback.AddListener((x) => { push_turn_right = true; });
			entry_up.callback.AddListener((x) => { push_turn_right = false; });
			break;
			case "Turn Left Button":
			entry_down.callback.AddListener((x) => { push_turn_left = true; });
			entry_up.callback.AddListener((x) => { push_turn_left = false; });
			break;
		}

		trigger.triggers.Add(entry_down);
		trigger.triggers.Add(entry_up);
	}

	void ButtonControl() {
		/*
		if (push_x_plus) {
			Vector3 tmp_position = new Vector3(1.0f * Time.deltaTime, 0, 0);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			this.transform.position = tmp_position;
		}

		if (push_x_minus) {
			Vector3 tmp_position = new Vector3(-1.0f * Time.deltaTime, 0, 0);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			this.transform.position = tmp_position;
		}

		if (push_y_plus) {
			Vector3 tmp_position = new Vector3(0, 1.0f * Time.deltaTime, 0);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			this.transform.position = tmp_position;
		}

		if (push_y_minus) {
			Vector3 tmp_position = new Vector3(0, -1.0f * Time.deltaTime, 0);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			this.transform.position = tmp_position;
		}

		if (push_z_plus) {
			Vector3 tmp_position = new Vector3(0, 0, 1.0f * Time.deltaTime);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			this.transform.position = tmp_position;
		}

		if (push_z_minus) {
			Vector3 tmp_position = new Vector3(0, 0, -1.0f * Time.deltaTime);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			this.transform.position = tmp_position;
		}

		if (push_left) {
			this.transform.eulerAngles += new Vector3(0, -45.0f * Time.deltaTime, 0);
		}

		if (push_right) {
			this.transform.eulerAngles += new Vector3(0, 45.0f * Time.deltaTime, 0);
		}

		if (push_arm_up) {
			left_arm.transform.localRotation *= Quaternion.Euler(0, -30.0f * Time.deltaTime, 0);
		}

		if (push_arm_down) {
			left_arm.transform.localRotation *= Quaternion.Euler(0, 30 * Time.deltaTime, 0);
		}
		*/

		if (push_right_arm_up) {
			right_arm.transform.localRotation *= Quaternion.Euler(0, -30.0f * Time.deltaTime, 0);
		}

		if (push_right_arm_down) {
			right_arm.transform.localRotation *= Quaternion.Euler(0, 30.0f * Time.deltaTime, 0);
		}

		if (push_left_arm_up) {
			left_arm.transform.localRotation *= Quaternion.Euler(0, -30.0f * Time.deltaTime, 0);
		}

		if (push_left_arm_down) {
			left_arm.transform.localRotation *= Quaternion.Euler(0, 30.0f * Time.deltaTime, 0);
		}

		if (push_move_forward) {
			Vector3 tmp_position = new Vector3(0, 0, 1.0f * Time.deltaTime);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			transform.position = tmp_position;
		}

		if (push_move_back) {
			Vector3 tmp_position = new Vector3(0, 0, -1.0f * Time.deltaTime);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			transform.position = tmp_position;
		}

		if (push_turn_right) {
			transform.eulerAngles += new Vector3(0, 45.0f * Time.deltaTime, 0);
		}

		if (push_turn_left) {
			transform.eulerAngles += new Vector3(0, -45.0f * Time.deltaTime, 0);
		}
	}
}
