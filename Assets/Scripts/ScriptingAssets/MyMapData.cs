using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MyMapJSONData
{
    //JSONファイルから読み取ったデータを保持するクラス

    public int[] values;
    public int mapWidth;
    public int mapHeight;
}

public class MyMapData
{
    //ResourceフォルダからJSONファイルを読み込んだデータを渡すクラス

    MyMapJSONData[] myMapDataArray;

    public MyMapData(int maxStageNum,string path)
    {
        Debug.Log($"MyMapDataのコンストラクタを実行します");
        myMapDataArray = new MyMapJSONData[maxStageNum];
        Debug.Log($"myMapdataArray.Length:{myMapDataArray.Length}");
        for (int i = 0; i < myMapDataArray.Length; i++)
        {
            string data = Resources.Load<TextAsset>(path).ToString(); //pathの例："JSON/posData" 
            myMapDataArray[i] = JsonUtility.FromJson<MyMapJSONData>(data);

            Debug.Log($"stageNum;{i}");
            Debug.Log(string.Join(",", myMapDataArray[i].values) );
        }

    }

    //Getter
    public int[] GetValues(int stageNum)
    {
        return myMapDataArray[stageNum].values;
    }
    public int GetWidth(int stageNum)
    {
        return myMapDataArray[stageNum].mapWidth;
    }
    public int GetHeight(int stageNum)
    {
        return myMapDataArray[stageNum].mapHeight;
    }
    public int GetLength(int stageNum)
    {
        return GetWidth(stageNum)*GetHeight(stageNum);
    }
    public int GetCellValue(int stageNum, int index)
    {
        return myMapDataArray[stageNum].values[index];
    }
    public int GetCellValue(int stageNum, int x, int y) //Excelに書き込まれた値がそのまま返ってくる
    {
        int stageWidht = myMapDataArray[stageNum].mapWidth;
        //return myMapDataArray[stageNum].values[y * stageWidht + x];
        return GetCellValue(stageNum, y * stageWidht + x);
    }


}
