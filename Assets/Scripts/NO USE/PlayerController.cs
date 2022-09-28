using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの頭をキーボードで操作する用
/// </summary>
public class PlayerController : MonoBehaviour {
	void Update () {
		if (Input.GetKey ("up"))
			transform.position += Vector3.forward * 0.02f;
		if (Input.GetKey ("down"))
			transform.position += Vector3.back * 0.02f;
		if (Input.GetKey ("right"))
			transform.position += Vector3.right * 0.02f;
		if (Input.GetKey ("left"))
			transform.position += Vector3.left * 0.02f;
		if (Input.GetKey ("h"))
			transform.eulerAngles += Vector3.up;
		if (Input.GetKey ("g"))
			transform.eulerAngles += Vector3.down;
	}
}
