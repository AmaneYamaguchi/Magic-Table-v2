using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テーブルの変形を管理するクラス
/// </summary>
public class TableManager : Singleton<TableManager> {
    /// <summary>
    /// テーブルの1辺の大きさ
    /// </summary>
    public float TableWidth
    {
        get
        {
            return 0.8f;
        }
    }
	/// <summary>
	/// 煙のパーティクル
	/// </summary>
	public List<ParticleSystem> Smokes;
    /// <summary>
    /// テーブルの向き
    /// </summary>
	public GameObject TableDirection;
    /// <summary>
    /// 四角形型のテーブル
    /// </summary>
	public GameObject SquareTable;
    /// <summary>
    /// 三角形型のテーブル
    /// </summary>
	public GameObject TriangleTable;
    /// <summary>
    /// 五角形型のテーブル
    /// </summary>
	public GameObject PentagonalTable;
    /// <summary>
    /// 魔法にかかったような効果音
    /// </summary>
	public AudioClip magicalSound;
    /// <summary>
    /// 音源
    /// </summary>
	private AudioSource audioSource;
    /// <summary>
    /// 現在テーブルの形状が変化している
    /// </summary>
    public bool IsChanging
    {
        private set; get;
    } = false;
    /*
    [SerializeField]
    private HandTranslate rightHand;
    [SerializeField]
    private HandTranslate leftHand;
    */
    /// <summary>
    /// テーブルの形状
    /// </summary>
    public enum TableShape
    {
        Triangle = 3,
        Square = 4,
        Pentagon = 5,
    }
    private TableShape _currentShape = TableShape.Square;
    /// <summary>
    /// 現在のテーブルの形状
    /// </summary>
    public TableShape CurrentShape
    {
        set
        {
            // 現在進行形でテーブルの形状が変化しているなら何もしない
            if (IsChanging) return;

            // 同じなら何もしない
            if (value == _currentShape) return;

            // 現在HMDが角にいる場合は何もしない（できない）
            if (WorldTranslate.Instance.HMDIsInCorner) return;

            _currentShape = value;

            // 実際にテーブルの形状を変更する
            StartCoroutine(ChangeTableShape(value));

            // HandTranslateの状態を更新する
            //rightHand.ResetRealHandDirection();
            //rightHand.ResetVirtualHandDirection(value);
            //leftHand.ResetRealHandDirection();
            //leftHand.ResetVirtualHandDirection(value);

            // 世界をルートの子にする
            WorldTranslate.Instance.ResetCurrentCorner();
        }
        get
        {
            return _currentShape;
        }
    }
    /// <summary>
    /// 角における回転角度のゲイン
    /// </summary>
    public float Gain
    {
        get
        {
            return 4f / (float)CurrentShape;
        }
    }

	protected override void Awake ()
    {
        base.Awake();
        StopSmoke();
		audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
	}

	/// <summary>
	/// 煙を出す
	/// </summary>
	private void PlaySmoke(){
		foreach (var smoke in Smokes){
			smoke.Play();
		}
	}
    /// <summary>
    /// 煙を止める
    /// </summary>
    private void StopSmoke()
    {
        foreach (var smoke in Smokes)
        {
            smoke.Stop();
        }
    }
    /// <summary>
    /// 指定した形状のテーブルのみを有効化する
    /// </summary>
    /// <param name="nextShape"></param>
    private void ActivateTable(TableShape nextShape)
    {
        TriangleTable.SetActive(nextShape == TableShape.Triangle);
        SquareTable.SetActive(nextShape == TableShape.Square);
        PentagonalTable.SetActive(nextShape == TableShape.Pentagon);
    }
    /// <summary>
    /// VR空間上のテーブルの形状を実際に変更する具体的な一連処理
    /// </summary>
    /// <param name="nextShape"></param>
    /// <returns></returns>
    private IEnumerator ChangeTableShape(TableShape nextShape)
    {
        // 変化している
        IsChanging = true;

        // 煙を出す
        PlaySmoke();

        // SEを出す
        audioSource.PlayOneShot(magicalSound);

        yield return new WaitForSeconds(0.7f);

        // nextShapeのテーブルを有効化する
        ActivateTable(nextShape);

        // テーブルの辺が丁度いい位置に来るようにテーブルの角度を変更する
        InitializeTableDirection();

        yield return new WaitForSeconds(0.8f);

        // 変化終了
        IsChanging = false;

        yield return 0f;
    }
	/// <summary>
    /// テーブルの方向を初期化する
    /// </summary>
	private void InitializeTableDirection()
    {
        // 角にいる時はテーブルの方向を変えられない
		TableDirection.transform.eulerAngles = new Vector3 (0f, (float)WorldTranslate.Instance.HMDDirection, 0f);
		TableDirection.transform.position = Vector3.zero;
	}

    // デバッグ用UI
    private int windowId = 0;
    private Rect windowRect = new Rect(0, 0, 200, 200);
    public void OnGUI()
    {
        windowRect = GUI.Window(windowId, windowRect, (Id) =>
        {
            if (GUILayout.Button("3"))
            {
                CurrentShape = TableShape.Triangle;
            }
            if (GUILayout.Button("4"))
            {
                CurrentShape = TableShape.Square;
            }
            if (GUILayout.Button("5"))
            {
                CurrentShape = TableShape.Pentagon;
            }
            GUILayout.Label($"dir = {WorldTranslate.Instance.HMDDirection}");
        }, "Table Manager (Debug)");
    }
}