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

    [SerializeField] GameObject entityParent;
    [SerializeField] GameObject brickYardPrefab;
    [SerializeField] GameObject SpawnPointPrefab;
    [SerializeField] GameObject towerPrefab;


    int outOfStageQ = 10; //マップの外にタイルを何枚はみ出して張るかを決める


    int maxStageNum = 2; //とりあえず2にしておく

    string mapDataPath = "JSON/mapData";

    Map map;


    public void Init(int stageIndex) //これをGameManagerから呼ぶ
    {
        Debug.Log($"MapMGRのInitを実行します");

        InitMap(stageIndex);

        PlaceEntitys();

        RenderMap();

    }

    private void InitMap(int stageIndex)
    {
        MapData mapData = new MapData(maxStageNum, mapDataPath);
        map = new Map(GameManager.instance.wallID_m1, mapData.GetWidth(stageIndex), mapData.GetHeight(stageIndex), stageIndex); //とりあえずTowerMGRだけなので最後の引数は1にしておく

        for (int i = 0; i < mapData.GetLength(stageIndex); i++)
        {
            // Debug.Log($"GetCellValue(stageIndex,{i}):{mapData.GetCellValue(stageIndex, i)}");

            map.SetValue(i, mapData.GetCellValue(stageIndex, i));

        }
    }
    private void PlaceEntitys()
    {
        System.Func<long,int, bool> IsDesiredID;

        //mapValueからIDを計算する計算式
        IsDesiredID = (mapValue,desiredID) => { return Mathf.Abs(mapValue) %  desiredID == 0 ? true : false; };

        for (int y = 0; y < map.GetHeight(); y++)
        {
            for(int x = 0; x < map.GetWidth(); x++)
            {
                if (IsDesiredID(map.GetValue(x,y),GameManager.instance.brickYardID))
                {
                    PlaceBrickYard(x, y);
                }else if (IsDesiredID(map.GetValue(x, y), GameManager.instance.p1SpawnPointID))
                {
                    PlaceSpawnPointMGR(x, y,true);

                }
                else if (IsDesiredID(map.GetValue(x, y), GameManager.instance.p2SpawnPointID))
                {
                    PlaceSpawnPointMGR(x, y,false);

                }
                else if(IsDesiredID(map.GetValue(x, y), GameManager.instance.p1TowerID))
                {
                    PlaceTower(x, y,true);

                }
                else if (IsDesiredID(map.GetValue(x, y), GameManager.instance.p2TowerID))
                {
                    PlaceTower(x, y,false);

                }
            }
        }
    }
    private void PlaceBrickYard(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x,y);

        GameObject brickYardGO = Instantiate(brickYardPrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity,entityParent.transform);
        BrickYardMGR brickYardMGR = brickYardGO.GetComponent<BrickYardMGR>(); 

        map.AddBrickYardMGR(pos,brickYardMGR);

        brickYardMGR.Init();

    }
    private void PlaceSpawnPointMGR(int x, int y, bool isP1)
    {
        Vector2Int pos = new Vector2Int(x, y);

        GameObject spwanPointGO = Instantiate(SpawnPointPrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity, entityParent.transform);
        SpawnPointMGR spawnPointMGR = spwanPointGO.GetComponent<SpawnPointMGR>();

        if (isP1)
        {
            map.AddP1SpawnPointMGR(pos, spawnPointMGR);
            spawnPointMGR.Init(isP1);

        }
        else
        {
            map.AddP2SpawnPointMGR(pos,spawnPointMGR);
            spawnPointMGR.Init(isP1);

        }
    }
    private void PlaceTower(int x, int y, bool isP1)
    {
        Vector2Int pos = new Vector2Int(x, y);

        GameObject towerGO = Instantiate(towerPrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity, entityParent.transform);
        TowerMGR towerMGR = towerGO.GetComponent<TowerMGR>();

        if (isP1)
        {
            map.AddP1TowerMGR(pos, towerMGR);
            towerMGR.Init(isP1);

        }
        else
        {
            map.AddP2TowerMGR(pos, towerMGR);
            towerMGR.Init(isP1);

        }
    }


    private void RenderMap()
    {
        //マップをクリアする（重複しないようにする）
        tilemap.ClearAllTiles();

        for (int y = -outOfStageQ; y < map.GetHeight() + outOfStageQ; y++)
        {
            for (int x = -outOfStageQ; x < map.GetWidth() + outOfStageQ; x++)
            {
                SetTileAccordingToValues(x, y);
            }
        }

    }


    private void SetTileAccordingToValues(int x, int y)
    {
        System.Func<long, bool> IsWall;

        IsWall = (mapValue) => { return mapValue < 0 ? true : false; };

        //mapの領域外
        if (0 > y || y > map.GetHeight() - 1 || 0 > x || x > map.GetWidth() - 1)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]); //全方向が壁のタイルを張る(3枚)
            //Debug.Log($"壁のタイルをタイルを{x},{y}に敷き詰めました（領域外）");

            return;
        }

        //負の数なら壁、正の数なら地面
        if (map.GetValue(x, y) < 0)
        {
            if (MapFunction.CalculateTileType(x, y, map, IsWall) < 47) //周りがすべて壁のタイルは3種類ある
            {
                //少なくとも一つの方向は地面のタイルである壁のタイル
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[MapFunction.CalculateTileType(x, y, map, IsWall)]);
                //Debug.Log($"GetValue:{map.GetValue(x, y)}、地面に接する壁、({x},{y})、タイル番号：{MapFunction.CalculateTileType(x, y, map, IsWall)}");
            }
            else
            {
                //周りがすべて壁のタイル
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                //Debug.Log($"壁に囲まれた壁、({x},{y})、タイル番号：47から49の乱数");

            }
        }
        else if (map.GetValue(x, y) > 0)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[map.GetStageIndex()].stageTileArray[UnityEngine.Random.Range(50, 52 + 1)]);
            //Debug.Log($"地面、({x},{y})、タイル番号：50から52の乱数");


        }
        else
        {
            Debug.LogError($"mapに予期せぬ値が入っています {map.GetValue(x, y)}");
        }

    }

    //Getter
    public Map GetMap()
    {
        return map;
    }
    //Setter
    //public void DivisionalSetMapValue(Vector2Int pos, int value)
    //{
    //    map.DivisionalSetValue(pos, value);
    //}
    //public void MultiplySetMapValue(Vector2Int pos, int value)
    //{
    //    map.MultiplySetValue(pos, value);
    //}
    //public void RemoveP1Unit(Vector2Int pos, UnitMGR unitMGR)
    //{
    //    map.RemoveP1UnitMGR(pos, unitMGR);
    //}
    //public void RemoveP2Unit(Vector2Int pos, UnitMGR unitMGR)
    //{
    //    map.RemoveP2UnitMGR(pos, unitMGR);
    //}
}


public class Map : EntityMap<global::EntityMGR>
{
    //MapMGRに沿うように改良されたEntityMap


    const int entityNum = 6; //マップに配置されうるものの種類(下のEIの種類数と同じ)
    //これらはEntityMapで使用されるので重複がなければ値は自由
    const int brickYard_EI = 0; //EI = Entity Index
    const int p1SpawnPointMGR_EI = 1;
    const int p2SpawnPointMGR_EI = 2;
    const int p1Tower_EI = 3;
    const int p2Tower_EI = 4;
    const int p1Unit_EI = 5;
    const int p2Unit_EI = 6;



    int _stageIndex;

    //コンストラクタ
    public Map(int initValue, int width, int height, int stageIndex) : base(initValue, width, height, entityNum)
    {
        _stageIndex = stageIndex;
    }

    public bool IDisExit(Vector2Int pos, int desiredID)
    {
        return Mathf.Abs(GetValue(pos)) % desiredID == 0 ? true : false;
    }

    public bool IsOutOfMap(Vector2Int pos)
    {
      return  IsOutOfDataRange(pos.x, pos.y);
    } 

    //Getter
    public int GetStageIndex()
    {
        return _stageIndex;
    }
    public BrickYardMGR GetBrickYardMGR(Vector2Int pos)
    {
        return (BrickYardMGR)GetEntityList(pos, brickYard_EI)[0];
    }
    public SpawnPointMGR GetP1SpawnPointMGR(Vector2Int pos)
    {
        return (SpawnPointMGR)GetEntityList(pos, p1SpawnPointMGR_EI)[0];
    }
    public SpawnPointMGR GetP2SpawnPointMGR(Vector2Int pos)
    {
        return (SpawnPointMGR)GetEntityList(pos, p2SpawnPointMGR_EI)[0];
    }
    public TowerMGR GetP1TowerMGR(Vector2Int pos)
    {
        return (TowerMGR)GetEntityList(pos, p1Tower_EI)[0];
    }
    public TowerMGR GetP2TowerMGR(Vector2Int pos)
    {
        return (TowerMGR)GetEntityList(pos, p2Tower_EI)[0];
    }

    //Setter

    public void AddBrickYardMGR(Vector2Int vector, BrickYardMGR entity)
    {
        AddEntity(vector.x, vector.y, brickYard_EI, entity);
    }
    public void RemoveBrickYardMGR(Vector2Int vector, BrickYardMGR entity)
    {
        RemoveEntity(vector.x, vector.y, brickYard_EI, entity);
    }

    public void AddP1SpawnPointMGR(Vector2Int vector, SpawnPointMGR entity)
    {
        AddEntity(vector.x, vector.y, p1SpawnPointMGR_EI, entity);
    }
    public void AddP2SpawnPointMGR(Vector2Int vector, SpawnPointMGR entity)
    {
        AddEntity(vector.x, vector.y, p2SpawnPointMGR_EI, entity);
    }

    public void RemoveP1SpawnPointMGR(Vector2Int vector, SpawnPointMGR entity)
    {
        RemoveEntity(vector.x, vector.y, p1SpawnPointMGR_EI, entity);
    }
    public void RemoveP2SpawnPointMGR(Vector2Int vector, SpawnPointMGR entity)
    {
        RemoveEntity(vector.x, vector.y, p2SpawnPointMGR_EI, entity);
    }

    public void AddP1TowerMGR(Vector2Int vector, TowerMGR entity)
    {
        AddEntity(vector.x, vector.y, p1Tower_EI, entity);
    }
    public void AddP2TowerMGR(Vector2Int vector, TowerMGR entity)
    {
        AddEntity(vector.x, vector.y, p2Tower_EI, entity);
    }
    public void RemoveP1TowerMGR(Vector2Int vector, TowerMGR entity)
    {
        RemoveEntity(vector.x, vector.y, p1Tower_EI, entity);
    }
    public void RemoveP2ToweMGRr(Vector2Int vector, TowerMGR entity)
    {
        RemoveEntity(vector.x, vector.y, p2Tower_EI, entity);
    }
    public void AddP1UnitMGR(Vector2Int vector, UnitMGR entity)
    {
        AddEntity(vector.x, vector.y, p1Unit_EI, entity);
    }
    public void AddP2UnitMGR(Vector2Int vector, UnitMGR entity)
    {
        AddEntity(vector.x, vector.y, p2Unit_EI, entity);
    }
    public void RemoveP1UnitMGR(Vector2Int vector, UnitMGR entity)
    {
        RemoveEntity(vector.x, vector.y, p1Unit_EI, entity);
    }
    public void RemoveP2UnitMGR(Vector2Int vector, UnitMGR entity)
    {
        RemoveEntity(vector.x, vector.y, p2Unit_EI, entity);
    }
}

public static class MapFunction
{
    //MapをTilemapにレンダリングするときに使用するクラス

    public static int CalculateTileType(int x, int y, Map map, System.Func<long, bool> IsWall)
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

        if (y == map.GetHeight() - 1 || IsWall(map.GetValue(x, y + 1))) //左側の条件式の方が先に判定されるので、mapの範囲外にGetValueすることはない（と思う）
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

        if (x == map.GetWidth() - 1 || IsWall(map.GetValue(x + 1, y)))
        {
            rightIsWall = true;
        }


        if (x == 0 || y == map.GetHeight() - 1 || IsWall(map.GetValue(x - 1, y + 1))) //この4つの場合分けは4隅を調べればよいので、xだけの判定で十分
        {
            upleftWallValue = 1;
        }

        if (x == 0 || y == 0 || IsWall(map.GetValue(x - 1, y - 1)))
        {
            downleftWallValue = 1;
        }

        if (x == map.GetWidth() - 1 || y == 0 || IsWall(map.GetValue(x + 1, y - 1)))
        {
            downrightWallValue = 1;
        }

        if (x == map.GetWidth() - 1 || y == map.GetHeight() - 1 || IsWall(map.GetValue(x + 1, y + 1)))
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
