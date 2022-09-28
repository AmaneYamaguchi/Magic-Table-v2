using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ハンドシミュレータ（左手）
/// </summary>
public class HandTest_left : MonoBehaviour {
	void Update () {
		if (Input.GetKey ("i"))
			transform.position += Vector3.forward * 0.02f;
		if (Input.GetKey ("m"))
			transform.position += Vector3.back * 0.02f;
		if (Input.GetKey ("k"))
			transform.position += Vector3.right * 0.02f;
		if (Input.GetKey ("j"))
			transform.position += Vector3.left * 0.02f;
		if (Input.GetKey ("p"))
			transform.eulerAngles += Vector3.up;
		if (Input.GetKey ("o"))
			transform.eulerAngles += Vector3.down;
	}
}
