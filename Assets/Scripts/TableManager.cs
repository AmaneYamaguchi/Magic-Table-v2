using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : Singleton<TableManager> {
    /// <summary>
    /// テーブルの1辺の大きさ
    /// </summary>
    public float TableWidth
    {
        get
        {
            return 0.7f;
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
    /// 頭の位置によってテーブルの向きを変えるのに使用するインデックス？
    /// </summary>
	public int dir = 0;
    /// <summary>
    /// 現在テーブルの形状が変化している
    /// </summary>
    public bool IsChanging
    {
        private set; get;
    } = false;

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

            _currentShape = value;

            // 実際にテーブルの形状を変更する
            StartCoroutine(ChangeTableShape(value));
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
	void Update () {
		//手を机の中心に合わせると机が変わる
        /*
		if ((Out == 1 && CurrentShape == TableShape.Triangle && distance_2d (magic1.transform.position, transform.position) < 0.2f && transform.position.y < 1f && transform.position.y > 0.5f && Rotation.direction != 4) || Input.GetKeyUp ("a")) {
            //四角形から三角形に変える
            //GlobalVariables.Gain = 4f / 3f;
            CurrentShape = TableShape.Triangle;
			PlaySmoke ();
			audiosource.PlayOneShot (se);
			dir = Rotation.direction;
			Invoke ("set3", 1.5f);
			Invoke ("table_direction", 0.8f);
			Invoke ("del4", 0.7f);
			//Invoke ("scene0to1", 0.8f);
			Out = 0;
			duration = 0f;
		}

		if ((Out == 1 && CurrentShape == TableShape.Square && distance_2d (magic2.transform.position, transform.position) < 0.17f && transform.position.y < 1f && transform.position.y > 0.5f && Rotation.direction != 4) || Input.GetKey ("b")) {
            //三角形から五角形に変える
            //GlobalVariables.Gain = 0.8f;
            CurrentShape = TableShape.Pentagon;
			PlaySmoke ();
			audiosource.PlayOneShot (se);
			dir = Rotation.direction;
			Invoke ("set5", 1.5f);
			Invoke ("table_direction", 0.8f);
			Invoke ("del3", 0.7f);
			//Invoke ("scene1to2", 0.8f);
			Out = 0;
			duration = 0f;
		}

		if ((Out == 1 && CurrentShape == TableShape.Pentagon && distance_2d (magic3.transform.position, transform.position) < 0.2f && transform.position.y < 1f && transform.position.y > 0.5f && Rotation.direction != 4) || Input.GetKey ("c")) {
            //五角形から四角形に変える
            //GlobalVariables.Gain = 1f;
            CurrentShape = TableShape.Square;
            PlaySmoke ();
			audiosource.PlayOneShot (se);
			dir = Rotation.direction;
			Invoke ("set4", 1.5f);
			Invoke ("table_direction", 0.8f);
			Invoke ("del5", 0.7f);
			//Invoke ("scene2to0", 0.8f);
			Out = 0;
			duration = 0f;
		}
        */

		//duration += Time.deltaTime;
		//if (duration > 15f) {
			//Out = 1;
		//}

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

        // 何？
        dir = RotationManager.Instance.direction;
        InitializeTableDirection();

        yield return new WaitForSeconds(0.8f);

        // 変化終了
        IsChanging = false;

        yield return 0f;
    }
		
	void InitializeTableDirection(){
		TableDirection.transform.eulerAngles = new Vector3 (0f, dir * 90f, 0f);
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
        }, "Table Manager (Debug)");
    }
}