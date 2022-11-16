using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手の動きを管理
/// </summary>
public class HandTranslate : MonoBehaviour
{
    /// <summary>
    /// 現実でのテーブル中心からトラッカーまでの距離
    /// </summary>
	private float radiusReal;
    /// <summary>
    /// 現実でのテーブル中心から下に下ろした直線と，トラッカーまでの直線のなす角（反時計回り正）
    /// </summary>
	private float thetaReal;
    /// <summary>
    /// VR空間でのテーブル中心からトラッカーまでの距離
    /// </summary>
	private float radiusVirtual;
    /// <summary>
    /// VR空間でのなす角
    /// </summary>
	private float thetaVirtual;

    /// <summary>
    /// トラッカーの位置
    /// </summary>
	private Vector3 realTrackerPos;
    /// <summary>
    /// トラッカーの姿勢
    /// </summary>
	private Vector3 realTrackerRot;
    /// <summary>
    /// 直前の手の位置
    /// </summary>
	private Vector3 handPosBefore;
    /// <summary>
    /// トラッカー
    /// </summary>
	public GameObject target;

    /// <summary>
    /// 四角形のテーブルの中心
    /// </summary>
	private Vector3 SquareCenter
    {
        get
        {
            return Vector3.zero;
        }
    }
    /// <summary>
    /// 三角形のテーブルの中心
    /// </summary>
    private Vector3 TriangleCenter
    {
        get
        {
            return new Vector3(0f, 0f, TableManager.Instance.TableWidth / 2f * (1f / Mathf.Sqrt(3f) - 1f));
        }
    }
    /// <summary>
    /// 五角形のテーブルの中心
    /// </summary>
    private Vector3 PentagonCenter
    {
        get
        {
            return new Vector3(0f, 0f, TableManager.Instance.TableWidth / 2f * (Mathf.Tan(54f * Mathf.Deg2Rad) - 1f));
        }
    }
    /// <summary>
    /// テーブルの中心のz座標
    /// </summary>
	private Vector3 TableCenter
    {
        get
        {
            switch (TableManager.Instance.CurrentShape)
            {
                case TableManager.TableShape.Triangle:
                    return TriangleCenter;
                case TableManager.TableShape.Square:
                default:
                    return SquareCenter;
                case TableManager.TableShape.Pentagon:
                    return PentagonCenter;
            }
        }
    }

    /// <summary>
    /// 一つ前の現実でのトラッカーの位置
    /// </summary>
	//private SquareDirection TrackerDirectionRealBefore = SquareDirection.Edge0;
    /// <summary>
    /// 現在の現実でのトラッカーの位置
    /// </summary>
	private SquareDirection TrackerDirectionReal
    {
        get
        {
            // 現実でのトラッカーの位置を判定
            if (realTrackerPos.z < realTrackerPos.x && realTrackerPos.z < -realTrackerPos.x) return SquareDirection.Edge0;
            if (realTrackerPos.z > realTrackerPos.x && realTrackerPos.z < -realTrackerPos.x) return SquareDirection.Edge90;
            if (realTrackerPos.z > realTrackerPos.x && realTrackerPos.z > -realTrackerPos.x) return SquareDirection.Edge180;
            if (realTrackerPos.z < realTrackerPos.x && realTrackerPos.z > -realTrackerPos.x) return SquareDirection.Edge270;

            return SquareDirection.Verticle;
        }
    }
    /// <summary>
    /// VR空間での一つ前の手の位置
    /// </summary>
	//private int TrackerDirectionVirtualBefore = 0;
    /// <summary>
    /// VR空間での現在の手の位置
    /// 三角形で012，五角形で34567
    /// </summary>
	//private int TrackerDirectionVirtual = 0;
    /// <summary>
    /// VR空間で三角形の時の手の位置
    /// </summary>
    private TriangleDirection TrackerDirectionVirtualTriangle
    {
        get
        {
            if (transform.localPosition.z < transform.localPosition.x / Mathf.Sqrt(3f) + TriangleCenter.z && transform.localPosition.z < -transform.localPosition.x / Mathf.Sqrt(3f) + TriangleCenter.z) return TriangleDirection.Edge0;
            if (transform.localPosition.z > transform.localPosition.x / Mathf.Sqrt(3f) + TriangleCenter.z && transform.localPosition.x < 0f) return TriangleDirection.Edge120;
            if (transform.localPosition.z > -transform.localPosition.x / Mathf.Sqrt(3f) + TriangleCenter.z && transform.localPosition.x > 0f) return TriangleDirection.Edge240;

            return TriangleDirection.Verticle;
        }
    }
    /// <summary>
    /// 1つ前の手の位置
    /// </summary>
    //private TriangleDirection TrackerDirectionVirtualTriangleBefore = TriangleDirection.Edge0;
    /// <summary>
    /// VR空間で五角形の時の手の位置
    /// </summary>
    private PentagonDirection TrackerDirectionVirtualPentagon
    {
        get
        {
            if (transform.localPosition.z < transform.localPosition.x * Mathf.Tan(54f * Mathf.Deg2Rad) + PentagonCenter.z && transform.localPosition.z < -transform.localPosition.x * Mathf.Tan(54f * Mathf.Deg2Rad) + PentagonCenter.z) return PentagonDirection.Edge0;
            if (transform.localPosition.z > transform.localPosition.x * Mathf.Tan(54f * Mathf.Deg2Rad) + PentagonCenter.z && transform.localPosition.z < -transform.localPosition.x * Mathf.Tan(18f * Mathf.Deg2Rad) + PentagonCenter.z) return PentagonDirection.Edge72;
            if (transform.localPosition.z > -transform.localPosition.x * Mathf.Tan(18f * Mathf.Deg2Rad) + PentagonCenter.z && transform.localPosition.x < 0f) return PentagonDirection.Edge144;
            if (transform.localPosition.x > 0f && transform.localPosition.z > transform.localPosition.x * Mathf.Tan(18f * Mathf.Deg2Rad) + PentagonCenter.z) return PentagonDirection.Edge216;
            if (transform.localPosition.z < transform.localPosition.x * Mathf.Tan(18f * Mathf.Deg2Rad) + PentagonCenter.z && transform.localPosition.z > -transform.localPosition.x * Mathf.Tan(54f * Mathf.Deg2Rad) + PentagonCenter.z) return PentagonDirection.Edge288;

            return PentagonDirection.Verticle;
        }
    }
    /// <summary>
    /// 1つ前の手の位置
    /// </summary>
    //private PentagonDirection TrackerDirectionVirtualPentagonBefore = PentagonDirection.Edge0;

    /// <summary>
    /// 現実でのトラッカーの位置が変わったかの判定
    /// </summary>
    /*
	private bool trackerMovedReal
    {
        get
        {
            return TrackerDirectionRealBefore != TrackerDirectionReal;
        }
    }
    */
    /// <summary>
    /// VR空間での手の位置が変わったかの判定
    /// </summary>
    /*
    private bool trackerMovedVirtual
    {
        get
        {
            //return TrackerDirectionVirtualBefore != TrackerDirectionVirtual;
            return TableManager.Instance.CurrentShape == TableManager.TableShape.Pentagon ?
                TrackerDirectionVirtualPentagonBefore != TrackerDirectionVirtualPentagon :
                TrackerDirectionVirtualTriangleBefore != TrackerDirectionVirtualTriangle;
        }
    }
    */

	void Start ()
    {
		target.SetActive(true);
	}

	void Update ()
    {
        // 四角形の場合はそのまま
        if (TableManager.Instance.CurrentShape == TableManager.TableShape.Square)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
        }
		else
        {
			UpdateRealHand(); //テーブルを変形させた時の位置によって変換したトラッカーの座標，姿勢を格納

            // リアルの手の位置を取得
			radiusReal = DistancePlanar(SquareCenter, realTrackerPos);
			thetaReal = AnglePlanar (Vector3.back, realTrackerPos);

            // 位置を更新
			//UpdateVirtualHand(TrackerDirectionRealBefore, TrackerDirectionVirtualBefore);
            if (TableManager.Instance.CurrentShape == TableManager.TableShape.Pentagon)
            {
                //UpdateVirtualHand(TrackerDirectionRealBefore, TrackerDirectionVirtualPentagonBefore);
                UpdateVirtualHand(TrackerDirectionReal, TrackerDirectionVirtualPentagon);
            }
            else
            {
                //UpdateVirtualHand(TrackerDirectionRealBefore, TrackerDirectionVirtualTriangleBefore);
                UpdateVirtualHand(TrackerDirectionReal, TrackerDirectionVirtualTriangle);
            }
		}

        //手が大きく飛んだ時元の位置に戻す
        if (0.15f * 0.15f < (handPosBefore - transform.position).sqrMagnitude)
        {
            ResetVirtualHand();
        }
        //手の位置がおかしくなったときはenter
        if (Input.GetKeyDown (KeyCode.Return))
        {
            ResetVirtualHand();
		}
		
	}
    private void LateUpdate()
    {
        handPosBefore = transform.position;
        //TrackerDirectionRealBefore = TrackerDirectionReal;
        //TrackerDirectionVirtualBefore = TrackerDirectionVirtual;
        //TrackerDirectionVirtualTriangleBefore = TrackerDirectionVirtualTriangle;
        //TrackerDirectionVirtualPentagonBefore = TrackerDirectionVirtualPentagon;
    }

    /// <summary>
    /// 2次元での二つの物体の距離
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <returns></returns>
    float DistancePlanar(Vector3 vec1,Vector3 vec2)
    {	
		Vector2 v1 = new Vector2 (vec1.x, vec1.z);
		Vector2 v2 = new Vector2 (vec2.x, vec2.z);
		return Vector2.Distance (v1, v2);
	}

    /// <summary>
    /// 2次元での二つのベクトルのなす角
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <returns></returns>
    float AnglePlanar(Vector3 vec1,Vector3 vec2)
    {
		Vector2 v1 = new Vector2 (vec1.x, vec1.z);
		Vector2 v2 = new Vector2 (vec2.x, vec2.z);
        //return Vector2.SignedAngle (v1, v2);
        var theta = Vector2.Angle(v1, v2);

        //thetaRealには絶対値しか格納されないので，方向によって正負を判定
        //トラッカーの場所によって90°ずつ値を変更
        if (Vector3.Cross(vec1, vec2).y > 0f) // 右手系でマイナスの時
        {
            // 逆転
            theta *= -1;
            //theta += (float)TrackerDirectionRealBefore;
            theta += (float)TrackerDirectionReal;
        }
        else
        {
            //if (TrackerDirectionRealBefore == SquareDirection.Edge270)
            if (TrackerDirectionReal == SquareDirection.Edge270)
                theta -= 90f;
            else
                //theta -= (float)TrackerDirectionRealBefore;
                theta -= (float)TrackerDirectionReal;
        }

        return theta;
    }

    /// <summary>
    /// リアルの手の方向を南に戻す
    /// </summary>
    /*
    public void ResetRealHandDirection()
    {
        TrackerDirectionRealBefore = 0;
        //TrackerDirectionReal = 0;
    }
    */
    /// <summary>
    /// バーチャルの手の方向を南に戻す
    /// </summary>
    /// <param name="shape"></param>
    /*
    public void ResetVirtualHandDirection(TableManager.TableShape shape = TableManager.TableShape.Square)
    {
        //TrackerDirectionVirtualBefore = shape == TableManager.TableShape.Pentagon ? 3 : 0;
        //TrackerDirectionVirtual = shape == TableManager.TableShape.Pentagon ? 3 : 0;

        //TrackerDirectionVirtualTriangleBefore = TriangleDirection.Edge0;
        //TrackerDirectionVirtualPentagonBefore = PentagonDirection.Edge0;
    }
    */

	/// <summary>
    /// リアルの手の位置を更新する（世界が回転している分だけ回転する）
    /// </summary>
	void UpdateRealHand()
    {
		realTrackerPos = new Vector3 (
            target.transform.position.x * Mathf.Cos ((int)RotationManager.Instance.HMDDirection * Mathf.Deg2Rad) - target.transform.position.z * Mathf.Sin((int)RotationManager.Instance.HMDDirection * Mathf.Deg2Rad),
            target.transform.position.y,
            target.transform.position.x * Mathf.Sin ((int)RotationManager.Instance.HMDDirection * Mathf.Deg2Rad) + target.transform.position.z * Mathf.Cos((int)RotationManager.Instance.HMDDirection * Mathf.Deg2Rad)
            );
		realTrackerRot = new Vector3(
            target.transform.eulerAngles.x,
            target.transform.eulerAngles.y - (int)RotationManager.Instance.HMDDirection,
            target.transform.eulerAngles.z
            );
	}
    /// <summary>
    /// バーチャルの手の位置を更新する
    /// </summary>
    /// <param name="gain"></param>
    /// <param name="tableCenter"></param>
    /// <param name="angle"></param>
	void UpdateVirtualHandPos(float gain, Vector3 tableCenter, float angle)
    {
        var theta = 90f - gain * 45f;
		thetaVirtual = thetaReal * gain;
		radiusVirtual = radiusReal * Mathf.Tan(theta * Mathf.Deg2Rad) * Mathf.Cos(thetaReal * Mathf.Deg2Rad) / Mathf.Cos(thetaVirtual * Mathf.Deg2Rad);
		transform.localPosition = tableCenter + new Vector3 (
            radiusVirtual * Mathf.Cos ((thetaVirtual + angle) * Mathf.Deg2Rad),
            realTrackerPos.y,
            radiusVirtual * Mathf.Sin ((thetaVirtual + angle) * Mathf.Deg2Rad));
	}

    // こいつらはUpdateVirtualHandRotでのみ使用
    /// <summary>
    /// 境界領域に入った時の手の角度
    /// </summary>
    private float rotOnEnteringBoarder = 0f;
    /// <summary>
    /// 領域の境に入った時変換式の回転角度を保持
    /// </summary>
    private float angleOnEnteringBoarder = 0f;
    /// <summary>
    /// エリアの境目に入ったかどうかの判定
    /// </summary>
    private bool hasEntered2Boarder = false;
    /// <summary>
    /// バーチャルの手の角度を更新する
    /// </summary>
    /// <param name="gain"></param>
    /// <param name="angle"></param>
	void UpdateVirtualHandRot(float gain, float angle)
    {
        // 境目（リアルのテーブルの角）を通過した場合
        if (Mathf.Abs (transform.position.x - transform.position.z) < 0.1f * Mathf.Sqrt(2f) || Mathf.Abs (transform.position.x + transform.position.z) < 0.1f * Mathf.Sqrt(2f))
        {
            // 最初のフレームで値を更新
			if (!hasEntered2Boarder)
            {
				rotOnEnteringBoarder = realTrackerRot.y;
				angleOnEnteringBoarder = angle;
				hasEntered2Boarder = true;
			}
            //angle_y = realTrackerRot.y; // angle_yを削除

            // dyを360°の範囲に収める
            var dy = realTrackerRot.y - rotOnEnteringBoarder; //angle_y - pre_angle_y;
            if (dy < 0f) dy += 360f;
            if (360f < dy) dy -= 360f;

			transform.localEulerAngles = new Vector3 (realTrackerRot.x, realTrackerRot.y + dy * gain + angleOnEnteringBoarder, realTrackerRot.z);
		}
        // 通過してない場合（普通）
        else
        {
			transform.localEulerAngles = new Vector3 (realTrackerRot.x, realTrackerRot.y + angle, realTrackerRot.z);
			hasEntered2Boarder = false;
		}
	}
    /// <summary>
    /// バーチャルの手の位置姿勢を更新する
    /// </summary>
    /// <param name="realDir"></param>
    /// <param name="vrDir"></param>
    private void UpdateVirtualHand(SquareDirection realDir, TriangleDirection vrDir)
    {
        UpdateVirtualHandPos(TableManager.Instance.Gain, TableCenter, 270f - (float)vrDir);
        UpdateVirtualHandRot(TableManager.Instance.Gain - 1f, (float)vrDir - (float)realDir);
    }
    /// <summary>
    /// バーチャルの手の位置姿勢を更新する
    /// </summary>
    /// <param name="realDir"></param>
    /// <param name="vrDir"></param>
    private void UpdateVirtualHand(SquareDirection realDir, PentagonDirection vrDir)
    {
        UpdateVirtualHandPos(TableManager.Instance.Gain, TableCenter, 270f - (float)vrDir);
        UpdateVirtualHandRot(TableManager.Instance.Gain - 1f, (float)vrDir - (float)realDir);
    }

    /// <summary>
    /// バーチャルの手の位置をリセットする
    /// </summary>
    private void ResetVirtualHand()
    {
        transform.position = target.transform.position;
    }
}