using System.Collections;
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


    int outOfStageQ = 10; //マップの外にタイルを何枚はみ出して張るかを決める

    int entityNum = 2; //マップに配置されうるものの種類

    int maxStageNum = 2; //とりあえず2にしておく

    string mapDataPath = "JSON/mapData";

    Map map;


    public void Init(int stageIndex) //これをGameManagerから呼ぶ
    {
        Debug.Log($"MapMGRのInitを実行します");

        InitMap(stageIndex);


        //PlaceSpawnPoint();

        //PlaceSpawnPoint();

        //PlaceTower();

        RenderMap();

    }

    private void InitMap(int stageIndex)
    {
        MapData mapData = new MapData(maxStageNum,mapDataPath);
        map = new Map(GameManager.instance.wallID, mapData.GetWidth(stageIndex), mapData.GetHeight(stageIndex), entityNum,stageIndex); //とりあえずTowerMGRだけなので最後の引数は1にしておく

        for (int i = 0; i < mapData.GetLength(stageIndex); i++)
        {
           // Debug.Log($"GetCellValue(stageIndex,{i}):{mapData.GetCellValue(stageIndex, i)}");

            map.SetValue(i, mapData.GetCellValue(stageIndex,i));

        }

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
        System.Func<long, bool> IsWall;

        IsWall = (mapValue) => { return mapValue < 0 ? true : false; };

        //mapの領域外
        if (0 > y || y > map.Height-1 || 0 > x || x > map.Width-1)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]); //全方向が壁のタイルを張る(3枚)
            Debug.Log($"壁のタイルをタイルを{x},{y}に敷き詰めました（領域外）");

            return;
        }

        //負の数なら壁、正の数なら地面
        if (map.GetValue(x, y) < 0)
        {
            if (MapFunction.CalculateTileType(x, y,map,IsWall) < 47) //周りがすべて壁のタイルは3種類ある
            {
                //少なくとも一つの方向は地面のタイルである壁のタイル
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[MapFunction.CalculateTileType(x, y, map, IsWall)]);
                Debug.Log($"GetValue:{map.GetValue(x, y)}、地面に接する壁、({x},{y})、タイル番号：{MapFunction.CalculateTileType(x, y, map, IsWall)}");
            }
            else
            {
                //周りがすべて壁のタイル
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                Debug.Log($"壁に囲まれた壁、({x},{y})、タイル番号：47から49の乱数");

            }
        }
        else if (map.GetValue(x, y) > 0)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[UnityEngine.Random.Range(50, 52 + 1)]);
            Debug.Log($"地面、({x},{y})、タイル番号：50から52の乱数");


        }
        else
        {
            Debug.LogError($"mapに予期せぬ値が入っています {map.GetValue(x, y)}");
        }


    }


}


public class Map : EntityMap<global::Entity>
{
    //MapMGRに沿うように改良されたEntityMap

    int _stageIndex;

    //コンストラクタ
    public Map(int initValue, int width, int height, int entityNum, int stageIndex) : base(initValue, width, height, entityNum)
    {
        _stageIndex = stageIndex;
    }

    //Getter
    public int GetStageIndex()
    {
        return _stageIndex;
    }

    //Setter

    public void AddBrickYard(Vector2Int vector, int entityIndex, BrickYard entity)
    {
        AddEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void RemoveBrickYard(Vector2Int vector, int entityIndex, BrickYard entity)
    {
        RemoveEntity(vector.x, vector.y, entityIndex, entity);
    }

    public void AddP1SpawnPoint(Vector2Int vector, int entityIndex, SpawnPoint entity)
    {
        AddEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void AddP2SpawnPoint(Vector2Int vector, int entityIndex, SpawnPoint entity)
    {
        AddEntity(vector.x, vector.y, entityIndex, entity);
    }

    public void RemoveP1SpawnPoint(Vector2Int vector, int entityIndex, SpawnPoint entity)
    {
        RemoveEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void RemoveP2SpawnPoint(Vector2Int vector, int entityIndex, SpawnPoint entity)
    {
        RemoveEntity(vector.x, vector.y, entityIndex, entity);
    }

    public void AddP1Tower(Vector2Int vector, int entityIndex, Tower entity)
    {
        AddEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void AddP2Tower(Vector2Int vector, int entityIndex, Tower entity)
    {
        AddEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void RemoveP1Tower(Vector2Int vector, int entityIndex, Tower entity)
    {
        RemoveEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void RemoveP2Tower(Vector2Int vector, int entityIndex, Tower entity)
    {
        RemoveEntity(vector.x, vector.y, entityIndex, entity);
    }
}

public static class MapFunction
{
    //MapをTilemapにレンダリングするときに使用するクラス

    public static int CalculateTileType(int x, int y, Map map, System.Func<long,bool>  IsWall)
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


        //そもそもgroundIDの時は0を返すようにする（これはRenderMapでは使わない）
        if (!IsWall(map.GetValue(x, y)))
        {
            return 0;
        }

        if (y == map.Height - 1 || IsWall(map.GetValue(x, y + 1))) //左側の条件式の方が先に判定されるので、mapの範囲外にGetValueすることはない（と思う）
        {
            upIsWall = true;
        }

        if (x == 0 || IsWall(map.GetValue(x - 1, y)))
        {
            leftIsWall = true;
        }

        if (y == 0 || IsWall(map.GetValue(x, y - 1)))
        {
            downIsWall = true;
        }

        if (x == map.Width - 1 || IsWall(map.GetValue(x + 1, y)))
        {
            rightIsWall = true;
        }


        if (x == 0 || y == map.Height - 1 || IsWall(map.GetValue(x - 1, y + 1))) //この4つの場合分けは4隅を調べればよいので、xだけの判定で十分
        {
            upleftWallValue = 1;
        }

        if (x == 0 || y == 0 || IsWall(map.GetValue(x - 1, y - 1)))
        {
            downleftWallValue = 1;
        }

        if (x == map.Width - 1 || y == 0 || IsWall(map.GetValue(x + 1, y - 1)))
        {
            downrightWallValue = 1;
        }

        if (x == map.Width - 1 || y == map.Height - 1 || IsWall(map.GetValue(x + 1, y + 1)))
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

}
