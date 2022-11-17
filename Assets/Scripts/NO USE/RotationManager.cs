using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HMDの回転を管理する
/// </summary>
public class RotationManager : Singleton<RotationManager>
{
    /// <summary>
    /// 部屋全体
    /// </summary>
	public GameObject wholeRoom;

    /// <summary>
    /// 回転軸
    /// </summary>
    [SerializeField]
	private GameObject rotationPoint45;
    [SerializeField]
    private GameObject rotationPoint135;
    [SerializeField]
    private GameObject rotationPoint225;
    [SerializeField]
    private GameObject rotationPoint315;

    //それぞれの点での，領域に入った時の頭の角度，角度差，その時に回転軸が回転すべき角度 ←プロパティ化できないか？
    float angleOnEnteringCorner45 = 0f;
    float dy45Before = 0f;
    float b1 = 0f;
    float angleOnEnteringCorner135 = 0f;
    float dy135Before = 0f;
    float b2 = 0f;
    float angleOnEnteringCorner225 = 0f;
    float dy225Before = 0f;
    float b3 = 0f;
    float angleOnEnteringCorner315 = 0f;
    float dy315Before = 0f;
    float b4 = 0f;

    /// <summary>
    /// 領域に入った時の回転軸の角度
    /// </summary>
    float rotationPointAngleOnEnteringCorner = 0f;
    /// <summary>
    /// 直線領域でのx方向もしくはz方向の位置
    /// </summary>
	float pos_x = 0f;
    /// <summary>
    /// 領域に入った時の位置
    /// </summary>
    float pre_pos_x = 0f;
    /// <summary>
    /// 移動量
    /// </summary>
    float dx = 0f;
    /// <summary>
    /// 補正領域での補正完了距離
    /// </summary>
	float correct_dis = 0f;

    /// <summary>
    /// テーブルの各角におけるHMDの状態
    /// </summary>
    private enum CornerAction
    {
        JustEntered = 0,       // 回転領域内に丁度入ったところ
        Right2Left = 1,     // 時計回りに通過
        Left2Right = -1,    // 反時計回りに通過
        Right2Right = 2,    // 時計回りで入って戻る
        Left2Left = -2,     // 反時計回りで入って戻る
    }
    private CornerAction action45 = CornerAction.JustEntered;
    private CornerAction action135 = CornerAction.JustEntered;
    private CornerAction action225 = CornerAction.JustEntered;
    private CornerAction action315 = CornerAction.JustEntered;
    /// <summary>
    /// actionの値を保持
    /// </summary>
    private CornerAction action45Cache = CornerAction.JustEntered;
    private CornerAction action135Cache = CornerAction.JustEntered;
    private CornerAction action225Cache = CornerAction.JustEntered;
    private CornerAction action315Cache = CornerAction.JustEntered;

    /// <summary>
    /// 頭の角度
    /// </summary>
	private float redirectedAngle = 0f;

    /// <summary>
    /// skyboxの回転量
    /// </summary>
	private float skyboxRot = 0f;

    /// <summary>
    /// リアル空間のテーブルに対してどの位置にいるかの判定
    /// </summary>
	public SquareDirection HMDDirection
    {
        get
        {
            // 南，西，北，東
            if (transform.position.z < transform.position.x && transform.position.z < -transform.position.x && transform.position.x > -TableManager.Instance.TableWidth / 2f && transform.position.x < TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge0;
            if (transform.position.z > transform.position.x && transform.position.z < -transform.position.x && transform.position.z > -TableManager.Instance.TableWidth / 2f && transform.position.z < TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge90;
            if (transform.position.z > transform.position.x && transform.position.z > -transform.position.x && transform.position.x > -TableManager.Instance.TableWidth / 2f && transform.position.x < TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge180;
            if (transform.position.z < transform.position.x && transform.position.z > -transform.position.x && transform.position.z > -TableManager.Instance.TableWidth / 2f && transform.position.z < TableManager.Instance.TableWidth / 2f) return SquareDirection.Edge270;

            // とりあえずその他は角とまとめておく
            return SquareDirection.Other;
        }
    }

    /// <summary>
    /// 部屋の位置の初期値
    /// </summary>
    public Vector3 InitRoomPos
    {
        private set; get;
    }

	protected override void Awake()
    {
        base.Awake();
        InitRoomPos = wholeRoom.transform.position;
	}

	void Update()
    {
        if (TableManager.Instance.IsChanging)
        {
            UpdateRotationPointsAngle(b1, b2, b3, b4);
            wholeRoom.transform.parent = null;
        }

        // 四角形なら何もしない
        if (TableManager.Instance.CurrentShape == TableManager.TableShape.Square) return;
        
        // 45°の角にいる場合
        if ((transform.position.x < rotationPoint45.transform.position.x && transform.position.z < rotationPoint45.transform.position.z) || (transform.position.z > transform.position.x - 0.15f && transform.position.z < transform.position.x + 0.15f && transform.position.z < -transform.position.x - 0.15f))
        {
            // 現在のアクションの内容を更新
            switch (action45)
            {
                case CornerAction.JustEntered:
                    action45 = transform.position.z < transform.position.x ? CornerAction.Left2Left : CornerAction.Right2Right;

                    ResetActionCache();
                    UpdateRotationPointsAngle(b1, b2, b3, b4);
                    wholeRoom.transform.parent = null;
                    rotationPointAngleOnEnteringCorner = rotationPoint45.transform.eulerAngles.y;
                    angleOnEnteringCorner45 = transform.eulerAngles.y;
                    break;
                case CornerAction.Right2Left:
                    if (transform.position.z < transform.position.x)
                        action45 = CornerAction.Left2Left;
                    break;
                case CornerAction.Left2Right:
                    if (transform.position.z > transform.position.x)
                        action45 = CornerAction.Right2Right;
                    break;
                case CornerAction.Right2Right:
                    if (transform.position.z < transform.position.x)
                        action45 = CornerAction.Left2Right;
                    break;
                case CornerAction.Left2Left:
                    if (transform.position.z > transform.position.x)
                        action45 = CornerAction.Right2Left;
                    break;
                default:
                    break;
            }

            // 回転軸を設定
            wholeRoom.transform.parent = rotationPoint45.transform;

            // 差分角度
            var dy = transform.eulerAngles.y - angleOnEnteringCorner45;

            // 回転角が360°以上変化しないよう調整
            if (dy45Before + 360f < dy) dy -= 360f;
            else if (dy < dy45Before - 360f) dy += 360f;

            // 中心で回転させる
            rotationPoint45.transform.eulerAngles = new Vector3(0f, dy * (1f - TableManager.Instance.Gain) + rotationPointAngleOnEnteringCorner, 0f);

            // スカイボックスの回転を調整
            skyboxRot += (dy45Before - dy) * (1f - TableManager.Instance.Gain);
            RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);

            dy45Before = dy;
        }

        // 135°の角
        else if ((transform.position.x < rotationPoint135.transform.position.x && transform.position.z > rotationPoint135.transform.position.z) || (transform.position.z > -transform.position.x - 0.15f && transform.position.z < -transform.position.x + 0.15f && transform.position.z > transform.position.x + 0.15f))
        {
            switch (action135)
            {
                case 0:
                    action135 = transform.position.z < -transform.position.x ? CornerAction.Left2Left : CornerAction.Right2Right;
                    ResetActionCache();
                    UpdateRotationPointsAngle(b1, b2, b3, b4);
                    wholeRoom.transform.parent = null;
                    rotationPointAngleOnEnteringCorner = rotationPoint135.transform.eulerAngles.y;
                    angleOnEnteringCorner135 = transform.eulerAngles.y;
                    break;
                case CornerAction.Right2Left:
                    if (transform.position.z < -transform.position.x)
                        action135 = CornerAction.Left2Left;
                    break;
                case CornerAction.Left2Right:
                    if (transform.position.z > -transform.position.x)
                        action135 = CornerAction.Right2Right;
                    break;
                case CornerAction.Right2Right:
                    if (transform.position.z < -transform.position.x)
                        action135 = CornerAction.Left2Right;
                    break;
                case CornerAction.Left2Left:
                    if (transform.position.z > -transform.position.x) // 通過したら
                        action135 = CornerAction.Right2Left;
                    break;
                default:
                    break;
            }

            // 回転軸を設定
            wholeRoom.transform.parent = rotationPoint135.transform;

            // 差分角度
            var dy = transform.eulerAngles.y - angleOnEnteringCorner135;

            // 回転角が360°以上変化しないよう調整
            if (dy135Before + 360f < dy) dy -= 360f;
            else if (dy < dy135Before - 360f) dy += 360f;

            // 中心で回転させる
            rotationPoint135.transform.eulerAngles = new Vector3(0f, dy * (1f - TableManager.Instance.Gain) + rotationPointAngleOnEnteringCorner, 0f);

            // スカイボックスの回転を調整
            skyboxRot += (dy135Before - dy) * (1f - TableManager.Instance.Gain);
            RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);

            dy135Before = dy;
        }

        // 225°の角
        else if ((transform.position.x > rotationPoint225.transform.position.x && transform.position.z > rotationPoint225.transform.position.z) || (transform.position.z > transform.position.x - 0.15f && transform.position.z < transform.position.x + 0.15f && transform.position.z > -transform.position.x + 0.15f))
        {
            switch (action225)
            {
                case 0:
                    action225 = transform.position.z > transform.position.x ? CornerAction.Left2Left : CornerAction.Right2Right;

                    ResetActionCache();
                    UpdateRotationPointsAngle(b1, b2, b3, b4);
                    wholeRoom.transform.parent = null;
                    rotationPointAngleOnEnteringCorner = rotationPoint225.transform.eulerAngles.y;
                    angleOnEnteringCorner225 = transform.eulerAngles.y;
                    break;
                case CornerAction.Right2Left:
                    if (transform.position.z > transform.position.x)
                        action225 = CornerAction.Left2Left;
                    break;
                case CornerAction.Left2Right:
                    if (transform.position.z < transform.position.x)
                        action225 = CornerAction.Right2Right;
                    break;
                case CornerAction.Right2Right:
                    if (transform.position.z > transform.position.x)
                        action225 = CornerAction.Left2Right;
                    break;
                case CornerAction.Left2Left:
                    if (transform.position.z < transform.position.x)
                        action225 = CornerAction.Right2Left;
                    break;
                default:
                    break;
            }

            // 回転軸を設定
            wholeRoom.transform.parent = rotationPoint225.transform;

            // 差分角度
            var dy = transform.eulerAngles.y - angleOnEnteringCorner225;

            // 回転角が360°以上変化しないよう調整
            if (dy225Before + 360f < dy) dy -= 360f;
            else if (dy < dy225Before - 360f) dy += 360f;

            // 中心で回転させる
            rotationPoint225.transform.eulerAngles = new Vector3(0f, dy * (1f - TableManager.Instance.Gain) + rotationPointAngleOnEnteringCorner, 0f);

            // スカイボックスの回転を調整
            skyboxRot += (dy225Before - dy) * (1f - TableManager.Instance.Gain);
            RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);

            dy225Before = dy;
        }

        // 315°の角
        else if ((transform.position.x > rotationPoint315.transform.position.x && transform.position.z < rotationPoint315.transform.position.z) || (transform.position.z > -transform.position.x - 0.15f && transform.position.z < -transform.position.x + 0.15f && transform.position.z < transform.position.x - 0.15f))
        {
            switch (action315)
            {
                case 0:
                    if (transform.position.z > -transform.position.x)
                        action315 = CornerAction.Left2Left;
                    else
                        action315 = CornerAction.Right2Right;
                    ResetActionCache();
                    UpdateRotationPointsAngle(b1, b2, b3, b4);
                    wholeRoom.transform.parent = null;
                    rotationPointAngleOnEnteringCorner = rotationPoint315.transform.eulerAngles.y;
                    angleOnEnteringCorner315 = transform.eulerAngles.y;
                    break;
                case CornerAction.Right2Left:
                    if (transform.position.z > -transform.position.x)
                        action315 = CornerAction.Left2Left;
                    break;
                case CornerAction.Left2Right:
                    if (transform.position.z < -transform.position.x)
                        action315 = CornerAction.Right2Right;
                    break;
                case CornerAction.Right2Right:
                    if (transform.position.z > -transform.position.x)
                        action315 = CornerAction.Left2Right;
                    break;
                case CornerAction.Left2Left:
                    if (transform.position.z < -transform.position.x)
                        action315 = CornerAction.Right2Left;
                    break;
                default:
                    break;
            }

            // 回転軸を設定
            wholeRoom.transform.parent = rotationPoint315.transform;

            // 差分角度
            var dy = transform.eulerAngles.y - angleOnEnteringCorner315;

            // 回転角が360°以上変化しないよう調整
            if (dy315Before + 360f < dy) dy -= 360f;
            else if (dy < dy315Before - 360f) dy += 360f;

            // 中心で回転させる
            rotationPoint315.transform.eulerAngles = new Vector3(0f, dy * (1f - TableManager.Instance.Gain) + rotationPointAngleOnEnteringCorner, 0f);

            // スカイボックスの回転を調整
            skyboxRot += (dy315Before - dy) * (1f - TableManager.Instance.Gain);
            RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);

            dy315Before = dy;
        }

        // 直線領域
        else
        {
            // 直線領域に入った時の補正
            // actionのキャッシュを更新（actionBeforeの更新はここでだけ）
            if (action45 != CornerAction.JustEntered)
            {
                action45Cache = action45;
                rotationPointAngleOnEnteringCorner = rotationPoint45.transform.eulerAngles.y;
            }
            if (action135 != CornerAction.JustEntered)
            {
                action135Cache = action135;
                rotationPointAngleOnEnteringCorner = rotationPoint135.transform.eulerAngles.y;
            }
            if (action225 != CornerAction.JustEntered)
            {
                action225Cache = action225;
                rotationPointAngleOnEnteringCorner = rotationPoint225.transform.eulerAngles.y;
            }
            if (action315 != CornerAction.JustEntered)
            {
                action315Cache = action315;
                rotationPointAngleOnEnteringCorner = rotationPoint315.transform.eulerAngles.y;
            }

            // 変化すべき角度b_nの更新（b_nの更新はここでだけ）
            if (action45 == CornerAction.Right2Left || action45 == CornerAction.Left2Right)
            {
                b1 += 90f * (1f - TableManager.Instance.Gain) * (float)action45;
                b1 = RotatedAngle(b1);
            }
            if (action135 == CornerAction.Right2Left || action135 == CornerAction.Left2Right)
            {
                b2 += 90f * (1f - TableManager.Instance.Gain) * (float)action135;
                b2 = RotatedAngle(b2);
            }
            if (action225 == CornerAction.Right2Left || action225 == CornerAction.Left2Right)
            {
                b3 += 90f * (1f - TableManager.Instance.Gain) * (float)action225; // +-1
                b3 = RotatedAngle(b3);
            }
            if (action315 == CornerAction.Right2Left || action315 == CornerAction.Left2Right)
            {
                b4 += 90f * (1f - TableManager.Instance.Gain) * (float)action315;
                b4 = RotatedAngle(b4);
            }

            // 上または下から来た場合
            if (action45 > 0 || action135 < 0 || action225 > 0 || action315 < 0)
            {
                pre_pos_x = transform.position.z;
                correct_dis = TableManager.Instance.TableWidth / 2f + Mathf.Abs(pre_pos_x);
            }
            // 右または左から来た場合
            if (action45 < 0 || action135 > 0 || action225 < 0 || action315 > 0)
            {
                pre_pos_x = transform.position.x;
                correct_dis = TableManager.Instance.TableWidth / 2f + Mathf.Abs(pre_pos_x);
            }

            // actionBeforeの値に応じて環境の角度を変更
            if (action45Cache > 0) // RR, RL
            {
                if (Mathf.Abs(rotationPoint45.transform.eulerAngles.y - b1) > 1f)
                {
                    pos_x = transform.position.z;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b1 - rotationPointAngleOnEnteringCorner < -300f) rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b1 - rotationPointAngleOnEnteringCorner > 300f) rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b1 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    rotationPoint45.transform.eulerAngles += new Vector3(0f, (b1 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint45.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint45.transform.eulerAngles.y - b1) > 0.1f)
                {
                    rotationPoint45.transform.eulerAngles += new Vector3(0f, (b1 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b1 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else rotationPoint45.transform.eulerAngles = new Vector3(0f, b1, 0f);
            }
            if (action45Cache < 0) // LL, LR
            {
                if (Mathf.Abs(rotationPoint45.transform.eulerAngles.y - b1) > 1f)
                {
                    pos_x = transform.position.x;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b1 - rotationPointAngleOnEnteringCorner < -300f) rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b1 - rotationPointAngleOnEnteringCorner > 300f) rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b1 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    rotationPoint45.transform.eulerAngles += new Vector3(0f, (b1 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint45.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint45.transform.eulerAngles.y - b1) > 0.1f)
                {
                    rotationPoint45.transform.eulerAngles += new Vector3(0f, (b1 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b1 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else rotationPoint45.transform.eulerAngles = new Vector3(0f, b1, 0f);
            }
            if (action135Cache > 0)
            {
                if (Mathf.Abs(rotationPoint135.transform.eulerAngles.y - b2) > 1f)
                {
                    pos_x = transform.position.x;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b2 - rotationPointAngleOnEnteringCorner < -300f)
                        rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b2 - rotationPointAngleOnEnteringCorner > 300f)
                        rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b2 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    rotationPoint135.transform.eulerAngles += new Vector3(0f, (b2 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint135.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint135.transform.eulerAngles.y - b2) > 0.1f)
                {
                    rotationPoint135.transform.eulerAngles += new Vector3(0f, (b2 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b2 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else
                    rotationPoint135.transform.eulerAngles = new Vector3(0f, b2, 0f);
            }
            if (action135Cache < 0)
            {
                if (Mathf.Abs(rotationPoint135.transform.eulerAngles.y - b2) > 1f)
                {
                    pos_x = transform.position.z;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b2 - rotationPointAngleOnEnteringCorner < -300f)
                        rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b2 - rotationPointAngleOnEnteringCorner > 300f)
                        rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b2 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    rotationPoint135.transform.eulerAngles += new Vector3(0f, (b2 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint135.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint135.transform.eulerAngles.y - b2) > 0.1f)
                {
                    rotationPoint135.transform.eulerAngles += new Vector3(0f, (b2 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b2 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else
                    rotationPoint135.transform.eulerAngles = new Vector3(0f, b2, 0f);
            }
            if (action225Cache > 0)
            {
                if (Mathf.Abs(rotationPoint225.transform.eulerAngles.y - b3) > 1f)
                {
                    pos_x = transform.position.z;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b3 - rotationPointAngleOnEnteringCorner < -300f)
                        rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b3 - rotationPointAngleOnEnteringCorner > 300f)
                        rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b3 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    rotationPoint225.transform.eulerAngles += new Vector3(0f, (b3 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint225.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint225.transform.eulerAngles.y - b3) > 0.1f)
                {
                    rotationPoint225.transform.eulerAngles += new Vector3(0f, (b3 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b3 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else
                    rotationPoint225.transform.eulerAngles = new Vector3(0f, b3, 0f);
            }
            if (action225Cache < 0)
            {
                if (Mathf.Abs(rotationPoint225.transform.eulerAngles.y - b3) > 1f)
                {
                    pos_x = transform.position.x;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b3 - rotationPointAngleOnEnteringCorner < -300f)
                        rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b3 - rotationPointAngleOnEnteringCorner > 300f)
                        rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b3 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    rotationPoint225.transform.eulerAngles += new Vector3(0f, (b3 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint225.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint225.transform.eulerAngles.y - b3) > 0.1f)
                {
                    rotationPoint225.transform.eulerAngles += new Vector3(0f, (b3 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b3 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else
                    rotationPoint225.transform.eulerAngles = new Vector3(0f, b3, 0f);
            }
            if (action315Cache > 0)
            {
                if (Mathf.Abs(rotationPoint315.transform.eulerAngles.y - b4) > 1f)
                {
                    pos_x = transform.position.x;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b4 - rotationPointAngleOnEnteringCorner < -300f)
                        rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b4 - rotationPointAngleOnEnteringCorner > 300f)
                        rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b4 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    rotationPoint315.transform.eulerAngles += new Vector3(0f, (b4 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint315.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint315.transform.eulerAngles.y - b4) > 0.1f)
                {
                    rotationPoint315.transform.eulerAngles += new Vector3(0f, (b4 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b4 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else
                    rotationPoint315.transform.eulerAngles = new Vector3(0f, b4, 0f);
            }
            if (action315Cache < 0)
            {
                if (Mathf.Abs(rotationPoint315.transform.eulerAngles.y - b4) > 1f)
                {
                    pos_x = transform.position.z;
                    dx = Mathf.Abs(pre_pos_x - pos_x);
                    if (b4 - rotationPointAngleOnEnteringCorner < -300f)
                        rotationPointAngleOnEnteringCorner -= 360f;
                    else if (b4 - rotationPointAngleOnEnteringCorner > 300f)
                        rotationPointAngleOnEnteringCorner += 360f;
                    skyboxRot -= (b4 - rotationPointAngleOnEnteringCorner) / TableManager.Instance.TableWidth * dx;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                    rotationPoint315.transform.eulerAngles += new Vector3(0f, (b4 - rotationPointAngleOnEnteringCorner) / correct_dis * dx, 0f);
                    pre_pos_x = pos_x;
                    redirectedAngle = rotationPoint315.transform.eulerAngles.y;
                }
                else if (Mathf.Abs(rotationPoint315.transform.eulerAngles.y - b4) > 0.1f)
                {
                    rotationPoint315.transform.eulerAngles += new Vector3(0f, (b4 - redirectedAngle) * Time.deltaTime * 5f, 0f);
                    skyboxRot -= (b4 - redirectedAngle) * Time.deltaTime;
                    RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);
                }
                else
                    rotationPoint315.transform.eulerAngles = new Vector3(0f, b4, 0f);
            }

            // フラグを解消
            action45 = CornerAction.JustEntered;
            action135 = CornerAction.JustEntered;
            action225 = CornerAction.JustEntered;
            action315 = CornerAction.JustEntered;
        }
    }

    /// <summary>
    /// 部屋のTransformを初期化する
    /// </summary>
    public void InitializeRoom()
    {
        wholeRoom.transform.position = InitRoomPos;
        wholeRoom.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// キャッシュされたアクションのデータを元に戻す
    /// </summary>
    private void ResetActionCache()
    {
        action45Cache = CornerAction.JustEntered;
        action135Cache = CornerAction.JustEntered;
        action225Cache = CornerAction.JustEntered;
        action315Cache = CornerAction.JustEntered;
    }
    /// <summary>
    /// アクションのデータのキャッシュを更新
    /// </summary>
    private void UpdateActionCache()
    {
        action45Cache = action45;
        action135Cache = action135;
        action225Cache = action225;
        action315Cache = action315;
    }
    /// <summary>
    /// テーブルの柱の回転角を更新する
    /// </summary>
    /// <param name="angle45"></param>
    /// <param name="angle135"></param>
    /// <param name="angle225"></param>
    /// <param name="angle315"></param>
    private void UpdateRotationPointsAngle(float angle45, float angle135, float angle225, float angle315)
    {
        rotationPoint45.transform.eulerAngles = new Vector3(0f, angle45, 0f);
        rotationPoint135.transform.eulerAngles = new Vector3(0f, angle135, 0f);
        rotationPoint225.transform.eulerAngles = new Vector3(0f, angle225, 0f);
        rotationPoint315.transform.eulerAngles = new Vector3(0f, angle315, 0f);
    }

    /// <summary>
    /// 360°以内に収まるよう角度表現を補正する
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float RotatedAngle(float angle)
    {
        if (angle < 0) return angle + 360f;
        if (360f < angle) return angle - 360f;
        return angle;
    }
}
