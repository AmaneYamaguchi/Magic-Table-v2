using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

	public static GameObject wholeRoom;

	//回転する軸
	private GameObject rotationpoint1;
	private GameObject rotationpoint2;
	private GameObject rotationpoint3;
	private GameObject rotationpoint4;

    //それぞれの点での，領域に入った時の頭の角度，現在の頭の角度，角度差，その時に回転軸が回転すべき角度
    float pre_angle_y1 = 0f, angle_y1 = 0f, dy1 = 0f, b1 = 0f;
    float pre_angle_y2 = 0f, angle_y2 = 0f, dy2 = 0f, b2 = 0f;
    float pre_angle_y3 = 0f, angle_y3 = 0f, dy3 = 0f, b3 = 0f;
    float pre_angle_y4 = 0f, angle_y4 = 0f, dy4 = 0f, b4 = 0f;

    float init_angle = 0f;  //領域に入った時の回転軸の角度
	float pos_x = 0f, pre_pos_x = 0f, dx = 0f;  //直線領域でのx方向もしくはz方向の位置，領域に入った時の位置，移動量
	float correct_dis = 0f;  //補正領域での補正完了距離

	private int flag1 = 0,flag2 = 0, flag3 = 0, flag4 = 0;  //0:回転領域，1:時計回りに通過，-1:反時計回りに通過，2:時計回りに途中で戻る，-2:反時計回りに途中で戻る
	private int tmp1 = 0, tmp2 = 0, tmp3 = 0, tmp4 = 0;  //flagの値を保持

	private float angle_y=0f;

	private float _rot = 0f;  //skyboxの回転量

	public static int direction = 0;  //テーブルに対してどの位置にいるかの判定

	void Awake() {
		wholeRoom = GameObject.Find("wholeRoom");
		rotationpoint1 = GameObject.Find ("rotationpoint1");
		rotationpoint2 = GameObject.Find ("rotationpoint2");
		rotationpoint3 = GameObject.Find ("rotationpoint3");
		rotationpoint4 = GameObject.Find ("rotationpoint4");
	}

	void Start() {
		
	}

	void Update(){
		if ((transform.position.z < transform.position.x && transform.position.z < -transform.position.x && transform.position.z > -HandTranslate.WID / 2f) || (transform.position.x > -HandTranslate.WID / 2f && transform.position.x < HandTranslate.WID / 2f && transform.position.z < -HandTranslate.WID / 2f))
			direction = 0;
		else if ((transform.position.z > transform.position.x && transform.position.z < -transform.position.x && transform.position.x > -HandTranslate.WID / 2f) || (transform.position.z > -HandTranslate.WID / 2f && transform.position.z < HandTranslate.WID / 2f && transform.position.x < -HandTranslate.WID / 2f))
			direction = 1;
		else if ((transform.position.z > transform.position.x && transform.position.z > -transform.position.x && transform.position.z < HandTranslate.WID / 2f) || (transform.position.x > -HandTranslate.WID / 2f && transform.position.x < HandTranslate.WID / 2f && transform.position.z > HandTranslate.WID / 2f))
			direction = 2;
		else if ((transform.position.z < transform.position.x && transform.position.z > -transform.position.x && transform.position.x < HandTranslate.WID / 2f) || (transform.position.z > -HandTranslate.WID / 2f && transform.position.z < HandTranslate.WID / 2f && transform.position.x > HandTranslate.WID / 2f))
			direction = 3;
		else
			direction = 4;

        if (ChangeTable.change == true)
        {
            rotationpoint1.transform.eulerAngles = new Vector3(0f, b1, 0f);
            rotationpoint2.transform.eulerAngles = new Vector3(0f, b2, 0f);
            rotationpoint3.transform.eulerAngles = new Vector3(0f, b3, 0f);
            rotationpoint4.transform.eulerAngles = new Vector3(0f, b4, 0f);
            wholeRoom.transform.parent = null;
        }
		/*
		if (transform.position.z < transform.position.x && transform.position.z < -transform.position.x)
			direction = 0;
		else if (transform.position.z > transform.position.x && transform.position.z < -transform.position.x)
			direction = 1;
		else if (transform.position.z > transform.position.x && transform.position.z > -transform.position.x)
			direction = 2;
		else if (transform.position.z < transform.position.x && transform.position.z > -transform.position.x)
			direction = 3;
		*/
		/*
		if (ChangeTable.scene != 0) {
			if (transform.position.x < rotationpoint1.transform.position.x && transform.position.z < rotationpoint1.transform.position.z) {
				wholeRoom.transform.parent = rotationpoint1.transform;
				angle_y1 = Mathf.Atan2 (transform.position.x - rotationpoint1.transform.position.x, transform.position.z - rotationpoint1.transform.position.z) * Mathf.Rad2Deg;
				if (angle_y1 < -10f)
					angle_y1 += 180f;
				if (angle_y1 > 100f)
					angle_y1 -= 180f;
				angle_y1 *= (1f - GlobalVariables.Gain);
				angle_y1 += dy1;
				if (Mathf.Abs (angle_y1 - pre_angle_y1) > 20f) {
					dy1 += pre_angle_y1;
					angle_y1 += dy1;
				}
				rotationpoint1.transform.eulerAngles = new Vector3 (0f, angle_y1, 0f);
				pre_angle_y1 = angle_y1;
			}
			if (transform.position.x < rotationpoint2.transform.position.x && transform.position.z > rotationpoint2.transform.position.z) {
				wholeRoom.transform.parent = rotationpoint2.transform;
				angle_y2 = Mathf.Atan2 (transform.position.z - rotationpoint2.transform.position.z, -transform.position.x + rotationpoint2.transform.position.x) * Mathf.Rad2Deg;
				if (angle_y2 < -10f)
					angle_y2 += 180f;
				if (angle_y2 > 100f)
					angle_y2 -= 180f;
				angle_y2 *= (1f - GlobalVariables.Gain);
				angle_y2 += dy2;
				if (Mathf.Abs (angle_y2 - pre_angle_y2) > 20f) {
					dy2 += pre_angle_y2;
					angle_y2 += dy2;
				}
				rotationpoint2.transform.eulerAngles = new Vector3 (0f, angle_y2, 0f);
				pre_angle_y2 = angle_y2;
			}
			if (transform.position.x > rotationpoint3.transform.position.x && transform.position.z > rotationpoint3.transform.position.z) {
				wholeRoom.transform.parent = rotationpoint3.transform;
				angle_y3 = Mathf.Atan2 (transform.position.x - rotationpoint3.transform.position.x, transform.position.z - rotationpoint3.transform.position.z) * Mathf.Rad2Deg;
				if (angle_y3 < -10f)
					angle_y3 += 180f;
				if (angle_y3 > 100f)
					angle_y3 -= 180f;
				angle_y3 *= (1f - GlobalVariables.Gain);
				angle_y3 += dy3;
				if (Mathf.Abs (angle_y3 - pre_angle_y3) > 20f) {
					dy3 += pre_angle_y3;
					angle_y3 += dy3;
				}
				rotationpoint3.transform.eulerAngles = new Vector3 (0f, angle_y3, 0f);
				pre_angle_y3 = angle_y3;
			}
			if (transform.position.x > rotationpoint4.transform.position.x && transform.position.z < rotationpoint4.transform.position.z) {
				wholeRoom.transform.parent = rotationpoint4.transform;
				angle_y4 = Mathf.Atan2 (-transform.position.z + rotationpoint4.transform.position.z, transform.position.x - rotationpoint4.transform.position.x) * Mathf.Rad2Deg;
				if (angle_y4 < -10f)
					angle_y4 += 180f;
				if (angle_y4 > 100f)
					angle_y4 -= 180f;
				angle_y4 *= (1f - GlobalVariables.Gain);
				angle_y4 += dy4;
				if (Mathf.Abs (angle_y4 - pre_angle_y4) > 20f) {
					dy4 += pre_angle_y4;
					angle_y4 += dy4;
				}
				rotationpoint4.transform.eulerAngles = new Vector3 (0f, angle_y4, 0f);
				pre_angle_y4 = angle_y4;
			}
			wholeRoom.transform.parent = null;
		}*/

		if (ChangeTable.scene != 0) {
			//回転領域に入った時の処理
			if ((transform.position.x < rotationpoint1.transform.position.x && transform.position.z < rotationpoint1.transform.position.z) || (transform.position.z > transform.position.x - 0.15f && transform.position.z < transform.position.x + 0.15f && transform.position.z < -transform.position.x - 0.15f)) {
				switch (flag1) {
				case 0:
					if (transform.position.z < transform.position.x)
						flag1 = -2;
					else
						flag1 = 2;
					tmp1 = 0;
					tmp2 = 0;
					tmp3 = 0;
					tmp4 = 0;
					rotationpoint1.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationpoint2.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationpoint3.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationpoint4.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationpoint1.transform.eulerAngles.y;
					pre_angle_y1 = transform.eulerAngles.y;
					break;
				case 1:
					if (transform.position.z < transform.position.x)
						flag1 = -2;
					break;
				case -1:
					if (transform.position.z > transform.position.x)
						flag1 = 2;
					break;
				case 2:
					if (transform.position.z < transform.position.x)
						flag1 = -1;
					break;
				case -2:
					if (transform.position.z > transform.position.x)
						flag1 = 1;
					break;
				default:
					break;
				}
				wholeRoom.transform.parent = rotationpoint1.transform;
				angle_y1 = transform.eulerAngles.y;
				//回転角が360°以上変化しないよう調整
				if (dy1 - (angle_y1 - pre_angle_y1) < -300f) {
					_rot += (dy1 - (angle_y1 - pre_angle_y1) + 360f) * (1f - GlobalVariables.Gain);
					dy1 = angle_y1 - pre_angle_y1 - 360f;
				} else if (dy1 - (angle_y1 - pre_angle_y1) > 300f) {
					_rot += (dy1 - (angle_y1 - pre_angle_y1) - 360f) * (1f - GlobalVariables.Gain);
					dy1 = angle_y1 - pre_angle_y1 + 360f;
				} else {
					_rot += (dy1 - (angle_y1 - pre_angle_y1)) * (1f - GlobalVariables.Gain);
					dy1 = angle_y1 - pre_angle_y1;
				}
				rotationpoint1.transform.eulerAngles = new Vector3 (0f, dy1 * (1f - GlobalVariables.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", _rot);
			} else if ((transform.position.x < rotationpoint2.transform.position.x && transform.position.z > rotationpoint2.transform.position.z) || (transform.position.z > -transform.position.x - 0.15f && transform.position.z < -transform.position.x + 0.15f && transform.position.z > transform.position.x + 0.15f)) {
				switch (flag2) {
				case 0:
					if (transform.position.z < -transform.position.x)
						flag2 = -2;
					else
						flag2 = 2;
					tmp1 = 0;
					tmp2 = 0;
					tmp3 = 0;
					tmp4 = 0;
					rotationpoint1.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationpoint2.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationpoint3.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationpoint4.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationpoint2.transform.eulerAngles.y;
					pre_angle_y2 = transform.eulerAngles.y;
					break;
				case 1:
					if (transform.position.z < -transform.position.x)
						flag2 = -2;
					break;
				case -1:
					if (transform.position.z > -transform.position.x)
						flag2 = 2;
					break;
				case 2:
					if (transform.position.z < -transform.position.x)
						flag2 = -1;
					break;
				case -2:
					if (transform.position.z > -transform.position.x)
						flag2 = 1;
					break;
				default:
					break;
				}
				wholeRoom.transform.parent = rotationpoint2.transform;
				angle_y2 = transform.eulerAngles.y;
				if (dy2 - (angle_y2 - pre_angle_y2) < -300f) {
					_rot += (dy2 - (angle_y2 - pre_angle_y2) + 360f) * (1f - GlobalVariables.Gain);
					dy2 = angle_y2 - pre_angle_y2 - 360f;
				} else if (dy2 - (angle_y2 - pre_angle_y2) > 300f) {
					_rot += (dy2 - (angle_y2 - pre_angle_y2) - 360f) * (1f - GlobalVariables.Gain);
					dy2 = angle_y2 - pre_angle_y2 + 360f;
				} else {
					_rot += (dy2 - (angle_y2 - pre_angle_y2)) * (1f - GlobalVariables.Gain);
					dy2 = angle_y2 - pre_angle_y2;
				}
				rotationpoint2.transform.eulerAngles = new Vector3 (0f, dy2 * (1f - GlobalVariables.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", _rot);
			} else if ((transform.position.x > rotationpoint3.transform.position.x && transform.position.z > rotationpoint3.transform.position.z) || (transform.position.z > transform.position.x - 0.15f && transform.position.z < transform.position.x + 0.15f && transform.position.z > -transform.position.x + 0.15f)) {
				switch (flag3) {
				case 0:
					if (transform.position.z > transform.position.x)
						flag3 = -2;
					else
						flag3 = 2;
					tmp1 = 0;
					tmp2 = 0;
					tmp3 = 0;
					tmp4 = 0;
					rotationpoint1.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationpoint2.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationpoint3.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationpoint4.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationpoint3.transform.eulerAngles.y;
					pre_angle_y3 = transform.eulerAngles.y;
					break;
				case 1:
					if (transform.position.z > transform.position.x)
						flag3 = -2;
					break;
				case -1:
					if (transform.position.z < transform.position.x)
						flag3 = 2;
					break;
				case 2:
					if (transform.position.z > transform.position.x)
						flag3 = -1;
					break;
				case -2:
					if (transform.position.z < transform.position.x)
						flag3 = 1;
					break;
				default:
					break;
				}
				wholeRoom.transform.parent = rotationpoint3.transform;
				angle_y3 = transform.eulerAngles.y;
				if (dy3 - (angle_y3 - pre_angle_y3) < -300f) {
					_rot += (dy3 - (angle_y3 - pre_angle_y3) + 360f) * (1f - GlobalVariables.Gain);
					dy3 = angle_y3 - pre_angle_y3 - 360f;
				} else if (dy3 - (angle_y3 - pre_angle_y3) > 300f) {
					_rot += (dy3 - (angle_y3 - pre_angle_y3) - 360f) * (1f - GlobalVariables.Gain);
					dy3 = angle_y3 - pre_angle_y3 + 360f;
				} else {
					_rot += (dy3 - (angle_y3 - pre_angle_y3)) * (1f - GlobalVariables.Gain);
					dy3 = angle_y3 - pre_angle_y3;
				}
				rotationpoint3.transform.eulerAngles = new Vector3 (0f, dy3 * (1f - GlobalVariables.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", _rot);
			} else if ((transform.position.x > rotationpoint4.transform.position.x && transform.position.z < rotationpoint4.transform.position.z) || (transform.position.z > -transform.position.x - 0.15f && transform.position.z < -transform.position.x + 0.15f && transform.position.z < transform.position.x - 0.15f)) {
				switch (flag4) {
				case 0:
					if (transform.position.z > -transform.position.x)
						flag4 = -2;
					else
						flag4 = 2;
					tmp1 = 0;
					tmp2 = 0;
					tmp3 = 0;
					tmp4 = 0;
					rotationpoint1.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationpoint2.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationpoint3.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationpoint4.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationpoint4.transform.eulerAngles.y;
					pre_angle_y4 = transform.eulerAngles.y;
					break;
				case 1:
					if (transform.position.z > -transform.position.x)
						flag4 = -2;
					break;
				case -1:
					if (transform.position.z < -transform.position.x)
						flag4 = 2;
					break;
				case 2:
					if (transform.position.z > -transform.position.x)
						flag4 = -1;
					break;
				case -2:
					if (transform.position.z < -transform.position.x)
						flag4 = 1;
					break;
				default:
					break;
				}
				wholeRoom.transform.parent = rotationpoint4.transform;
				angle_y4 = transform.eulerAngles.y;
				if (dy4 - (angle_y4 - pre_angle_y4) < -300f) {
					_rot += (dy4 - (angle_y4 - pre_angle_y4) + 360f) * (1f - GlobalVariables.Gain);
					dy4 = angle_y4 - pre_angle_y4 - 360f;
				} else if (dy4 - (angle_y4 - pre_angle_y4) > 300f) {
					_rot += (dy4 - (angle_y4 - pre_angle_y4) - 360f) * (1f - GlobalVariables.Gain);
					dy4 = angle_y4 - pre_angle_y4 + 360f;
				} else {
					_rot += (dy4 - (angle_y4 - pre_angle_y4)) * (1f - GlobalVariables.Gain);
					dy4 = angle_y4 - pre_angle_y4;
				}
				rotationpoint4.transform.eulerAngles = new Vector3 (0f, dy4 * (1f - GlobalVariables.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", _rot);
			} else {
				//直線領域に入った時の補正
				if (flag1 != 0) {
					tmp1 = flag1;
					init_angle = rotationpoint1.transform.eulerAngles.y;
				}
				if (flag2 != 0) {
					tmp2 = flag2;
					init_angle = rotationpoint2.transform.eulerAngles.y;
				}
				if (flag3 != 0) {
					tmp3 = flag3;
					init_angle = rotationpoint3.transform.eulerAngles.y;
				}
				if (flag4 != 0) {
					tmp4 = flag4;
					init_angle = rotationpoint4.transform.eulerAngles.y;
				}

				if (flag1 == 1 || flag1 == -1) {
					b1 += 90f * (1f - GlobalVariables.Gain) * (float)flag1;
					if (b1 < 0f)
						b1 += 360f;
					if (b1 > 360f)
						b1 -= 360f;
				}
				if (flag2 == 1 || flag2 == -1) {
					b2 += 90f * (1f - GlobalVariables.Gain) * (float)flag2;
					if (b2 < 0f)
						b2 += 360f;
					if (b2 > 360f)
						b2 -= 360f;
				}
				if (flag3 == 1 || flag3 == -1) {
					b3 += 90f * (1f - GlobalVariables.Gain) * (float)flag3;
					if (b3 < 0f)
						b3 += 360f;
					if (b3 > 360f)
						b3 -= 360f;
				}
				if (flag4 == 1 || flag4 == -1) {
					b4 += 90f * (1f - GlobalVariables.Gain) * (float)flag4;
					if (b4 < 0f)
						b4 += 360f;
					if (b4 > 360f)
						b4 -= 360f;
				}
		
				if (flag1 > 0 || flag2 < 0 || flag3 > 0 || flag4 < 0) {
					pre_pos_x = transform.position.z;
					correct_dis = HandTranslate.WID / 2f + Mathf.Abs (pre_pos_x);
				}
				if (flag1 < 0 || flag2 > 0 || flag3 < 0 || flag4 > 0) {
					pre_pos_x = transform.position.x;
					correct_dis = HandTranslate.WID / 2f + Mathf.Abs (pre_pos_x);
				}

				/*
				if (flag1 > 0 || flag4 < 0 || flag1 < 0 || flag2 > 0)
					pre_pos_x = -HandTranslate.WID / 2f;
				if (flag2 < 0 || flag3 > 0 || flag3 < 0 || flag4 > 0)
					pre_pos_x = HandTranslate.WID / 2f;
					*/
				
				if (tmp1 > 0) {
					if (Mathf.Abs (rotationpoint1.transform.eulerAngles.y - b1) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b1 - init_angle < -300f)
							init_angle -= 360f;
						else if (b1 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b1 - init_angle) / HandTranslate.WID * dx;
						rotationpoint1.transform.eulerAngles += new Vector3 (0f, (b1 - init_angle) / correct_dis * dx, 0f);
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						pre_pos_x = pos_x;
						angle_y = rotationpoint1.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint1.transform.eulerAngles.y - b1) > 0.1f) {
						rotationpoint1.transform.eulerAngles += new Vector3 (0f, (b1 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b1 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint1.transform.eulerAngles = new Vector3 (0f, b1, 0f);
				}
				if (tmp1 < 0) {
					if (Mathf.Abs (rotationpoint1.transform.eulerAngles.y - b1) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b1 - init_angle < -300f)
							init_angle -= 360f;
						else if (b1 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b1 - init_angle) / HandTranslate.WID * dx;
						rotationpoint1.transform.eulerAngles += new Vector3 (0f, (b1 - init_angle) / correct_dis * dx, 0f);
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						pre_pos_x = pos_x;
						angle_y = rotationpoint1.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint1.transform.eulerAngles.y - b1) > 0.1f) {
						rotationpoint1.transform.eulerAngles += new Vector3 (0f, (b1 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b1 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint1.transform.eulerAngles = new Vector3 (0f, b1, 0f);
				}
				if (tmp2 > 0) {
					if (Mathf.Abs (rotationpoint2.transform.eulerAngles.y - b2) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b2 - init_angle < -300f)
							init_angle -= 360f;
						else if (b2 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b2 - init_angle) / HandTranslate.WID * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						rotationpoint2.transform.eulerAngles += new Vector3 (0f, (b2 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationpoint2.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint2.transform.eulerAngles.y - b2) > 0.1f) {
						rotationpoint2.transform.eulerAngles += new Vector3 (0f, (b2 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b2 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint2.transform.eulerAngles = new Vector3 (0f, b2, 0f);
				}
				if (tmp2 < 0) {
					if (Mathf.Abs (rotationpoint2.transform.eulerAngles.y - b2) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b2 - init_angle < -300f)
							init_angle -= 360f;
						else if (b2 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b2 - init_angle) / HandTranslate.WID * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						rotationpoint2.transform.eulerAngles += new Vector3 (0f, (b2 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationpoint2.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint2.transform.eulerAngles.y - b2) > 0.1f) {
						rotationpoint2.transform.eulerAngles += new Vector3 (0f, (b2 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b2 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint2.transform.eulerAngles = new Vector3 (0f, b2, 0f);
				}
				if (tmp3 > 0) {
					if (Mathf.Abs (rotationpoint3.transform.eulerAngles.y - b3) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b3 - init_angle < -300f)
							init_angle -= 360f;
						else if (b3 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b3 - init_angle) / HandTranslate.WID * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						rotationpoint3.transform.eulerAngles += new Vector3 (0f, (b3 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationpoint3.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint3.transform.eulerAngles.y - b3) > 0.1f) {
						rotationpoint3.transform.eulerAngles += new Vector3 (0f, (b3 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b3 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint3.transform.eulerAngles = new Vector3 (0f, b3, 0f);
				}
				if (tmp3 < 0) {
					if (Mathf.Abs (rotationpoint3.transform.eulerAngles.y - b3) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b3 - init_angle < -300f)
							init_angle -= 360f;
						else if (b3 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b3 - init_angle) / HandTranslate.WID * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						rotationpoint3.transform.eulerAngles += new Vector3 (0f, (b3 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationpoint3.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint3.transform.eulerAngles.y - b3) > 0.1f) {
						rotationpoint3.transform.eulerAngles += new Vector3 (0f, (b3 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -=(b3 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint3.transform.eulerAngles = new Vector3 (0f, b3, 0f);
				}
				if (tmp4 > 0) {
					if (Mathf.Abs (rotationpoint4.transform.eulerAngles.y - b4) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b4 - init_angle < -300f)
							init_angle -= 360f;
						else if (b4 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b4 - init_angle) / HandTranslate.WID * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						rotationpoint4.transform.eulerAngles += new Vector3 (0f, (b4 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationpoint4.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint4.transform.eulerAngles.y - b4) > 0.1f) {
						rotationpoint4.transform.eulerAngles += new Vector3 (0f, (b4 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b4 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint4.transform.eulerAngles = new Vector3 (0f, b4, 0f);
				}
				if (tmp4 < 0) {
					if (Mathf.Abs (rotationpoint4.transform.eulerAngles.y - b4) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b4 - init_angle < -300f)
							init_angle -= 360f;
						else if (b4 - init_angle > 300f)
							init_angle += 360f;
						_rot -= (b4 - init_angle) / HandTranslate.WID * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
						rotationpoint4.transform.eulerAngles += new Vector3 (0f, (b4 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationpoint4.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationpoint4.transform.eulerAngles.y - b4) > 0.1f) {
						rotationpoint4.transform.eulerAngles += new Vector3 (0f, (b4 - angle_y) * Time.deltaTime * 5f, 0f);
						_rot -= (b4 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", _rot);
					} else
						rotationpoint4.transform.eulerAngles = new Vector3 (0f, b4, 0f);
				}
				flag1 = 0;
				flag2 = 0;
				flag3 = 0;
				flag4 = 0;
			}
		} 
	}
}
