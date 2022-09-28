using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触れたときにテーブルの形状を変えるようなボタン
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ShapeControlButton : MonoBehaviour
{
    /// <summary>
    /// 形状の変化先
    /// </summary>
    public TableManager.TableShape shape;

    /// <summary>
    /// 触れたときにテーブルの形状を変更
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        TableManager.Instance.CurrentShape = shape;
    }
}
