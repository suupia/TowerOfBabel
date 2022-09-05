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
    public int GetMaxStageNum()
    {
        return myMapDataArray.Length;
    }
    public int[] GetValues(int stageIndex)
    {
        return myMapDataArray[stageIndex].values;
    }
    public int GetWidth(int stageIndex)
    {
        return myMapDataArray[stageIndex].mapWidth;
    }
    public int GetHeight(int stageIndex)
    {
        return myMapDataArray[stageIndex].mapHeight;
    }
    public int GetLength(int stageIndex)
    {
        return GetWidth(stageIndex)*GetHeight(stageIndex);
    }

    public int GetCellValue(int stageIndex, int index)
    {
        return myMapDataArray[stageIndex].values[index];
    }
    public int GetCellValue(int stageIndex, int x, int y) //Excelに書き込まれた値がそのまま返ってくる
    {
        int stageWidht = myMapDataArray[stageIndex].mapWidth;
        //return myMapDataArray[stageIndex].values[y * stageWidht + x];
        return GetCellValue(stageIndex, y * stageWidht + x);
    }


}
