using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TileTerrainFileFunction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A)) {
            StringToByte();
        }
	}
    public void StringToByte() {
        //byte[] byteArray = System.Text.Encoding.Default.GetBytes("0000000000");
        //500*500*10/8=312500
        byte[] byteArray =new byte[500000];
        //先转成byte[];
        string content = System.Convert.ToBase64String(byteArray);
        StreamWriter sw = new StreamWriter("e:\\20141021.xml");//这里写上你要保存的路径
        sw.WriteLine(content);//按行写
        sw.Close();//关闭
    }
    /// <summary>
    /// 编码
    /// </summary>
    public void Encoded() {
        System.IO.FileStream fs = System.IO.File.OpenRead("c://1.jpg");

        byte[] dt = new byte[fs.Length];
        fs.Read(dt, 0, (int)fs.Length);
        fs.Close();
        string s = System.Convert.ToBase64String(dt);
        fs = System.IO.File.OpenWrite("d://1.b64");
        dt = Encoding.Default.GetBytes(s);
        fs.Write(dt, 0, dt.Length);
        fs.Flush();
        fs.Close();
    }
    /// <summary>
    /// 解码
    /// </summary>
   public void Decode() {
        System.IO.FileStream fs = System.IO.File.OpenRead("c://1.b64");
        byte[] dt = new byte[fs.Length];
        fs.Read(dt, 0, (int)fs.Length);
        string s = Encoding.Default.GetString(dt);
        dt = System.Convert.FromBase64String(s);
        fs = System.IO.File.OpenWrite("c://2.jpg");
        fs.Write(dt, 0, dt.Length);
        fs.Close();
    }
         
}
