﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapMGR : MonoBehaviour
{
    [System.Serializable]
    class StageTileArray //インスペクター上でタイルをセットできるようにするためにクラスを作る
    {
        [SerializeField] public TileBase[] stageTileArray;
    }
    [SerializeField] Tilemap tilemap;
    [SerializeField] StageTileArray[] tileArray;

    EntityMapData<MonoBehaviour> map;

    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] int outOfStageQ; //マップの外にタイルを何枚はみ出して張るかを決める




    public void SetupMap() //これをGameManagerから呼ぶ
    {
        //初期化
        //ここで想定することは
        //mapにRMIと同様のインスタンスがはいって欲しい
        //つまり、数字だけが入っているマス目とシーンに置くゲームオブジェクトにアタッチするMGRスクリプトが入っているマス目がメンバとして用意されたインスタンス
        map = null;
        map = new EntityMapData<MonoBehaviour>(GameManager.instance.wallID, mapWidth, mapHeight, 1); //とりあえずTowerMGRだけなので最後の引数は1にしておく


        PlaceSpawnPoint();

        PlaceSpawnPoint();

        PlaceTower();

        RenderMap();

    }

    private void ReSetupMap() //MakeTheFirstRoadで失敗したときに呼ばれる
    {
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                if (map.GetValue(x, y) % GameManager.instance.groundID == 0) //MakeTheFirstRoadで作った道を壁に戻す
                {
                    map.SetValue(x, y, GameManager.instance.wallID); //直接値を代入していることに注意
                }
            }
        }




        //Debug.Log($"numOfFristRoadCounterを{GetNumOfFristRoad()}に初期化しました");

        RenderMap();

        //SetupMap()に比べて、PlaceCastle()とPlaceTower()がない
    }
    private void RenderMap()
    {
        //マップをクリアする（重複しないようにする）
        tilemap.ClearAllTiles();

        for (int y = -outOfStageQ; y < map.Height + outOfStageQ; y++)
        {
            for (int x = -outOfStageQ; x < map.Width + outOfStageQ; x++)
            {
                SetTileAccordingToValues(x, y);
            }
        }

    }

    private void PlaceSpawnPoint()
    {

    }
    private void PlaceTower()
    {

    }
    private void PlaceBrickYard()
    {

    }

    private void SetTileAccordingToValues(int x, int y)
    {

        //if (0 <= y && y < map.Height && 0 <= x && x < map.Width)
        //{
        //    // 1 = タイルあり、0 = タイルなし
        //    if (map.GetValue(x, y) % GameManager.instance.wallID == 0)
        //    {
        //        if (CalculateTileType(x, y) < 47) //周りがすべて壁のタイルは3種類ある
        //        {
        //            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[CalculateTileType(x, y)]);
        //        }
        //        else
        //        {
        //            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]);
        //        }
        //        //Debug.Log($"タイルを{x},{y}に敷き詰めました");
        //    }
        //    else if (map.GetValue(x, y) % GameManager.instance.groundID == 0)
        //    {
        //        tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(50, 52 + 1)]);
        //    }

        //}
        //else //mapの領域外
        //{
        //    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]); //全方向が壁のタイルを張る(3枚)
        //}
    }
    private int CalculateTileType(int x, int y)
    {
        bool upIsWall = false;
        bool leftIsWall = false;
        bool downIsWall = false;
        bool rightIsWall = false;
        int upleftWallValue = 0; //1のときwallがあることを表す
        int downleftWallValue = 0;
        int downrightWallValue = 0;
        int uprightWallValue = 0;
        int binarySub;

        if (IsOutRangeOfMap(x, y))
        {
            Debug.LogError($"CalculateTileType({x},{y})の引数でmapの範囲外が指定されました");
            return -100;
        }

        //そもそもgroundIDの時は0を返すようにする（これはRenderMapでは使わない）
        if (map.GetValue(x, y) % GameManager.instance.groundID == 0)
        {
            return 0;
        }

        if (y == map.Height - 1 || map.GetValue(x, y + 1) % GameManager.instance.wallID == 0) //左側の条件式の方が先に判定されるので、mapの範囲外にGetValueすることはない（と思う）
        {
            upIsWall = true;
        }

        if (x == 0 || map.GetValue(x - 1, y) % GameManager.instance.wallID == 0)
        {
            leftIsWall = true;
        }

        if (y == 0 || map.GetValue(x, y - 1) % GameManager.instance.wallID == 0)
        {
            downIsWall = true;
        }

        if (x == map.Width - 1 || map.GetValue(x + 1, y) % GameManager.instance.wallID == 0)
        {
            rightIsWall = true;
        }


        if (x == 0 || y == map.Height - 1 || map.GetValue(x - 1, y + 1) % GameManager.instance.wallID == 0) //この4つの場合分けは4隅を調べればよいので、xだけの判定で十分
        {
            upleftWallValue = 1;
        }

        if (x == 0 || y == 0 || map.GetValue(x - 1, y - 1) % GameManager.instance.wallID == 0)
        {
            downleftWallValue = 1;
        }

        if (x == map.Width - 1 || y == 0 || map.GetValue(x + 1, y - 1) % GameManager.instance.wallID == 0)
        {
            downrightWallValue = 1;
        }

        if (x == map.Width - 1 || y == map.Height - 1 || map.GetValue(x + 1, y + 1) % GameManager.instance.wallID == 0)
        {
            uprightWallValue = 1;
        }



        //壁と接しないとき
        if (!upIsWall && !leftIsWall && !downIsWall && !rightIsWall)
        {
            return 1;
        }

        //1つの壁と接するとき
        if (upIsWall && !leftIsWall && !downIsWall && !rightIsWall)
        {
            return 2;
        }
        else if (!upIsWall && leftIsWall && !downIsWall && !rightIsWall)
        {
            return 3;
        }
        else if (!upIsWall && !leftIsWall && downIsWall && !rightIsWall)
        {
            return 4;
        }
        else if (!upIsWall && !leftIsWall && !downIsWall && rightIsWall)
        {
            return 5;
        }

        //2つの壁と接するとき
        if (upIsWall && !leftIsWall && downIsWall && !rightIsWall)
        {
            return 6;
        }
        else if (!upIsWall && leftIsWall && !downIsWall && rightIsWall)
        {
            return 7;
        }
        else if (upIsWall && leftIsWall && !downIsWall && !rightIsWall)
        {
            return 8 + upleftWallValue;
        }
        else if (!upIsWall && leftIsWall && downIsWall && !rightIsWall)
        {
            return 10 + downleftWallValue;
        }
        else if (!upIsWall && !leftIsWall && downIsWall && rightIsWall)
        {
            return 12 + downrightWallValue;
        }
        else if (upIsWall && !leftIsWall && !downIsWall && rightIsWall)
        {
            return 14 + uprightWallValue;
        }

        //3つの壁と接するとき
        if (upIsWall && leftIsWall && downIsWall && !rightIsWall)
        {
            binarySub = 0;
            binarySub = upleftWallValue + 2 * downleftWallValue;
            return 16 + binarySub;

        }
        else if (!upIsWall && leftIsWall && downIsWall && rightIsWall)
        {
            binarySub = 0;
            binarySub = downleftWallValue + 2 * downrightWallValue;
            return 20 + binarySub;

        }
        else if (upIsWall && !leftIsWall && downIsWall && rightIsWall)
        {
            binarySub = 0;
            binarySub = downrightWallValue + 2 * uprightWallValue;
            return 24 + binarySub;
        }
        else if (upIsWall && leftIsWall && !downIsWall && rightIsWall)
        {
            binarySub = 0;
            binarySub = uprightWallValue + 2 * upleftWallValue;
            return 28 + binarySub;
        }

        //4つの壁と接するとき
        if (upIsWall && leftIsWall && downIsWall && rightIsWall)
        {
            //周囲に地面が一つでもあるとき
            if (upleftWallValue == 0 || downleftWallValue == 0 || downrightWallValue == 0 || uprightWallValue == 0)
            {
                binarySub = 0;
                binarySub = upleftWallValue + 2 * downleftWallValue + 4 * downrightWallValue + 8 * uprightWallValue;
                return 32 + binarySub;
            }
            else   //周囲がすべて壁の時はタイルの種類をあとで乱数で決める
            {
                return 47;
            }

        }

        Debug.LogError($"CalculateTileNum({y} ,{x})に失敗しました");
        return -100;
    }
    private bool IsOutRangeOfMap(int x, int y)
    {
        if (x < 0 || y < 0 || x > map.Width - 1 || y > map.Height - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public EntityMapData<MonoBehaviour> GetMap()
    {
        return map;
    }
    public int GetMapSize()
    {
        return mapHeight * mapWidth;
    }
    public int GetMapHeight()
    {
        return mapHeight;
    }
    public int GetMapWidth()
    {
        return mapWidth;
    }
    public long GetMapValue(int x, int y)
    {
        return map.GetValue(x, y);
    }
    public long GetMapValue(Vector2Int vector)
    {
        return map.GetValue(vector);
    }
    public long GetMapValue(int index)
    {
        return map.GetValue(index);
    }


    //Setter
    public void MultiplySetMapValue(Vector2Int vector, int value)
    {
        map.MultiplySetValue(vector, value);
    }

    public void DivisionalSetMapValue(Vector2Int vector, int value)
    {
        if (map.GetValue(vector) % value != 0)
        {
            Debug.LogError($"DivisionalSetMapValue({vector},{value})でmap({vector})の値を{value}で割り切ることができませんでした。");
        }
        else
        {
            map.DivisionalSetValue(vector, value);
        }
    }

}

//Getter

