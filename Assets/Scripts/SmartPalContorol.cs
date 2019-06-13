using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SmartPalContorol : MonoBehaviour {

	//public GameObject SmartPal;

	public Text CameraPositionText;
	public Text SmartPalPositionText;

	private DebugText debug;

	private Button PosXPlusButton;
	private Button PosXMinusButton;
	private Button PosYPlusButton;
	private Button PosYMinusButton;
	private Button PosZPlusButton;
	private Button PosZMinusButton;

	private bool push_x_plus = false;
	private bool push_x_minus = false;
	private bool push_y_plus = false;
	private bool push_y_minus = false;
	private bool push_z_plus = false;
	private bool push_z_minus = false;

	// Start is called before the first frame update
	void Start() {
		debug = GameObject.Find("Main System/Text Canvas/Debug Text").GetComponent<DebugText>();
		
		PosXPlusButton = GameObject.Find("Main System/Button Canvas/X Plus Button").GetComponent<Button>();
		PosXMinusButton = GameObject.Find("Main System/Button Canvas/X Minus Button").GetComponent<Button>();
		PosYPlusButton = GameObject.Find("Main System/Button Canvas/Y Plus Button").GetComponent<Button>();
		PosYMinusButton = GameObject.Find("Main System/Button Canvas/Y Minus Button").GetComponent<Button>();
		PosZPlusButton = GameObject.Find("Main System/Button Canvas/Z Plus Button").GetComponent<Button>();
		PosZMinusButton = GameObject.Find("Main System/Button Canvas/Z Minus Button").GetComponent<Button>();
		
		AddTrigger(PosXPlusButton);
		AddTrigger(PosXMinusButton);
		AddTrigger(PosYPlusButton);
		AddTrigger(PosYMinusButton);
		AddTrigger(PosZPlusButton);
		AddTrigger(PosZMinusButton);

		//debug.Debug("HOGE");
	}

	// Update is called once per frame
	void Update() {
		CameraPositionText.text = "Camra Position : " + Camera.main.transform.position.ToString("f2");
		SmartPalPositionText.text = "SmartPal Position : " + this.transform.position.ToString("f2");

		ButtonControl();
	}

	void AddTrigger(Button button) {

		EventTrigger trigger = button.GetComponent<EventTrigger>();
		EventTrigger.Entry entry_down = new EventTrigger.Entry();
		entry_down.eventID = EventTriggerType.PointerDown;
		EventTrigger.Entry entry_up = new EventTrigger.Entry();
		entry_up.eventID = EventTriggerType.PointerUp;
		switch (button.name.ToString()) {
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
		}

		trigger.triggers.Add(entry_down);
		trigger.triggers.Add(entry_up);
	}

	void ButtonControl() {
		if (push_x_plus) {
			this.transform.position += new Vector3(1.0f * Time.deltaTime, 0, 0);
		}

		if (push_x_minus) {
			this.transform.position += new Vector3(-1.0f * Time.deltaTime, 0, 0);
		}

		if (push_y_plus) {
			this.transform.position += new Vector3(0, 1.0f * Time.deltaTime, 0);
		}

		if (push_y_minus) {
			this.transform.position += new Vector3(0, -1.0f * Time.deltaTime, 0);
		}

		if (push_z_plus) {
			this.transform.position += new Vector3(0, 0, 1.0f * Time.deltaTime);
		}

		if (push_z_minus) {
			this.transform.position += new Vector3(0, 0, -1.0f * Time.deltaTime);
		}
	}
}
