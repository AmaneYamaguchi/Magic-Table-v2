using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTable : MonoBehaviour {
	public ParticleSystem smoke1;
	public ParticleSystem smoke2;
	public ParticleSystem smoke3;
	public ParticleSystem smoke4;
	public GameObject tabledirection;
	public GameObject square;
	public GameObject triangle;
	public GameObject pentagon;
	//public GameObject hexagon;
	public GameObject magic1;
	public GameObject magic2;
	public GameObject magic3;
	public static float duration = 0f;
	//public GameObject magic4;
	public AudioClip se;
	AudioSource audiosource;
	public static int dir = 0;  //頭の位置によってテーブルの向きを変えるのに使用するインデックス

	Vector3 initRoomPos;  //部屋の初期位置

	public static int scene = 0;//0:square, 1:tri, 2:pen
	public static int Out = 0;//0:inside, 1:outside
    public static bool change = false;

	void Awake () {
		smoke1.Stop ();
		smoke2.Stop ();
		smoke3.Stop ();
		smoke4.Stop ();
		audiosource = gameObject.GetComponent<AudioSource> ();
	}

	void Start () {
		initRoomPos = Rotation.wholeRoom.transform.position;
	}

	void Update () {
		//手を机の中心に合わせると机が変わる
		if ((Out == 1 && scene == 0 && distance_2d (magic1.transform.position, transform.position) < 0.2f && transform.position.y < 1f && transform.position.y > 0.5f && Rotation.direction != 4) || Input.GetKeyUp ("a")) {
			//四角形から三角形に変える
			GlobalVariables.Gain = 4f / 3f;
			playSmoke ();
			audiosource.PlayOneShot (se);
			dir = Rotation.direction;
			Invoke ("set3", 1.5f);
			Invoke ("table_direction", 0.8f);
			Invoke ("del4", 0.7f);
			Invoke ("scene0to1", 0.8f);
			Out = 0;
			duration = 0f;
		}

		if ((Out == 1 && scene == 1 && distance_2d (magic2.transform.position, transform.position) < 0.17f && transform.position.y < 1f && transform.position.y > 0.5f && Rotation.direction != 4) || Input.GetKey ("b")) {
			//三角形から五角形に変える
			GlobalVariables.Gain = 0.8f;
			playSmoke ();
			audiosource.PlayOneShot (se);
			dir = Rotation.direction;
			Invoke ("set5", 1.5f);
			Invoke ("table_direction", 0.8f);
			Invoke ("del3", 0.7f);
			Invoke ("scene1to2", 0.8f);
			Out = 0;
			duration = 0f;
		}

		if ((Out == 1 && scene == 2 && distance_2d (magic3.transform.position, transform.position) < 0.2f && transform.position.y < 1f && transform.position.y > 0.5f && Rotation.direction != 4) || Input.GetKey ("c")) {
			//五角形から四角形に変える
			playSmoke ();
			audiosource.PlayOneShot (se);
			dir = Rotation.direction;
			Invoke ("set4", 1.5f);
			Invoke ("table_direction", 0.8f);
			Invoke ("del5", 0.7f);
			Invoke ("scene2to0", 0.8f);
			Out = 0;
			duration = 0f;
		}

		duration += Time.deltaTime;
		if (duration > 15f) {
			Out = 1;
		}

	}

	/*魔法エフェクト*/
	void playSmoke(){
		smoke1.Play();
		smoke2.Play();
		smoke3.Play();
		smoke4.Play();
	}

	/*３角形のテーブルを出す*/
	void set3(){
		triangle.SetActive (true);
        change = false;
	}
	/*３角形のテーブルを消す*/
	void del3(){
		triangle.SetActive (false);
	}
	/*４角形のテーブルを出す*/
	void set4(){
		square.SetActive (true);
        change = false;
	}
	/*４角形のテーブルを消す*/
	void del4(){
		square.SetActive (false);
	}
	/*５角形のテーブルを出す*/
	void set5(){
		pentagon.SetActive (true);
        change = false;
	}
	/*５角形のテーブルを消す*/
	void del5(){
		pentagon.SetActive (false);
	}
	void transInit(){
		Rotation.wholeRoom.transform.position = initRoomPos;
		Rotation.wholeRoom.transform.eulerAngles = new Vector3(0, 0, 0);
	}
		
	void table_direction(){
		//dir = Rotation.direction;
		tabledirection.transform.eulerAngles = new Vector3 (0f, dir * 90f, 0f);
		tabledirection.transform.position = new Vector3 (0f, 0f, 0f);
        change = true;
	}

	void scene0to1(){
		scene = 1;
	}

	void scene1to2(){
		scene = 2;
	}

	void scene2to0(){
		scene = 0;
	}

	float distance_2d(Vector3 vec1,Vector3 vec2){	
		Vector2 v1 = new Vector2 (vec1.x, vec1.z);
		Vector2 v2 = new Vector2 (vec2.x, vec2.z);
		return Vector2.Distance (v1, v2);
	}
}