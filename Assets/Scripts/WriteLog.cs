using UnityEngine;
using System.Collections;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception
using System.Text; //Encoding
using System.Collections.Generic;

public class WriteLog : MonoBehaviour {
	private string guitxt = "";
	private string outputFileName;
    //private float cameralotate;
    //public int playbackSpeed=1;
	public GameObject Head;
	public GameObject righthand;
	public GameObject lefthand;
	public GameObject Square;
	public GameObject Triangle;
	public GameObject Pentagon;

    //List<float> stageMapData = new List<float>();
    public static bool fback = false;
    public static bool rec = true;
    //public GameObject realHeadPosition;
    //public GameObject virtualHeadPosition;
    //int i;
    //int height;
    float timer = 0f;

    void Awake()
    {
#if Unity_EDITOR
        if (Directory.Exists(Application.dataPath + "/Data"))
        {
            Debug.Log("Data Folder Exists");
        }
        else
        {
            Directory.CreateDirectory(Application.dataPath + "/Data");
            Debug.Log("Data Folder Create");
        }
#else
        if (Directory.Exists(Application.dataPath + "/Data"))
        {
            Debug.Log("Data Folder Exists");
        }
        else
        {
            Directory.CreateDirectory(Application.dataPath + "/Data");
            Debug.Log("Data Folder Create");
        }
#endif
    }
    

	// Use this for initialization
	void Start () {
		guitxt = "/Data/"+DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
		outputFileName = guitxt;
		ReadFile();
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
		guitxt = SetDefaultText();
        if (rec && timer > 0.1f)
        {
            timer = 0f;
            WriteFile(guitxt);
            //RecData();
        }
        //Playback(stageMapData);
		// ();
	}
	
	void WriteFile(string txt){
		FileInfo fi = new FileInfo(Application.dataPath + "/" + outputFileName);
		using (StreamWriter sw = fi.AppendText()){
			sw.WriteLine(guitxt);
		}
	}
	
	void ReadFile(){
		FileInfo fi = new FileInfo(Application.dataPath + "/" + outputFileName);
		try {
			using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)){
				guitxt = sr.ReadToEnd();
			}
        }
#pragma warning disable 0168
        catch (Exception e){
#pragma warning disable 0168
			WriteFile(guitxt);
			guitxt = "Time, Head.Position.x, Head.Position.y, Head.Position.z, rightHand.Position.x, rightHand.Position.y, rightHand.Position.z, leftHand.Position.x, leftHand.Position.y, leftHand.Position.z, Head.Rotation.x, Head.Rotation.y, Head.Rotation.z, rightHand.Rotation.x, rightHand.Rotation.y, rightHand.Rotation.z, leftHand.Rotation.x, leftHand.Rotation.y, leftHand.Rotation.z, Square, Triangle, Pentagon";
			WriteFile(guitxt);
		}
	}
	
	string SetDefaultText(){
		return Time.time + ", " + Head.transform.position.x + ", " + Head.transform.position.y + ", " + Head.transform.position.z + ", " + righthand.transform.position.x + ", " + righthand.transform.position.y + ", " + righthand.transform.position.z  + ", " + lefthand.transform.position.x + ", " + lefthand.transform.position.y + ", " + lefthand.transform.position.z+ ", " + Head.transform.eulerAngles.x + ", " + (Head.transform.eulerAngles.y) + ", " + Head.transform.eulerAngles.z + ", " + righthand.transform.eulerAngles.x + ", " + righthand.transform.eulerAngles.y + ", " + righthand.transform.eulerAngles.z + ", " + lefthand.transform.eulerAngles.x + ", " + lefthand.transform.eulerAngles.y + ", " + lefthand.transform.eulerAngles.z + ", " + Square.activeSelf +  ", " + Triangle.activeSelf + ", " + Pentagon.activeSelf;
	}

    /*void RecData()
    {
        stageMapData.Add(Head.transform.position.x);
        stageMapData.Add(Head.transform.position.y);
        stageMapData.Add(Head.transform.position.z);
        stageMapData.Add(Hand.transform.position.x);
        stageMapData.Add(Hand.transform.position.y);
        stageMapData.Add(Hand.transform.position.z);

    }*/

    /*void Playback(List<float> arrays)
    {
        if (fback)
            if (i < arrays.Count / (6*playbackSpeed))
            {
                realHeadPosition.transform.position = new Vector3(arrays[i * 6 * playbackSpeed], arrays[i * 6 * playbackSpeed + 1]+3f, arrays[i * 6*playbackSpeed + 2]);
                virtualHeadPosition.transform.position = new Vector3(arrays[i * 6 *playbackSpeed+ 3], arrays[i * 6 *playbackSpeed+ 4]+3f, arrays[i * 6*playbackSpeed + 5]);
                i++;
        }
    }*/

    /*void GetKey () {
        if (Input.GetKeyDown(KeyCode.I))
            cameralotate = Ref.transform.eulerAngles.y;
        if (Input.GetKeyDown(KeyCode.P))
            fback = true;        
    }*/
}