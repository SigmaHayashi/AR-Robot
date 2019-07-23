using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour {

	public bool ScreenNOTSleep = true;

	//public Button changeMainSceneButton;
	//public Button changeTestSceneButton;

	[NonSerialized]
	public bool debug_mode = false;
	public Canvas TextCanvas;
	public Canvas ButtonCanvas;
	public Canvas RobotControllCanvas;

	// Use this for initialization
	void Start () {
		// 画面が消えないようにする
		if (ScreenNOTSleep) {
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
		else {
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
		}

		/*
		if (changeMainSceneButton != null) {
			changeMainSceneButton.onClick.AddListener(changeMainScene);
		}
		if (changeTestSceneButton != null) {
			changeTestSceneButton.onClick.AddListener(changeTestScene);
		}
		*/
		/*
		TextCanvas.gameObject.SetActive(false);
		ButtonCanvas.gameObject.SetActive(false);
		RobotControllCanvas.gameObject.SetActive(true);
		*/
	}
	
	// Update is called once per frame
	void Update () {
		// 戻るボタンでアプリ終了
		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}

		//6本指タッチでデバッグモードON/OFF切り替え
		if (!Application.isEditor) {
			if (Input.touchCount >= 6) {
				Touch touch = Input.GetTouch(Input.touchCount - 1);
				if (touch.phase == TouchPhase.Began) {
					debug_mode = !debug_mode;
					if (debug_mode) {
						DebugMode();
						/*
						RobotControllCanvas.gameObject.SetActive(false);
						TextCanvas.gameObject.SetActive(true);
						ButtonCanvas.gameObject.SetActive(true);
						*/
					}
					else {
						NormalMode();
						/*
						TextCanvas.gameObject.SetActive(false);
						ButtonCanvas.gameObject.SetActive(false);
						RobotControllCanvas.gameObject.SetActive(true);
						*/
					}
				}
			}
		}
	}

	public void DebugMode() {
		RobotControllCanvas.gameObject.SetActive(false);
		TextCanvas.gameObject.SetActive(true);
		ButtonCanvas.gameObject.SetActive(true);
		
		SmartPalContorol controll = GameObject.Find("smartpal5_link").GetComponent<SmartPalContorol>();
		controll.finish_arrow = false;
		controll.arrow3D.SetActive(true);
		controll.ArrowChange();

	}

	public void NormalMode() {
		TextCanvas.gameObject.SetActive(false);
		ButtonCanvas.gameObject.SetActive(false);
		RobotControllCanvas.gameObject.SetActive(true);
	}

	public void NotActivateUI() {
		TextCanvas.gameObject.SetActive(false);
		ButtonCanvas.gameObject.SetActive(false);
		RobotControllCanvas.gameObject.SetActive(false);
	}

	/*
	void changeMainScene() {
		SceneManager.LoadScene("AR B-sen");
	}

	void changeTestScene() {
		SceneManager.LoadScene("Shader Test Scene");
	}
	*/
}
