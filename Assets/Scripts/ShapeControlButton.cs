using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�ꂽ�Ƃ��Ƀe�[�u���̌`���ς���悤�ȃ{�^��
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ShapeControlButton : MonoBehaviour
{
    /// <summary>
    /// �`��̕ω���
    /// </summary>
    public TableManager.TableShape shape;

    /// <summary>
    /// �G�ꂽ�Ƃ��Ƀe�[�u���̌`���ύX
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        TableManager.Instance.CurrentShape = shape;
    }
}
