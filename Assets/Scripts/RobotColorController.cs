using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotColorController : MonoBehaviour {

	private DebugText debug;
	
	private List<GameObject> robot_parts = new List<GameObject>();
	private GameObject ar_camera;
	
	[Range(0, 10)]
	public float safety_distance = 1.0f;

	Renderer[] renderers;
	Material[] mats;
	private List<Color> origin_colors = new List<Color>();
	public Color safety_color = new Color32(60, 180, 255, 255);
	public Color danger_color = new Color32(255, 0, 0, 255);

	[Range(0, 1)]
	public float robot_alpha = 1.0f;

	private char robot_state;
	private const char ROBOT_STATE_SAFETY = (char)0x00;
	private const char ROBOT_STATE_DANGER = (char)0x01;

	// Start is called before the first frame update
	void Start() {
		debug = GameObject.Find("Debug Text").GetComponent<DebugText>();

		GetAllChildren(transform.gameObject, ref robot_parts);
		ar_camera = GameObject.Find("First Person Camera");

		renderers = GetComponentsInChildren<Renderer>();
		ChangeShader();
		SaveColors();
		ChangeRobotColors(origin_colors[0]);
		robot_state = ROBOT_STATE_SAFETY;
	}


	// Update is called once per frame
	void Update() {
		float distance;
		float min_distance = CalcDistance(transform.gameObject, ar_camera);

		foreach (GameObject part in robot_parts) {
			distance = CalcDistance(part, ar_camera);
			//Debug.Log("Name: " + part.name + ", distance: " + distance + ", min: " + min_distance);
			if (distance < min_distance) {
				min_distance = distance;
			}
		}

		debug.ClearDebug();
		debug.Debug("Distance: " + min_distance.ToString("f2"));

		if (min_distance < safety_distance) {
			if (robot_state != ROBOT_STATE_DANGER) {
				ChangeRobotColors(danger_color, ROBOT_STATE_DANGER);
			}
		}
		else {
			if (robot_state != ROBOT_STATE_SAFETY) {
				ChangeRobotColors(safety_color, ROBOT_STATE_SAFETY);
			}
		}
	}


	void ChangeShader() {
		foreach (Renderer ren in renderers) {
			mats = ren.materials;
			for (int i = 0; i < ren.materials.Length; i++) {
				mats[i].shader = Shader.Find("Custom/SemiTransparent");
			}
		}
	}

	void SaveColors() {
		foreach (Renderer ren in renderers) {
			mats = ren.materials;
			for (int i = 0; i < ren.materials.Length; i++) {
				origin_colors.Add(mats[i].color);
			}
		}
	}

	public void ChangeRobotColors(Color color, char state = ROBOT_STATE_SAFETY) {
		foreach (Renderer ren in renderers) {
			mats = ren.materials;
			for (int i = 0; i < ren.materials.Length; i++) {
				Color tmp_color = color;
				tmp_color.a = robot_alpha;
				mats[i].SetColor("_Color", tmp_color);
			}
			ren.materials = mats;
		}
		robot_state = state;
	}

	float CalcDistance(GameObject obj_a, GameObject obj_b) {
		Vector3 obj_a_pos = obj_a.transform.position;
		Vector3 obj_b_pos = obj_b.transform.position;
		return Mathf.Sqrt(Mathf.Pow((obj_a_pos.x - obj_b_pos.x), 2) + Mathf.Pow((obj_a_pos.z - obj_b_pos.z), 2));
	}

	void GetAllChildren(GameObject obj, ref List<GameObject> all_children) {
		Transform children = obj.GetComponentInChildren<Transform>();
		if(children.childCount == 0) {
			return;
		}
		foreach(Transform ob in children) {
			all_children.Add(ob.gameObject);
			GetAllChildren(ob.gameObject, ref all_children);
		}
	}

}
