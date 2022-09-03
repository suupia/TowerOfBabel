using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapData : MyMapData
{
    MyMapJSONData[] myMapDataArray;  //ステージのタワーの位置やステージの説明文のデータ

    //const int numToIdentifyID = 10; //IDを識別するための数。Excel上で１の位の数でIDの識別をしているため、10を代入している。
    //const int numForTower= 0; //towerIDの１の位の数字は0
    //const int numForItem = 1; //itemIDの１の位の数字は1
    //const int stageDescriptionItemID = 0; //アイテムの中でのステージの説明文のID  ItemMGRで用いる
    //const int hiddenCharacterItemID = 1;
    //const int idNotExistNum = -888; //探していたもののIDが存在しないときの数字。負の数であることが重要。

    //const int numToIdentifyID = 10; //IDを識別するための数。Excel上で１の位の数でIDの識別をしているため、10を代入している。
    const int numToIdentifyRank = 1; //IDを識別するために区切る桁の指数部分。Excel上で１の位の数でIDの識別をしているため、10^1すなわち1を代入している。
    const int numForTower = 0; //towerの１の位の数字は0
    const int numForStoryItem = 1; //storyItemの１の位の数字は1
    const int numForHiddenCharacter = 2; //hiddenCharacterItemの１の位の数字は2
    const int numForRecordItem = 3; //recordItemの１の位の数字は3

    const int idNotExistNum = -888; //探していたもののIDが存在しないときの数字。負の数であることが重要。


    public   MapData(int maxStageNum,string path) : base(maxStageNum,path)
    {

    }


    public int GetIDFromResources(int stageNum, int x, int y)
    {
        int stageWidht = myMapDataArray[stageNum].mapWidth;
        int cellValue = myMapDataArray[stageNum].values[y * stageWidht + x];

        if (cellValue < 0)
        {
            Debug.Log("cellValueが0以下です　アイテムのIDには正の整数を指定してください");
            return idNotExistNum;
        }

        int identifyID = cellValue % (int)Mathf.Pow(10, numToIdentifyRank);

        int id = (cellValue - identifyID) / (int)Mathf.Pow(10, numToIdentifyRank); //1の位の数字を取り除いた数

        return id;
    }

    public int GetIdentifyIDFromResources(int stageNum, int x, int y) //numToIdentifyRankで区切られたIdentifyIDを取得する(今は1の位の値を返す)
    {
        int stageWidht = myMapDataArray[stageNum].mapWidth;
        int cellValue = myMapDataArray[stageNum].values[y * stageWidht + x];

        if (cellValue < 0)
        {
            Debug.Log("cellValueが0以下です　アイテムのIDには正の整数を指定してください");
            return idNotExistNum;
        }

        int identifyID = cellValue % (int)Mathf.Pow(10, numToIdentifyRank);

        return identifyID;
    }

}
