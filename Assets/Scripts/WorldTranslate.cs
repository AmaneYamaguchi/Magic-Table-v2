using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン全体の回転角度の管理をする
/// </summary>
public class WorldTranslate : Singleton<WorldTranslate>
{
    /// <summary>
    /// 部屋全体
    /// </summary>
	public Transform Environment;

    /// <summary>
    /// 回転軸（左手前）
    /// </summary>
    [SerializeField]
    private Transform corner45;
    /// <summary>
    /// 回転軸（左奥）
    /// </summary>
    [SerializeField]
    private Transform corner135;
    /// <summary>
    /// 回転軸（右奥）
    /// </summary>
    [SerializeField]
    private Transform corner225;
    /// <summary>
    /// 回転軸（右手前）
    /// </summary>
    [SerializeField]
    private Transform corner315;

    [SerializeField]
    private Transform HMD;
    /// <summary>
    /// 1F前のHMDの位置
    /// </summary>
    private Vector3 HMDPositionBefore = Vector3.zero;
    /// <summary>
    /// HMDの移動量
    /// </summary>
    public Vector3 HMDDeltaPosition
    {
        get
        {
            return HMD.position - HMDPositionBefore;
        }
    }
    /// <summary>
    /// HMDが辺で移動した距離
    /// HMDが角に入った時にリセットする
    /// </summary>
    private Vector3 HMDMovedLengthInEdge = Vector3.zero;
    private Vector3 HMDMovedLengthInPastEdge = Vector3.zero;

    /// <summary>
    /// テーブルの上でのHMDの方角判定の閾値
    /// </summary>
    [SerializeField]
    private float angleThresholdInsideTable = 15f;
    /// <summary>
    /// テーブルの内外判定をする時の半径（後で図作る）
    /// </summary>
    private float radiusInsideTable
    {
        get
        {
            return (TableManager.Instance.TableWidth / 2f) / Mathf.Cos((45f + angleThresholdInsideTable / 2f) * Mathf.Deg2Rad);
        }
    }
    /// <summary>
    /// テーブルの内側での角度判定
    /// </summary>
    private float GainInsideTable
    {
        get
        {
            return HMDIsInsideTable ? -90f / angleThresholdInsideTable : 1f;
        }
    }

    /// <summary>
    /// リアル空間のテーブルに対してどの位置にいるかの判定
    /// 4方向
    /// </summary>
    public SquareDirection HMDDirectionRough
    {
        get
        {
            // 南，西，北，東
            if (HMD.position.z <= HMD.position.x && HMD.position.z <= -HMD.position.x && HMD.position.x >= -TableManager.Instance.TableWidth / 2f && HMD.position.x <= TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge0;
            if (HMD.position.z >= HMD.position.x && HMD.position.z <= -HMD.position.x && HMD.position.z >= -TableManager.Instance.TableWidth / 2f && HMD.position.z <= TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge90;
            if (HMD.position.z >= HMD.position.x && HMD.position.z >= -HMD.position.x && HMD.position.x >= -TableManager.Instance.TableWidth / 2f && HMD.position.x <= TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge180;
            if (HMD.position.z <= HMD.position.x && HMD.position.z >= -HMD.position.x && HMD.position.z >= -TableManager.Instance.TableWidth / 2f && HMD.position.z <= TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge270;

            // その他
            return SquareDirection.Other;
        }
    }
    /// <summary>
    /// リアル空間のテーブルに対してどの位置にいるかの判定
    /// 8方向
    /// </summary>
    public SquareDirection HMDDirection
    {
        get
        {
            // HMDの極座標を求める
            Vector3 to = HMD.position;
            to.y = 0f;
            float signedAngle = Vector3.SignedAngle(Vector3.back, to, Vector3.up);
            signedAngle = (0f < signedAngle) ? signedAngle : signedAngle + 360f;

            // 南西，北西，北東，南東
            // テーブル外側の矩形にいる，または，テーブル内側の扇形にいる
            if ((HMD.position.x < -TableManager.Instance.TableWidth / 2f && HMD.position.z < -TableManager.Instance.TableWidth / 2f)
                //|| (45f - angleThresholdInsideTable / 2f < signedAngle && signedAngle < 45f + angleThresholdInsideTable / 2f)
                || (IsInsideRedirectedArea(corner45, HMD, angleThresholdInsideTable / 2f))
                ) return SquareDirection.Corner45;
            if ((HMD.position.x < -TableManager.Instance.TableWidth / 2f && TableManager.Instance.TableWidth / 2f < HMD.position.z)
                || (IsInsideRedirectedArea(corner135, HMD, angleThresholdInsideTable / 2f))
                ) return SquareDirection.Corner135;
            if ((TableManager.Instance.TableWidth / 2f < HMD.position.x && TableManager.Instance.TableWidth / 2f < HMD.position.z)
                || (IsInsideRedirectedArea(corner225, HMD, angleThresholdInsideTable / 2f))
                ) return SquareDirection.Corner225;
            if ((TableManager.Instance.TableWidth / 2f < HMD.position.x && HMD.position.z < -TableManager.Instance.TableWidth / 2f)
                || (IsInsideRedirectedArea(corner315, HMD, angleThresholdInsideTable / 2f))
                ) return SquareDirection.Corner315;

            // 東西南北は粗いバージョンと同じ
            return HMDDirectionRough;
        }
    }
    /// <summary>
    /// 1F前のHMDの方角
    /// </summary>
    private SquareDirection HMDDirectionBefore;
    /// <summary>
    /// HMDが1つ前にいた辺
    /// </summary>
    private SquareDirection HMDEdgeBefore;
    
    public bool HMDIsInEdge
    {
        get
        {
            //return HMDDirection == (SquareDirection.Edge0 | SquareDirection.Edge90 | SquareDirection.Edge180 | SquareDirection.Edge270);
            return HMDDirection == SquareDirection.Edge0 ||
                HMDDirection == SquareDirection.Edge90 ||
                HMDDirection == SquareDirection.Edge180 ||
                HMDDirection == SquareDirection.Edge270;
        }
    }
    public bool HMDIsInCorner
    {
        get
        {
            //return HMDDirection == (SquareDirection.Corner45 | SquareDirection.Corner135 | SquareDirection.Corner225 | SquareDirection.Corner315);
            return HMDDirection == SquareDirection.Corner45 ||
                HMDDirection == SquareDirection.Corner135 ||
                HMDDirection == SquareDirection.Corner225 ||
                HMDDirection == SquareDirection.Corner315;
        }
    }
    public bool HMDWasInEdge
    {
        get
        {
            //return HMDDirectionBefore == (SquareDirection.Edge0 | SquareDirection.Edge90 | SquareDirection.Edge180 | SquareDirection.Edge270);
            return HMDDirectionBefore == SquareDirection.Edge0 ||
                HMDDirectionBefore == SquareDirection.Edge90 ||
                HMDDirectionBefore == SquareDirection.Edge180 ||
                HMDDirectionBefore == SquareDirection.Edge270;
        }
    }
    public bool HMDWasInCorner
    {
        get
        {
            //return HMDDirectionBefore == (SquareDirection.Corner45 | SquareDirection.Corner135 | SquareDirection.Corner225 | SquareDirection.Corner315);
            return HMDDirectionBefore == SquareDirection.Corner45 ||
                HMDDirectionBefore == SquareDirection.Corner135 ||
                HMDDirectionBefore == SquareDirection.Corner225 ||
                HMDDirectionBefore == SquareDirection.Corner315;
        }
    }
    public bool HMDIsInsideTable
    {
        get
        {
            return Mathf.Abs(HMD.position.x) < TableManager.Instance.TableWidth / 2f
                && Mathf.Abs(HMD.position.z) < TableManager.Instance.TableWidth / 2f;
        }
    }

    /// <summary>
    /// HMDに回転ゲインがかかっているかどうか
    /// </summary>
    private bool HMDIsRotating
    {
        get
        {
            return CurrentCorner != null;
        }
    }

    /// <summary>
    /// 部屋の位置の初期値
    /// </summary>
    public Vector3 InitRoomPos
    {
        private set; get;
    }
    
    /// <summary>
    /// （HMDが角にいたとして）HMDが現在いる角
    /// 45，135，225，315
    /// </summary>
    public Transform CurrentCorner
    {
        private set; get;
    }
    /// <summary>
    /// 角に入った時のHMDの位置
    /// 角内での変化角度を測るために使用
    /// </summary>
    private Vector3 HMDPositionOnEnteringCorner = Vector3.zero;
    /// <summary>
    /// 角に入った時のHMDの角度 y座標を回転のゲインに反映させる
    /// </summary>
    private Vector3 HMDRotationOnEnteringCorner = Vector3.zero;
    /// <summary>
    /// HMDが角にいる時入ってからのHMDの角度変化
    /// 符号付き -180~180
    /// </summary>
    private float diffAngleInCorner
    {
        get
        {
            if (CurrentCorner == null)
            {
                Debug.LogWarning("WorldTranslate: currentCorner is not set!");
                return 0f;
            }

            // 回転
            float currentY = HMD.rotation.eulerAngles.y;
            float diffY = currentY - HMDRotationOnEnteringCorner.y;

            // -90～90を取るよう補正
            if (180f < diffY) diffY -= 360f;

            return diffY;
        }
    }
    /// <summary>
    /// 1F前のdiffAngleInCorner
    /// </summary>
    private float diffAngleInCornerBefore = 0f;
    /// <summary>
    /// HMDが角から辺に来た後のHMDの仮想的な角度変化
    /// </summary>
    private float diffAngleInEdge
    {
        get
        {
            // 方向指定
            float targetDiffAngle = HMDDirection - HMDEdgeBefore;
            if (180f < targetDiffAngle) targetDiffAngle -= 360f;
            if (targetDiffAngle < -180) targetDiffAngle += 360f;
            float direction = diffAngleInCorner - targetDiffAngle > 0f ? -1f : 1f;

            Vector3 dr = HMDMovedLengthInEdge;
            float dz = HMDDirection == SquareDirection.Edge0 || HMDDirection == SquareDirection.Edge180 ? dr.x : dr.z;
            Vector3 drPast = HMDMovedLengthInPastEdge;
            float dx = HMDDirection == SquareDirection.Edge90 || HMDDirection == SquareDirection.Edge270 ? drPast.x : drPast.z;

            return 90f * direction * (dz - dx) / TableManager.Instance.TableWidth * 2f;
        }
    }
    private float diffAngleInEdgeBefore = 0f;
    /// <summary>
    /// 環境が変化すべき角度
    /// </summary>
    private float redirectedDiffAngleInCorner
    {
        get
        {
            // 今のテーブルのゲイン - リアルテーブルのゲイン
            float tableGain = TableManager.Instance.Gain - 1f;
            return tableGain * (diffAngleInCorner + diffAngleInEdge); // diff pos in edge も追加したい
        }
    }
    /// <summary>
    /// HMDが角に入った時の環境の角度
    /// </summary>
    private float environmentAngleOnEnteringCorner = 0f;

    protected override void Awake()
    {
        base.Awake();
        InitRoomPos = Environment.transform.position;
    }

    private void Update()
    {
        // HMDがリアルテーブルの角（90°）に入った時に環境を角の子にする
        if (HMDWasInEdge && HMDIsInCorner && !HMDIsRotating)
        {
            switch (HMDDirection)
            {
                case SquareDirection.Corner45:
                    CurrentCorner = corner45;
                    break;
                case SquareDirection.Corner135:
                    CurrentCorner = corner135;
                    break;
                case SquareDirection.Corner225:
                    CurrentCorner = corner225;
                    break;
                case SquareDirection.Corner315:
                    CurrentCorner = corner315;
                    break;
                default:
                    CurrentCorner = null;
                    break;
            }
            Environment.parent = CurrentCorner;
            HMDPositionOnEnteringCorner = HMD.position;
            HMDRotationOnEnteringCorner = HMD.rotation.eulerAngles;

            // -180~180を取るように補正
            if (180f < HMDRotationOnEnteringCorner.y) HMDRotationOnEnteringCorner.y -= 360f;
            if (HMDRotationOnEnteringCorner.y < -180f) HMDRotationOnEnteringCorner.y += 360f;

            environmentAngleOnEnteringCorner = CurrentCorner.eulerAngles.y;
            HMDEdgeBefore = HMDDirectionBefore;

            // 移動距離のリセット
            ResetMovedLengthInEdge();
        }

        // HMDのリアル位置に対し，角の角度をイイ感じに足す // 位置ではなく角度変化だった
        if (HMDIsRotating)
        {
            CurrentCorner.eulerAngles = new Vector3(0f, environmentAngleOnEnteringCorner - redirectedDiffAngleInCorner, 0f);
        }

        // HMDが角から出て，かつHMDの回転差分が所定の角度なら，環境をルートの子に戻す
        if (HMDIsRotating && HMDIsInEdge)
        {
            // 所定の角度だけ回転したか判定
            float targetDiffAngle = HMDDirection - HMDEdgeBefore;
            if (180f < targetDiffAngle) targetDiffAngle -= 360f;
            if (targetDiffAngle < -180f) targetDiffAngle += 360f;
            if ((diffAngleInCorner + diffAngleInEdge - targetDiffAngle) * (diffAngleInCornerBefore + diffAngleInEdgeBefore - targetDiffAngle) > 0f) return;

            // 以下では回転が完了した場合の後処理
            // 回転角度を-90°，0°，90°のどれかに丸める
            var angle = (TableManager.Instance.Gain - 1f) * ((diffAngleInCorner + diffAngleInEdge <= -45) ? -90f : (diffAngleInCorner + diffAngleInEdge < 45f) ? 0f : 90f);
            CurrentCorner.eulerAngles = new Vector3(0f, environmentAngleOnEnteringCorner - angle, 0f);

            Environment.parent = null;
            CurrentCorner = null;
        }
    }

    private void LateUpdate()
    {
        diffAngleInEdgeBefore = diffAngleInEdge;
        
        // HMDの移動距離を加算する
        Vector3 absDeltaPos = HMDDeltaPosition;
        absDeltaPos.x = Mathf.Abs(absDeltaPos.x);
        absDeltaPos.y = Mathf.Abs(absDeltaPos.y);
        absDeltaPos.z = Mathf.Abs(absDeltaPos.z);
        if (HMDIsInEdge && HMDDirection != HMDEdgeBefore)
        {
            HMDMovedLengthInEdge += absDeltaPos;
        }
        if (HMDIsInEdge && HMDDirection == HMDEdgeBefore)
        {
            HMDMovedLengthInPastEdge += absDeltaPos;
        }

        // 1F前の値たちを更新する
        HMDPositionBefore = HMD.position;
        HMDDirectionBefore = HMDDirection;
        diffAngleInCornerBefore = diffAngleInCorner;
    }

    /// <summary>
    /// テーブルの角付近にいるHMDがテーブルの内部のリダイレクションの範囲にいるかどうか判定する
    /// </summary>
    /// <param name="corner"></param>
    /// <param name="hmd"></param>
    /// <param name="halfThresholdAngle"></param>
    /// <returns></returns>
    private bool IsInsideRedirectedArea(Transform corner, Transform hmd, float halfThresholdAngle)
    {
        // テーブルの外側にいたらだめ
        if (TableManager.Instance.TableWidth / 2f < Mathf.Abs(hmd.position.x) || TableManager.Instance.TableWidth / 2f < Mathf.Abs(hmd.position.z)) return false;

        // 指定した角でない象限にいたらだめ
        if (corner.position.x * hmd.position.x < 0f || corner.position.z * hmd.position.z < 0f) return false;

        Vector3 from = -corner.position;
        from.y = 0f;
        Vector3 to = hmd.position - corner.position;
        to.y = 0f;
        float angle = Vector3.Angle(from, to);

        return angle <= halfThresholdAngle;
    }
    /// <summary>
    /// 回転中心となる角を変更する
    /// </summary>
    /// <param name="dir"></param>
    public void ResetCurrentCorner(Transform corner = null)
    {
        CurrentCorner = corner;
    }
    /// <summary>
    /// 移動距離をリセットする
    /// </summary>
    public void ResetMovedLengthInEdge()
    {
        HMDMovedLengthInEdge = Vector3.zero;
        HMDMovedLengthInPastEdge = Vector3.zero;
        Debug.Log($"Reset Moved Length In Edge");
    }
}
