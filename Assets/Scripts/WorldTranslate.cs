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
            // 南西，北西，北東，南東
            if ((HMD.position.x < corner45.position.x && HMD.position.z < corner45.position.z)
                || (HMD.position.z > HMD.position.x - 0.15f && HMD.position.z < HMD.position.x + 0.15f && HMD.position.z < -HMD.position.x - 0.15f)) return SquareDirection.Corner45;
            if ((HMD.position.x < corner135.position.x && HMD.position.z > corner135.position.z)
                || (HMD.position.z > -HMD.position.x - 0.15f && HMD.position.z < -HMD.position.x + 0.15f && HMD.position.z > HMD.position.x + 0.15f)) return SquareDirection.Corner135;
            if ((HMD.position.x > corner225.position.x && HMD.position.z > corner225.position.z)
                || (HMD.position.z > HMD.position.x - 0.15f && HMD.position.z < HMD.position.x + 0.15f && HMD.position.z > -HMD.position.x + 0.15f)) return SquareDirection.Corner225;
            if ((HMD.position.x > corner315.position.x && HMD.position.z < corner315.position.z)
                || (HMD.position.z > -HMD.position.x - 0.15f && HMD.position.z < -HMD.position.x + 0.15f && HMD.position.z < HMD.position.x - 0.15f)) return SquareDirection.Corner315;

            // 東西南北は粗いバージョンと同じ
            return HMDDirectionRough;
        }
    }
    /// <summary>
    /// 1F前のHMDの方角
    /// </summary>
    private SquareDirection HMDDirectionBefore;
    
    private bool HMDIsInEdge
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
    private bool HMDIsInCorner
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
    private bool HMDWasInEdge
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
    private bool HMDWasInCorner
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
    private Transform currentCorner
    {
        set; get;
    }
    /// <summary>
    /// 1F前のHMDの位置
    /// 角内での変化角度を測るために使用
    /// </summary>
    private Vector3 HMDPositionBefore = Vector3.zero;
    /// <summary>
    /// HMDが角にいる時の，1Fあたりの変化角度
    /// 符号付き -90～90
    /// </summary>
    public float diffAngleInCorner
    {
        get
        {
            if (currentCorner == null)
            {
                Debug.LogWarning("WorldTranslate: currentCorner is not set!");
                return 0f;
            }

            // 変数設定
            Vector3 cornerPos = currentCorner.position;

            Vector3 from = HMDPositionBefore - cornerPos;
            from.y = 0f;
            Vector3 to = HMD.position - cornerPos;
            to.y = 0f;

            return Vector3.SignedAngle(from, to, Vector3.up);
        }
    }
    /// <summary>
    /// 環境が変化すべき角度
    /// </summary>
    public float redirectedDiffAngleInCorner
    {
        get
        {
            float tableGain = TableManager.Instance.Gain - 1f;
            return tableGain * diffAngleInCorner;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InitRoomPos = Environment.transform.position;
    }

    private void Update()
    {
        // 四角形の場合はそのまま
        //if (TableManager.Instance.CurrentShape == TableManager.TableShape.Square) return;

        // 三角形または五角形の場合は
        // HMDがリアルテーブルの角（90°）に入った時に環境を角の子にする
        if (HMDWasInEdge && HMDIsInCorner)
        {
            switch (HMDDirection)
            {
                case SquareDirection.Corner45:
                    currentCorner = corner45;
                    Environment.parent = corner45;
                    break;
                case SquareDirection.Corner135:
                    currentCorner = corner135;
                    Environment.parent = corner135;
                    break;
                case SquareDirection.Corner225:
                    currentCorner = corner225;
                    Environment.parent = corner225;
                    break;
                case SquareDirection.Corner315:
                    currentCorner = corner315;
                    Environment.parent = corner315;
                    break;
                default:
                    break;
            }
        }

        // HMDのリアル位置に対し，角の角度をイイ感じに足す
        if (HMDIsInCorner)
        {
            currentCorner.eulerAngles -= new Vector3(0f, redirectedDiffAngleInCorner, 0f);

            // TODO：テーブル内部のX字型領域についての考慮をする
        }

        // HMDが角から出たら，環境をルートの子に戻す
        if (HMDWasInCorner && HMDIsInEdge)
        {
            Environment.parent = transform.root;
            currentCorner = null;
        }
    }

    private void LateUpdate()
    {
        HMDDirectionBefore = HMDDirectionRough;
        HMDPositionBefore = HMD.position;
    }
}
