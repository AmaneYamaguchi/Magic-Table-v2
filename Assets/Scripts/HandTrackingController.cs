using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ハントラを制御する
/// </summary>
public class HandTrackingController : MonoBehaviour
{
    GameObject[] points = new GameObject[70];
    OVRBody ovrBody;

    // スタート時に呼ばれる
    void Start()
    {
        // ポイント群の生成
        for (int i = 0; i < 70; i++)
        {
            points[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (i < 18)
            {
                points[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                points[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
        }

        // 参照
        ovrBody = GetComponent<OVRBody>();
    }

    // フレーム更新毎に呼ばれる
    void Update()
    {
        // ボディトラッキングの有効時
        if (ovrBody != null && ovrBody.BodyState != null)
        {
            OVRPlugin.BodyState bodyState = (OVRPlugin.BodyState)ovrBody.BodyState;

            // ポイント群の更新
            for (int i = 0; i < 70; i++)
            {
                OVRPlugin.Posef pose = bodyState.JointLocations[i].Pose;
                Vector3 pos = this.points[i].transform.position;
                pos.x = pose.Position.x;
                pos.y = pose.Position.y;
                pos.z = pose.Position.z + 1;
                this.points[i].transform.position = pos;
            }
        }
    }
}
