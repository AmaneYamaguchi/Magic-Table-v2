using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("e"))
			transform.position += Vector3.forward * 0.02f;
		if (Input.GetKey ("x"))
			transform.position += Vector3.back * 0.02f;
		if (Input.GetKey ("d"))
			transform.position += Vector3.right * 0.02f;
		if (Input.GetKey ("s"))
			transform.position += Vector3.left * 0.02f;
		if (Input.GetKey ("y"))
			transform.eulerAngles += Vector3.up;
		if (Input.GetKey ("t"))
			transform.eulerAngles += Vector3.down;
	}
}
