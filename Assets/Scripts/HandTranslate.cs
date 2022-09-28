using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTranslate : MonoBehaviour
{
	//public static float TableManager.Instance.TableWidth = 0.7f;  //テーブルの一辺の長さ
	
	private float re_r;  //現実でのテーブル中心からトラッカーまでの距離
	private float re_theta;  //現実でのテーブル中心から下に下ろした直線と，トラッカーまでの直線のなす角（反時計回り正）
	private float vr_r;  //VR空間でのテーブル中心からトラッカーまでの距離
	private float vr_theta;  //VR空間でのなす角

	private Vector3 con_pos;  //トラッカーの位置
	private Vector3 con_angle;  //トラッカーの姿勢

	private Vector3 pre_pos; //直前の手の位置

	public GameObject controller;  //トラッカー

	private float angle_y = 0f, pre_angle_y = 0f,dy = 0f;  //現在の手の角度，境界領域に入った時の手の角度，角度の変化量
	private float angle_tmp = 0f;  //領域の境に入った時変換式の回転角度を保持

	private Vector3 square = new Vector3 (0f, 0f, 0f);  //四角形のテーブルの中心
	private Vector3 triangle = new Vector3 (0f, 0f, TableManager.Instance.TableWidth / 2f * (1f / Mathf.Sqrt (3f) - 1f));  //三角形のテーブルの中心
	private Vector3 pentagon = new Vector3 (0f, 0f, TableManager.Instance.TableWidth / 2f * (Mathf.Tan (54f * Mathf.Deg2Rad) - 1f));  //五角形のテーブルの中心
	private float center;  //テーブルの中心

	private int pre_re_area = 0;  //一つ前の現実でのトラッカーの位置
	private int re_area = 0;  //現在の現実でのトラッカーの位置
	private int pre_vr_area = 0;  //VR空間での一つ前の手の位置
	private int vr_area = 0;  //VR空間での現在の手の位置

	private bool flag1=false;  //シーンが１になったかどうか
	private bool flag2=false;  //シーンが２になったかどうか
	//enum型をつかうといいかも

	private bool tmp1 = false;  //現実でのトラッカーの位置が変わったかの判定
	private bool tmp2 = false;  //VR空間での手の位置が変わったかの判定
	private bool tmp3 = false;  //エリアの境目に入ったかどうかの判定

	void Start () {
		controller.SetActive (true);
	}

	void Update () {
		if (TableManager.Instance.CurrentShape != TableManager.TableShape.Square) {
			trans_con ();  //テーブルを変形させた時の位置によって変換したトラッカーの座標，姿勢を格納

			if (TableManager.Instance.CurrentShape == TableManager.TableShape.Triangle && flag1 == false) {
				pre_vr_area = 0;
				vr_area = 0;
				pre_re_area = 0;
				re_area = 0;
				flag1 = true;
				flag2 = false;
			}
			if (TableManager.Instance.CurrentShape == TableManager.TableShape.Pentagon && flag2 == false) {
				pre_vr_area = 3;
				vr_area = 3;
				pre_re_area = 0;
				re_area = 0;
				flag2 = true;
				flag1 = false;
			}



			//現実でのトラッカーの位置を判定
			if (con_pos.z < con_pos.x && con_pos.z < -con_pos.x && re_area != 0) {
				re_area = 0;
				tmp1 = true;
			} else if (con_pos.z > con_pos.x && con_pos.z < -con_pos.x && re_area != 1) {
				re_area = 1;
				tmp1 = true;
			} else if (con_pos.z > con_pos.x && con_pos.z > -con_pos.x && re_area != 2) {
				re_area = 2;
				tmp1 = true;
			} else if (con_pos.z < con_pos.x && con_pos.z > -con_pos.x && re_area != 3) {
				re_area = 3;
				tmp1 = true;
			}

			//VR空間での手の位置を判定
			if (TableManager.Instance.CurrentShape == TableManager.TableShape.Triangle) {
				if (transform.localPosition.z < transform.localPosition.x / Mathf.Sqrt (3f) + triangle.z && transform.localPosition.z < -transform.localPosition.x / Mathf.Sqrt (3f) + triangle.z && vr_area != 0) {
					vr_area = 0;
					tmp2 = true;
				} else if (transform.localPosition.z > transform.localPosition.x / Mathf.Sqrt (3f) + triangle.z && transform.localPosition.x < 0f && vr_area != 1) {
					vr_area = 1;
					tmp2 = true;
				} else if (transform.localPosition.z > -transform.localPosition.x / Mathf.Sqrt (3f) + triangle.z && transform.localPosition.x > 0f && vr_area != 2) {
					vr_area = 2;
					tmp2 = true;
				}
			}
			if (TableManager.Instance.CurrentShape == TableManager.TableShape.Pentagon) {
				if (transform.localPosition.z < transform.localPosition.x * Mathf.Tan (54f * Mathf.Deg2Rad) + pentagon.z && transform.localPosition.z < -transform.localPosition.x * Mathf.Tan (54f * Mathf.Deg2Rad) + pentagon.z && vr_area != 3) {
					vr_area = 3;
					tmp2 = true;
				} else if (transform.localPosition.z > transform.localPosition.x * Mathf.Tan (54f * Mathf.Deg2Rad) + pentagon.z && transform.localPosition.z < -transform.localPosition.x * Mathf.Tan (18f * Mathf.Deg2Rad) + pentagon.z && vr_area != 4) {
					vr_area = 4;
					tmp2 = true;
				} else if (transform.localPosition.z > -transform.localPosition.x * Mathf.Tan (18f * Mathf.Deg2Rad) + pentagon.z && transform.localPosition.x < 0f && vr_area != 5) {
					vr_area = 5;
					tmp2 = true;
				} else if (transform.localPosition.x > 0f && transform.localPosition.z > transform.localPosition.x * Mathf.Tan (18f * Mathf.Deg2Rad) + pentagon.z && vr_area != 6) {
					vr_area = 6;
					tmp2 = true;
				} else if (transform.localPosition.z < transform.localPosition.x * Mathf.Tan (18f * Mathf.Deg2Rad) + pentagon.z && transform.localPosition.z > -transform.localPosition.x * Mathf.Tan (54f * Mathf.Deg2Rad) + pentagon.z && vr_area != 7) {
					vr_area = 7;
					tmp2 = true;
				}
			}

			//現実，VR空間両方で位置が変化した場合変換式を変更
			if (tmp1 == true && tmp2 == true) {
				pre_re_area = re_area;
				pre_vr_area = vr_area;
				tmp1 = false;
				tmp2 = false;
			}

			re_r = distance_2d (square, con_pos);
			re_theta = angle_2d (Vector3.back, con_pos);

			//re_thetaには絶対値しか格納されないので，方向によって正負を判定
			//トラッカーの場所によって90°ずつ値を変更
			if (Vector3.Cross (Vector3.back, con_pos).y > 0f) {
				re_theta *= -1;
				re_theta += pre_re_area * 90f;
			} else{
				if (pre_re_area == 3)
					re_theta -= 90f;
				else
					re_theta -= pre_re_area * 90f;
			}
				
			trans_hand (pre_re_area, pre_vr_area);


			//手が大きく飛んだ時元の位置に戻す
			if ((pre_pos - transform.position).magnitude > 0.2f) {
				//transform.position = pre_pos;
				transform.position = controller.transform.position;
				tmp1 = true;
			}
		} else {
			transform.position = new Vector3 (controller.transform.position.x, controller.transform.position.y, controller.transform.position.z);
			transform.eulerAngles = new Vector3 (controller.transform.eulerAngles.x, controller.transform.eulerAngles.y, controller.transform.eulerAngles.z);
		}

		//手の位置がおかしくなったときはenter
		if (Input.GetKeyDown (KeyCode.Return)) {
			transform.position = controller.transform.position;
			tmp1 = true;
		}
		pre_pos = transform.position;
	}

	//2次元での二つの物体の距離
	float distance_2d(Vector3 vec1,Vector3 vec2){	
		Vector2 v1 = new Vector2 (vec1.x, vec1.z);
		Vector2 v2 = new Vector2 (vec2.x, vec2.z);
		return Vector2.Distance (v1, v2);
	}

	//2次元での二つのベクトルのなす角
	float angle_2d(Vector3 vec1,Vector3 vec2){
		Vector2 v1 = new Vector2 (vec1.x, vec1.z);
		Vector2 v2 = new Vector2 (vec2.x, vec2.z);
		return Vector2.Angle (v1, v2);
	}
		
	void trans_con(){
		con_pos = new Vector3 (controller.transform.position.x * Mathf.Cos (TableManager.Instance.dir * 90f * Mathf.Deg2Rad) - controller.transform.position.z * Mathf.Sin (TableManager.Instance.dir * 90f * Mathf.Deg2Rad), controller.transform.position.y, controller.transform.position.x * Mathf.Sin (TableManager.Instance.dir * 90f * Mathf.Deg2Rad) + controller.transform.position.z * Mathf.Cos (TableManager.Instance.dir * 90f * Mathf.Deg2Rad));
		con_angle = new Vector3 (controller.transform.eulerAngles.x, controller.transform.eulerAngles.y - TableManager.Instance.dir * 90f, controller.transform.eulerAngles.z);
	}

	void trans_pos(float angle_gain,float theta,float center,float angle){
		vr_theta = re_theta * angle_gain;
		vr_r = re_r * Mathf.Tan (theta * Mathf.Deg2Rad) * Mathf.Cos (re_theta * Mathf.Deg2Rad) / Mathf.Cos (vr_theta * Mathf.Deg2Rad);
		transform.localPosition = new Vector3 (vr_r * Mathf.Cos ((vr_theta + angle) * Mathf.Deg2Rad), con_pos.y, vr_r * Mathf.Sin ((vr_theta + angle) * Mathf.Deg2Rad) + center);
	}

	void trans_angle(float gain ,float angle){
		if (Mathf.Abs (transform.position.x - transform.position.z) / Mathf.Sqrt (2f) < 0.1f || Mathf.Abs (transform.position.x + transform.position.z) / Mathf.Sqrt (2f) < 0.1f) {
			if (tmp3 == false) {
				pre_angle_y = con_angle.y;
				angle_tmp = angle;
				tmp3 = true;
			}
			angle_y = con_angle.y;
			if (dy - (angle_y - pre_angle_y) < -300f) {
				dy = angle_y - pre_angle_y - 360f;
			} else if (dy - (angle_y - pre_angle_y) > 300f) {
				dy = angle_y - pre_angle_y + 360f;
			} else {
				dy = angle_y - pre_angle_y;
			}
			transform.localEulerAngles = new Vector3 (con_angle.x, con_angle.y + dy * gain + angle_tmp, con_angle.z);
		} else {
			transform.localEulerAngles = new Vector3 (con_angle.x, con_angle.y + angle, con_angle.z);
			tmp3 = false;
		}
	}

	void trans_hand(int re,int vr){
		if (vr < 3) {
			center = triangle.z;
			trans_pos (TableManager.Instance.Gain, 90f - TableManager.Instance.Gain * 45f, center, (3 - vr) * 120f - 90f);
			trans_angle (TableManager.Instance.Gain - 1f, vr * 120 - re * 90f);
		} else {
			center = pentagon.z;
			trans_pos (TableManager.Instance.Gain, 90f - TableManager.Instance.Gain * 45f, center, (8 - vr) * 72f - 90f);
			trans_angle (TableManager.Instance.Gain - 1f, (vr - 3) * 72f - re * 90f);
		}
	}
		
}