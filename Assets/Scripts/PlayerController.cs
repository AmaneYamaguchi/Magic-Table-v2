using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
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
