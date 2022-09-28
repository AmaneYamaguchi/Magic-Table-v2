using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スクリーンショットを撮影するクラス
/// </summary>
public class ScreenShot : MonoBehaviour {
	void Update () {
		if (Input.GetKeyDown ("q")) {
			ScreenCapture.CaptureScreenshot ("MagicTable.png", 5);
		}
	}
}
