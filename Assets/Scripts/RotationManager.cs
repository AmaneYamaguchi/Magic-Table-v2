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
    float pre_angle_y1 = 0f;
    float dy1 = 0f;
    float b1 = 0f;
    float pre_angle_y2 = 0f;
    float dy2 = 0f;
    float b2 = 0f;
    float pre_angle_y3 = 0f;
    float dy3 = 0f;
    float b3 = 0f;
    float pre_angle_y4 = 0f;
    float dy4 = 0f;
    float b4 = 0f;

    /// <summary>
    /// 領域に入った時の回転軸の角度
    /// </summary>
    float init_angle = 0f;
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
        InCorner = 0,       // 回転領域内
        Right2Left = 1,     // 時計回りに通過
        Left2Right = -1,    // 反時計回りに通過
        Right2Right = 2,    // 時計回りで入って戻る
        Left2Left = -2,     // 反時計回りで入って戻る
    }
    private CornerAction action45 = CornerAction.InCorner;
    private CornerAction action135 = CornerAction.InCorner;
    private CornerAction action225 = CornerAction.InCorner;
    private CornerAction action315 = CornerAction.InCorner;
    /// <summary>
    /// actionの値を保持
    /// </summary>
    private CornerAction action45Before = CornerAction.InCorner;
    private CornerAction action135Before = CornerAction.InCorner;
    private CornerAction action225Before = CornerAction.InCorner;
    private CornerAction action315Before = CornerAction.InCorner;

    /// <summary>
    /// 頭の角度？
    /// </summary>
	private float angle_y = 0f;

    /// <summary>
    /// skyboxの回転量
    /// </summary>
	private float skyboxRot = 0f;

    /// <summary>
    /// テーブルに対してどの位置にいるかの判定
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
            return SquareDirection.Verticle;
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
            rotationPoint45.transform.eulerAngles = new Vector3(0f, b1, 0f);
            rotationPoint135.transform.eulerAngles = new Vector3(0f, b2, 0f);
            rotationPoint225.transform.eulerAngles = new Vector3(0f, b3, 0f);
            rotationPoint315.transform.eulerAngles = new Vector3(0f, b4, 0f);
            wholeRoom.transform.parent = null;
        }

        // 四角形でない場合
		if (TableManager.Instance.CurrentShape != TableManager.TableShape.Square) {
			//回転領域に入った時の処理
            
            // 45°の角
			if ((transform.position.x < rotationPoint45.transform.position.x && transform.position.z < rotationPoint45.transform.position.z) || (transform.position.z > transform.position.x - 0.15f && transform.position.z < transform.position.x + 0.15f && transform.position.z < -transform.position.x - 0.15f)) {
				switch (action45)
                {
				    case CornerAction.InCorner:
					    if (transform.position.z < transform.position.x)
						    action45 = CornerAction.Left2Left;
					    else
						    action45 = CornerAction.Right2Right;
					    action45Before = 0;
					    action135Before = 0;
					    action225Before = 0;
					    action315Before = 0;
					    rotationPoint45.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					    rotationPoint135.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					    rotationPoint225.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					    rotationPoint315.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					    wholeRoom.transform.parent = null;
					    init_angle = rotationPoint45.transform.eulerAngles.y;
					    pre_angle_y1 = transform.eulerAngles.y;
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
				wholeRoom.transform.parent = rotationPoint45.transform;
				var angle_y1 = transform.eulerAngles.y;
				//回転角が360°以上変化しないよう調整
				if (dy1 - (angle_y1 - pre_angle_y1) < -300f) {
					skyboxRot += (dy1 - (angle_y1 - pre_angle_y1) + 360f) * (1f - TableManager.Instance.Gain);
					dy1 = angle_y1 - pre_angle_y1 - 360f;
				}
                else if (dy1 - (angle_y1 - pre_angle_y1) > 300f)
                {
					skyboxRot += (dy1 - (angle_y1 - pre_angle_y1) - 360f) * (1f - TableManager.Instance.Gain);
					dy1 = angle_y1 - pre_angle_y1 + 360f;
				}
                else
                {
					skyboxRot += (dy1 - (angle_y1 - pre_angle_y1)) * (1f - TableManager.Instance.Gain);
					dy1 = angle_y1 - pre_angle_y1;
				}
				rotationPoint45.transform.eulerAngles = new Vector3 (0f, dy1 * (1f - TableManager.Instance.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
			}
            
            // 135°の角
            else if ((transform.position.x < rotationPoint135.transform.position.x && transform.position.z > rotationPoint135.transform.position.z) || (transform.position.z > -transform.position.x - 0.15f && transform.position.z < -transform.position.x + 0.15f && transform.position.z > transform.position.x + 0.15f))
            {
				switch (action135) {
				case 0:
					if (transform.position.z < -transform.position.x)
						action135 = CornerAction.Left2Left;
					else
						action135 = CornerAction.Right2Right;
					action45Before = 0;
					action135Before = 0;
					action225Before = 0;
					action315Before = 0;
					rotationPoint45.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationPoint135.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationPoint225.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationPoint315.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationPoint135.transform.eulerAngles.y;
					pre_angle_y2 = transform.eulerAngles.y;
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
					if (transform.position.z > -transform.position.x)
						action135 = CornerAction.Right2Left;
					break;
				default:
					break;
				}
				wholeRoom.transform.parent = rotationPoint135.transform;
				var angle_y2 = transform.eulerAngles.y;
				if (dy2 - (angle_y2 - pre_angle_y2) < -300f) {
					skyboxRot += (dy2 - (angle_y2 - pre_angle_y2) + 360f) * (1f - TableManager.Instance.Gain);
					dy2 = angle_y2 - pre_angle_y2 - 360f;
				} else if (dy2 - (angle_y2 - pre_angle_y2) > 300f) {
					skyboxRot += (dy2 - (angle_y2 - pre_angle_y2) - 360f) * (1f - TableManager.Instance.Gain);
					dy2 = angle_y2 - pre_angle_y2 + 360f;
				} else {
					skyboxRot += (dy2 - (angle_y2 - pre_angle_y2)) * (1f - TableManager.Instance.Gain);
					dy2 = angle_y2 - pre_angle_y2;
				}
				rotationPoint135.transform.eulerAngles = new Vector3 (0f, dy2 * (1f - TableManager.Instance.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
			}

            // 225°の角
            else if ((transform.position.x > rotationPoint225.transform.position.x && transform.position.z > rotationPoint225.transform.position.z) || (transform.position.z > transform.position.x - 0.15f && transform.position.z < transform.position.x + 0.15f && transform.position.z > -transform.position.x + 0.15f))
            {
				switch (action225) {
				case 0:
					if (transform.position.z > transform.position.x)
						action225 = CornerAction.Left2Left;
					else
						action225 = CornerAction.Right2Right;
					action45Before = 0;
					action135Before = 0;
					action225Before = 0;
					action315Before = 0;
					rotationPoint45.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationPoint135.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationPoint225.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationPoint315.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationPoint225.transform.eulerAngles.y;
					pre_angle_y3 = transform.eulerAngles.y;
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
				wholeRoom.transform.parent = rotationPoint225.transform;
				var angle_y3 = transform.eulerAngles.y;
				if (dy3 - (angle_y3 - pre_angle_y3) < -300f) {
					skyboxRot += (dy3 - (angle_y3 - pre_angle_y3) + 360f) * (1f - TableManager.Instance.Gain);
					dy3 = angle_y3 - pre_angle_y3 - 360f;
				}
                else if (dy3 - (angle_y3 - pre_angle_y3) > 300f)
                {
					skyboxRot += (dy3 - (angle_y3 - pre_angle_y3) - 360f) * (1f - TableManager.Instance.Gain);
					dy3 = angle_y3 - pre_angle_y3 + 360f;
				}
                else
                {
					skyboxRot += (dy3 - (angle_y3 - pre_angle_y3)) * (1f - TableManager.Instance.Gain);
					dy3 = angle_y3 - pre_angle_y3;
				}
				rotationPoint225.transform.eulerAngles = new Vector3 (0f, dy3 * (1f - TableManager.Instance.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
			}

            // 315°の角
            else if ((transform.position.x > rotationPoint315.transform.position.x && transform.position.z < rotationPoint315.transform.position.z) || (transform.position.z > -transform.position.x - 0.15f && transform.position.z < -transform.position.x + 0.15f && transform.position.z < transform.position.x - 0.15f))
            {
				switch (action315) {
				case 0:
					if (transform.position.z > -transform.position.x)
						action315 = CornerAction.Left2Left;
					else
						action315 = CornerAction.Right2Right;
					action45Before = 0;
					action135Before = 0;
					action225Before = 0;
					action315Before = 0;
					rotationPoint45.transform.eulerAngles = new Vector3 (0f, b1, 0f);
					rotationPoint135.transform.eulerAngles = new Vector3 (0f, b2, 0f);
					rotationPoint225.transform.eulerAngles = new Vector3 (0f, b3, 0f);
					rotationPoint315.transform.eulerAngles = new Vector3 (0f, b4, 0f);
					wholeRoom.transform.parent = null;
					init_angle = rotationPoint315.transform.eulerAngles.y;
					pre_angle_y4 = transform.eulerAngles.y;
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
				wholeRoom.transform.parent = rotationPoint315.transform;
				var angle_y4 = transform.eulerAngles.y;
				if (dy4 - (angle_y4 - pre_angle_y4) < -300f)
                {
					skyboxRot += (dy4 - (angle_y4 - pre_angle_y4) + 360f) * (1f - TableManager.Instance.Gain);
					dy4 = angle_y4 - pre_angle_y4 - 360f;
				}
                else if (dy4 - (angle_y4 - pre_angle_y4) > 300f)
                {
					skyboxRot += (dy4 - (angle_y4 - pre_angle_y4) - 360f) * (1f - TableManager.Instance.Gain);
					dy4 = angle_y4 - pre_angle_y4 + 360f;
				}
                else
                {
					skyboxRot += (dy4 - (angle_y4 - pre_angle_y4)) * (1f - TableManager.Instance.Gain);
					dy4 = angle_y4 - pre_angle_y4;
				}
				rotationPoint315.transform.eulerAngles = new Vector3 (0f, dy4 * (1f - TableManager.Instance.Gain) + init_angle, 0f);
				RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
			}

            // 直線領域
            else
            {
				//直線領域に入った時の補正
				if (action45 != 0) {
					action45Before = action45;
					init_angle = rotationPoint45.transform.eulerAngles.y;
				}
				if (action135 != 0) {
					action135Before = action135;
					init_angle = rotationPoint135.transform.eulerAngles.y;
				}
				if (action225 != 0) {
					action225Before = action225;
					init_angle = rotationPoint225.transform.eulerAngles.y;
				}
				if (action315 != 0) {
					action315Before = action315;
					init_angle = rotationPoint315.transform.eulerAngles.y;
				}

				if (action45 == CornerAction.Right2Left || action45 == CornerAction.Left2Right) {
					b1 += 90f * (1f - TableManager.Instance.Gain) * (float)action45;
					if (b1 < 0f)
						b1 += 360f;
					if (b1 > 360f)
						b1 -= 360f;
				}
				if (action135 == CornerAction.Right2Left || action135 == CornerAction.Left2Right) {
					b2 += 90f * (1f - TableManager.Instance.Gain) * (float)action135;
					if (b2 < 0f)
						b2 += 360f;
					if (b2 > 360f)
						b2 -= 360f;
				}
				if (action225 == CornerAction.Right2Left || action225 == CornerAction.Left2Right) {
					b3 += 90f * (1f - TableManager.Instance.Gain) * (float)action225;
					if (b3 < 0f)
						b3 += 360f;
					if (b3 > 360f)
						b3 -= 360f;
				}
				if (action315 == CornerAction.Right2Left || action315 == CornerAction.Left2Right) {
					b4 += 90f * (1f - TableManager.Instance.Gain) * (float)action315;
					if (b4 < 0f)
						b4 += 360f;
					if (b4 > 360f)
						b4 -= 360f;
				}
		
				if (action45 > 0 || action135 < 0 || action225 > 0 || action315 < 0) {
					pre_pos_x = transform.position.z;
					correct_dis = TableManager.Instance.TableWidth / 2f + Mathf.Abs (pre_pos_x);
				}
				if (action45 < 0 || action135 > 0 || action225 < 0 || action315 > 0) {
					pre_pos_x = transform.position.x;
					correct_dis = TableManager.Instance.TableWidth / 2f + Mathf.Abs (pre_pos_x);
				}
				
                // tmp_nの値に応じて環境の角度を変更
				if (action45Before > 0)
                {
					if (Mathf.Abs (rotationPoint45.transform.eulerAngles.y - b1) > 1f)
                    {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b1 - init_angle < -300f) init_angle -= 360f;
						else if (b1 - init_angle > 300f) init_angle += 360f;
						skyboxRot -= (b1 - init_angle) / TableManager.Instance.TableWidth * dx;
						rotationPoint45.transform.eulerAngles += new Vector3 (0f, (b1 - init_angle) / correct_dis * dx, 0f);
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						pre_pos_x = pos_x;
						angle_y = rotationPoint45.transform.eulerAngles.y;
					}
                    else if (Mathf.Abs (rotationPoint45.transform.eulerAngles.y - b1) > 0.1f)
                    {
						rotationPoint45.transform.eulerAngles += new Vector3 (0f, (b1 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b1 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					}
                    else rotationPoint45.transform.eulerAngles = new Vector3 (0f, b1, 0f);
				}
				if (action45Before < 0)
                {
					if (Mathf.Abs (rotationPoint45.transform.eulerAngles.y - b1) > 1f)
                    {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b1 - init_angle < -300f)
							init_angle -= 360f;
						else if (b1 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b1 - init_angle) / TableManager.Instance.TableWidth * dx;
						rotationPoint45.transform.eulerAngles += new Vector3 (0f, (b1 - init_angle) / correct_dis * dx, 0f);
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						pre_pos_x = pos_x;
						angle_y = rotationPoint45.transform.eulerAngles.y;
					}
                    else if (Mathf.Abs (rotationPoint45.transform.eulerAngles.y - b1) > 0.1f)
                    {
						rotationPoint45.transform.eulerAngles += new Vector3 (0f, (b1 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b1 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					}
                    else rotationPoint45.transform.eulerAngles = new Vector3 (0f, b1, 0f);
				}
				if (action135Before > 0) {
					if (Mathf.Abs (rotationPoint135.transform.eulerAngles.y - b2) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b2 - init_angle < -300f)
							init_angle -= 360f;
						else if (b2 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b2 - init_angle) / TableManager.Instance.TableWidth * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						rotationPoint135.transform.eulerAngles += new Vector3 (0f, (b2 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationPoint135.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationPoint135.transform.eulerAngles.y - b2) > 0.1f) {
						rotationPoint135.transform.eulerAngles += new Vector3 (0f, (b2 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b2 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					} else
						rotationPoint135.transform.eulerAngles = new Vector3 (0f, b2, 0f);
				}
				if (action135Before < 0) {
					if (Mathf.Abs (rotationPoint135.transform.eulerAngles.y - b2) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b2 - init_angle < -300f)
							init_angle -= 360f;
						else if (b2 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b2 - init_angle) / TableManager.Instance.TableWidth * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						rotationPoint135.transform.eulerAngles += new Vector3 (0f, (b2 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationPoint135.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationPoint135.transform.eulerAngles.y - b2) > 0.1f) {
						rotationPoint135.transform.eulerAngles += new Vector3 (0f, (b2 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b2 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					} else
						rotationPoint135.transform.eulerAngles = new Vector3 (0f, b2, 0f);
				}
				if (action225Before > 0) {
					if (Mathf.Abs (rotationPoint225.transform.eulerAngles.y - b3) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b3 - init_angle < -300f)
							init_angle -= 360f;
						else if (b3 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b3 - init_angle) / TableManager.Instance.TableWidth * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						rotationPoint225.transform.eulerAngles += new Vector3 (0f, (b3 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationPoint225.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationPoint225.transform.eulerAngles.y - b3) > 0.1f) {
						rotationPoint225.transform.eulerAngles += new Vector3 (0f, (b3 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b3 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					} else
						rotationPoint225.transform.eulerAngles = new Vector3 (0f, b3, 0f);
				}
				if (action225Before < 0) {
					if (Mathf.Abs (rotationPoint225.transform.eulerAngles.y - b3) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b3 - init_angle < -300f)
							init_angle -= 360f;
						else if (b3 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b3 - init_angle) / TableManager.Instance.TableWidth * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						rotationPoint225.transform.eulerAngles += new Vector3 (0f, (b3 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationPoint225.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationPoint225.transform.eulerAngles.y - b3) > 0.1f) {
						rotationPoint225.transform.eulerAngles += new Vector3 (0f, (b3 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -=(b3 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					} else
						rotationPoint225.transform.eulerAngles = new Vector3 (0f, b3, 0f);
				}
				if (action315Before > 0) {
					if (Mathf.Abs (rotationPoint315.transform.eulerAngles.y - b4) > 1f) {
						pos_x = transform.position.x;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b4 - init_angle < -300f)
							init_angle -= 360f;
						else if (b4 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b4 - init_angle) / TableManager.Instance.TableWidth * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						rotationPoint315.transform.eulerAngles += new Vector3 (0f, (b4 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationPoint315.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationPoint315.transform.eulerAngles.y - b4) > 0.1f) {
						rotationPoint315.transform.eulerAngles += new Vector3 (0f, (b4 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b4 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					} else
						rotationPoint315.transform.eulerAngles = new Vector3 (0f, b4, 0f);
				}
				if (action315Before < 0) {
					if (Mathf.Abs (rotationPoint315.transform.eulerAngles.y - b4) > 1f) {
						pos_x = transform.position.z;
						dx = Mathf.Abs (pre_pos_x - pos_x);
						if (b4 - init_angle < -300f)
							init_angle -= 360f;
						else if (b4 - init_angle > 300f)
							init_angle += 360f;
						skyboxRot -= (b4 - init_angle) / TableManager.Instance.TableWidth * dx;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
						rotationPoint315.transform.eulerAngles += new Vector3 (0f, (b4 - init_angle) / correct_dis * dx, 0f);
						pre_pos_x = pos_x;
						angle_y = rotationPoint315.transform.eulerAngles.y;
					} else if (Mathf.Abs (rotationPoint315.transform.eulerAngles.y - b4) > 0.1f) {
						rotationPoint315.transform.eulerAngles += new Vector3 (0f, (b4 - angle_y) * Time.deltaTime * 5f, 0f);
						skyboxRot -= (b4 - angle_y) * Time.deltaTime;
						RenderSettings.skybox.SetFloat ("_Rotation", skyboxRot);
					} else
						rotationPoint315.transform.eulerAngles = new Vector3 (0f, b4, 0f);
				}

                // フラグを解消
				action45 = 0;
				action135 = 0;
				action225 = 0;
				action315 = 0;
			}
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
}
