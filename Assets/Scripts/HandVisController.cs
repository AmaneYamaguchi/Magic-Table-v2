using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ハントラ時とコントローラ時で手の見た目を変更する
/// </summary>
public class HandVisController : MonoBehaviour
{
    /// <summary>
    /// ハントラ時のメッシュ
    /// </summary>
    [SerializeField]
    private SkinnedMeshRenderer OVRHandPrefab;
    /// <summary>
    /// コントローラ時のメッシュ
    /// </summary>
    [SerializeField]
    private SkinnedMeshRenderer Controller;
    [SerializeField]
    private Collider OVRHandCollider;
    [SerializeField]
    private Collider ControllerCollider;

    private void FixedUpdate()
    {
        Controller.enabled = !OVRHandPrefab.enabled;
        OVRHandCollider.enabled = OVRHandPrefab.enabled;
        ControllerCollider.enabled = !OVRHandPrefab.enabled;
    }
}
