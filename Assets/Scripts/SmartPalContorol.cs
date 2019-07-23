using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GoogleARCore;

public class SmartPalContorol : MonoBehaviour {

	private MainScript Main;

	private GameObject coordinates_adapter;

	private GameObject right_arm;
	private GameObject left_arm;

	[NonSerialized] public GameObject arrow3D;
	[NonSerialized] public bool finish_arrow = false;

	//public Canvas ButtonCanvas;
	private Canvas ControllCanvas;

	public Text CameraPositionText;
	public Text SmartPalPositionText;

	private DebugText debug;
	
	private Button RightArmUpButton;
	private Button RightArmDownButton;
	private Button LeftArmUpButton;
	private Button LeftArmDownButton;
	private Button MoveForwardButton;
	private Button MoveBackButton;
	private Button TurnRightButton;
	private Button TurnLeftButton;

	private Button PosXPlusButton;
	private Button PosXMinusButton;
	private Button PosYPlusButton;
	private Button PosYMinusButton;
	private Button PosZPlusButton;
	private Button PosZMinusButton;
	private Button LeftButton;
	private Button RightButton;
	private Button ArmResetButton;

	private bool push_right_arm_up = false;
	private bool push_right_arm_down = false;
	private bool push_left_arm_up = false;
	private bool push_left_arm_down = false;
	private bool push_move_forward = false;
	private bool push_move_back = false;
	private bool push_turn_right = false;
	private bool push_turn_left = false;

	private bool push_x_plus = false;
	private bool push_x_minus = false;
	private bool push_y_plus = false;
	private bool push_y_minus = false;
	private bool push_z_plus = false;
	private bool push_z_minus = false;
	private bool push_left = false;
	private bool push_right = false;
	private bool push_arm_reset = false;

	private int init_state = 0;
	private RobotColorController ColorController;
	private float robot_alpha_default;

	// Start is called before the first frame update
	void Start() {
		Main = GameObject.Find("Main System").GetComponent<MainScript>();

		debug = GameObject.Find("Main System/Text Canvas/Debug Text").GetComponent<DebugText>();
		
		GameObject prefab = (GameObject)Resources.Load("Coordinates Adapter");
		coordinates_adapter = (GameObject)Instantiate(prefab, this.transform);
		coordinates_adapter.transform.parent = this.transform;

		right_arm = GameObject.Find("r_arm_j1_link");
		left_arm = GameObject.Find("l_arm_j1_link");

		arrow3D = GameObject.Find("3D Arrow");
		
		RightArmUpButton = GameObject.Find("Main System/Robot Controll Canvas/Right Arm Up Button").GetComponent<Button>();
		RightArmDownButton = GameObject.Find("Main System/Robot Controll Canvas/Right Arm Down Button").GetComponent<Button>();
		LeftArmUpButton = GameObject.Find("Main System/Robot Controll Canvas/Left Arm Up Button").GetComponent<Button>();
		LeftArmDownButton = GameObject.Find("Main System/Robot Controll Canvas/Left Arm Down Button").GetComponent<Button>();
		MoveForwardButton = GameObject.Find("Main System/Robot Controll Canvas/Move Forward Button").GetComponent<Button>();
		MoveBackButton = GameObject.Find("Main System/Robot Controll Canvas/Move Back Button").GetComponent<Button>();
		TurnRightButton = GameObject.Find("Main System/Robot Controll Canvas/Turn Right Button").GetComponent<Button>();
		TurnLeftButton = GameObject.Find("Main System/Robot Controll Canvas/Turn Left Button").GetComponent<Button>();
		
		PosXPlusButton = GameObject.Find("Main System/Button Canvas/X Plus Button").GetComponent<Button>();
		PosXMinusButton = GameObject.Find("Main System/Button Canvas/X Minus Button").GetComponent<Button>();
		PosYPlusButton = GameObject.Find("Main System/Button Canvas/Y Plus Button").GetComponent<Button>();
		PosYMinusButton = GameObject.Find("Main System/Button Canvas/Y Minus Button").GetComponent<Button>();
		PosZPlusButton = GameObject.Find("Main System/Button Canvas/Z Plus Button").GetComponent<Button>();
		PosZMinusButton = GameObject.Find("Main System/Button Canvas/Z Minus Button").GetComponent<Button>();
		LeftButton = GameObject.Find("Main System/Button Canvas/Left Button").GetComponent<Button>();
		RightButton = GameObject.Find("Main System/Button Canvas/Right Button").GetComponent<Button>();
		ArmResetButton = GameObject.Find("Main System/Button Canvas/Arm Reset Button").GetComponent<Button>();

		AddTrigger(RightArmUpButton);
		AddTrigger(RightArmDownButton);
		AddTrigger(LeftArmUpButton);
		AddTrigger(LeftArmDownButton);
		AddTrigger(MoveForwardButton);
		AddTrigger(MoveBackButton);
		AddTrigger(TurnRightButton);
		AddTrigger(TurnLeftButton);
		
		AddTrigger(PosXPlusButton);
		AddTrigger(PosXMinusButton);
		AddTrigger(PosYPlusButton);
		AddTrigger(PosYMinusButton);
		AddTrigger(PosZPlusButton);
		AddTrigger(PosZMinusButton);
		AddTrigger(LeftButton);
		AddTrigger(RightButton);
		//AddTrigger(ArmResetButton);
		ArmResetButton.onClick.AddListener(ArmReset);

		/*
		ButtonCanvas = GameObject.Find("Main System/Button Canvas").GetComponent<Canvas>();
		ButtonCanvas.gameObject.SetActive(false);
		*/
		/*
		ControllCanvas = GameObject.Find("Main System/Robot Controll Canvas").GetComponent<Canvas>();
		ControllCanvas.gameObject.SetActive(false);
		*/

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
			Main.NotActivateUI();
			robot_alpha_default = ColorController.robot_alpha;
			ColorController.robot_alpha = 0.0f;
			ColorController.ChangeRobotColors(ColorController.safety_color);
			arrow3D.SetActive(false);
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
					robot_euler.y -= rot2robot + 20.0f;
					transform.eulerAngles = robot_euler;

					/*
					Vector3 tmp_position = new Vector3(0.0f, 0.7f, 0.2f);
					coordinates_adapter.transform.localPosition = tmp_position;
					tmp_position = coordinates_adapter.transform.position;
					arrow3D.transform.position = tmp_position;
					Vector3 tmp_euler = transform.eulerAngles;
					arrow3D.transform.eulerAngles = tmp_euler;
					*/
					ArrowChange();

					ColorController.robot_alpha = robot_alpha_default;
					ColorController.ChangeRobotColors(ColorController.safety_color);
					//ButtonCanvas.gameObject.SetActive(true);
					//ControllCanvas.gameObject.SetActive(true);
					Main.NormalMode();
					arrow3D.SetActive(true);
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
			/*
			case "Arm Reset Button":
			entry_down.callback.AddListener((x) => { push_arm_reset = true; });
			entry_up.callback.AddListener((x) => { push_arm_reset = false; });
			break;
			*/
		}

		trigger.triggers.Add(entry_down);
		trigger.triggers.Add(entry_up);
	}

	void ButtonControl() {
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
			Vector3 tmp_position = new Vector3(0, 0, 0.8f * Time.deltaTime);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			transform.position = tmp_position;

			if (!finish_arrow) {
				arrow3D.SetActive(false);
				finish_arrow = true;
			}
		}

		if (push_move_back) {
			Vector3 tmp_position = new Vector3(0, 0, -0.8f * Time.deltaTime);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			transform.position = tmp_position;

			if (!finish_arrow) {
				arrow3D.SetActive(false);
				finish_arrow = true;
			}
		}

		if (push_turn_right || push_right) {
			transform.eulerAngles += new Vector3(0, 45.0f * Time.deltaTime, 0);

			ArrowChange();
		}

		if (push_turn_left || push_left) {
			transform.eulerAngles += new Vector3(0, -45.0f * Time.deltaTime, 0);

			ArrowChange();
		}


		if (push_x_plus) {
			Vector3 tmp_pos = transform.position;
			tmp_pos.x += 2.0f * Time.deltaTime;
			transform.position = tmp_pos;

			ArrowChange();
		}

		if (push_x_minus) {
			Vector3 tmp_pos = transform.position;
			tmp_pos.x -= 2.0f * Time.deltaTime;
			transform.position = tmp_pos;

			ArrowChange();
		}

		if (push_y_plus) {
			Vector3 tmp_pos = transform.position;
			tmp_pos.y += 0.5f * Time.deltaTime;
			transform.position = tmp_pos;

			ArrowChange();
		}

		if (push_y_minus) {
			Vector3 tmp_pos = transform.position;
			tmp_pos.y -= 0.5f * Time.deltaTime;
			transform.position = tmp_pos;

			ArrowChange();
		}

		if (push_z_plus) {
			Vector3 tmp_pos = transform.position;
			tmp_pos.z += 2.0f * Time.deltaTime;
			transform.position = tmp_pos;

			ArrowChange();
		}

		if (push_z_minus) {
			Vector3 tmp_pos = transform.position;
			tmp_pos.z -= 2.0f * Time.deltaTime;
			transform.position = tmp_pos;

			ArrowChange();
		}

		//if (push_arm_reset) {
			/*
			bool left_complete = false, right_complete = false;
			while (!left_complete || !right_complete) {
				if (!left_complete) {
					left_arm.transform.localRotation *= Quaternion.Euler(0, 60.0f * Time.deltaTime, 0);
					if (left_arm.transform.localEulerAngles.y < 0.5f || left_arm.transform.localEulerAngles.y > 359.5f) {
						left_arm.transform.localEulerAngles = new Vector3(0, 0, 180);
						left_complete = true;
					}
				}
				if (!right_complete) {
					right_arm.transform.localRotation *= Quaternion.Euler(0, 60.0f * Time.deltaTime, 0);
					if (right_arm.transform.localEulerAngles.y < 0.5f || right_arm.transform.localEulerAngles.y > 359.5f) {
						right_arm.transform.localEulerAngles = new Vector3(0, 0, 180);
						right_complete = true;
					}
				}
			}
			*/
			/*
			Vector3 tmp_euler = new Vector3(0, 0, 180);
			left_arm.transform.localEulerAngles = tmp_euler;
			right_arm.transform.localEulerAngles = tmp_euler;
			*/
			/*
			IEnumerator coroutine = ArmResetSlowly();
			StartCoroutine(coroutine);
			*/
		//}
	}

	public void ArrowChange() {
		if (!finish_arrow) {
			Vector3 tmp_position = new Vector3(0.0f, 0.5f, 0.2f);
			coordinates_adapter.transform.localPosition = tmp_position;
			tmp_position = coordinates_adapter.transform.position;
			arrow3D.transform.position = tmp_position;
			Vector3 tmp_euler = transform.eulerAngles;
			arrow3D.transform.eulerAngles = tmp_euler;
		}
	}

	void ArmReset() {
		if (!push_arm_reset) {
			IEnumerator coroutine = ArmResetSlowly();
			StartCoroutine(coroutine);
		}
	}

	IEnumerator ArmResetSlowly() {
		push_arm_reset = true;
		bool left_complete = false, right_complete = false;
		float old_left_angle = left_arm.transform.localEulerAngles.y;
		float old_right_angle = right_arm.transform.localEulerAngles.y;
		float now_left_angle = left_arm.transform.localEulerAngles.y;
		float now_right_angle = right_arm.transform.localEulerAngles.y;
		while (!left_complete || !right_complete) {
			if (!left_complete) {
				//if (left_arm.transform.localEulerAngles.y < 1.0f || left_arm.transform.localEulerAngles.y > 359.0f) {
				if ((old_left_angle * now_left_angle < 0 && Mathf.Abs(old_left_angle * now_left_angle) < 100.0f) || Mathf.Abs(now_left_angle) < 1.0f) {
					left_arm.transform.localEulerAngles = new Vector3(0, 0, 180);
					left_complete = true;
				}
				else {
					old_left_angle = left_arm.transform.localEulerAngles.y;
					left_arm.transform.localRotation *= Quaternion.Euler(0, 120.0f * Time.deltaTime, 0);
					now_left_angle = left_arm.transform.localEulerAngles.y;
					if (old_left_angle > 180.0f) {
						old_left_angle = (360.0f - old_left_angle) * -1;
					}
					if (now_left_angle > 180.0f) {
						now_left_angle = (360.0f - now_left_angle) * -1;
					}
				}
			}
			if (!right_complete) {
				//if (right_arm.transform.localEulerAngles.y < 1.0f || right_arm.transform.localEulerAngles.y > 359.0f) {
				if((old_right_angle * now_right_angle < 0 && Mathf.Abs(old_right_angle * now_right_angle) < 100.0f) || Mathf.Abs(now_right_angle) < 1.0f) {
					right_arm.transform.localEulerAngles = new Vector3(0, 0, 180);
					right_complete = true;
				}
				else {
					old_right_angle = right_arm.transform.localEulerAngles.y;
					right_arm.transform.localRotation *= Quaternion.Euler(0, 120.0f * Time.deltaTime, 0);
					now_right_angle = right_arm.transform.localEulerAngles.y;
					if(old_right_angle > 180.0f) {
						old_right_angle = (360.0f - old_right_angle) * -1;
					}
					if(now_right_angle > 180.0f) {
						now_right_angle = (360.0f - now_right_angle) * -1;
					}
				}
			}

			yield return null;
		}
		push_arm_reset = false;
	}
}
