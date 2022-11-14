using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常にカメラの方向を向くオブジェクト
/// </summary>
public class BillBoard : MonoBehaviour
{
    /// <summary>
    /// ターゲットにするオブジェクト（カメラ想定）
    /// </summary>
    public Transform Target;
    /// <summary>
    /// y軸を固定するかどうか
    /// </summary>
    public bool YConstraint = true;

    void Update()
    {
        var targetPos = YConstraint ? new Vector3(Target.position.x, transform.position.y, Target.position.z) : Target.position;
        transform.LookAt(targetPos, Vector3.up);
    }
}
