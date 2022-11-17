using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 四角形のテーブルの方角
/// </summary>
public enum SquareDirection
{
    Edge0 = 0,
    Corner45 = 45,
    Edge90 = 90,
    Corner135 = 135,
    Edge180 = 180,
    Corner225 = 225,
    Edge270 = 270,
    Corner315 = 315,
    Other = 1000,
}
/// <summary>
/// 三角形のテーブルの方角
/// </summary>
public enum TriangleDirection
{
    Edge0 = 0,
    Edge120 = 120,
    Edge240 = 240,
    Corner = 1000,
}
/// <summary>
/// 五角形のテーブルの方角
/// </summary>
public enum PentagonDirection
{
    Edge0 = 0,
    Edge72 = 72,
    Edge144 = 144,
    Edge216 = 216,
    Edge288 = 288,
    Corner = 1000,
}
