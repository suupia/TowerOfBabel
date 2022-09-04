using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericMap
{
    //数字だけを格納することができるマップ
    int _width;
    int _height;
    long[] _values = null;
    int _initValue;
    int _outOfRangeValue = -1;

    //データが存在する領域、端の領域、それ以外　の３つの領域がある

    //コンストラクタ
    public NumericMap(int initValue, int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Mapの幅または高さが0以下になっています");
            return;
        }
        _width = width;
        _height = height;
        _values = new long[width * height];

        FillAll(initValue);
    }

    //プロパティ
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }

    //Getter
    public int GetLength()
    {
        return _values.Length;
    }
    public long GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return _outOfRangeValue;
        }
        return _values[index];
    }
    public long GetValue(int x, int y)
    {
        if (IsOutOfEdge(x, y))
        {
            //edgeの外側
            Debug.LogError($"IsOutOfRange({x},{y})がtrueです");
            return _outOfRangeValue;
        }
        if (IsOnTheEdge(x, y))
        {
            //edgeの上
            //データは存在しないが、判定のために初期値を使いたい場合
            //Debug.Log($"IsOnTheEdge({x},{y})がtrueです");
            return _initValue;
        }
        return _values[ToSubscript(x, y)];
    }
    public long GetValue(Vector2Int vector)
    {
        return GetValue(vector.x, vector.y);
    }


    //Setter
    public void SetValue(int index , int value)
    {
        if (index <0 || index > _values.Length-1)
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return;
        }
        _values[index] = value;
    }
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfEdge(x, y))
        {
            Debug.LogError($"IsOutOfRange({x},{y})がtrueです");
            return;
        }
        _values[ToSubscript(x, y)] = value;
    }
    public void SetValue(Vector2Int vector, int value)
    {
        SetValue(vector.x, vector.y, value);
    }
    public void MultiplySetValue(Vector2Int vector, int value)
    {
        int x = vector.x;
        int y = vector.y;

        if (IsOutOfEdge(x, y))
        {
            Debug.LogError($"MultiplySetValue({x},{y})で領域外に値{value}を設定しようとしました");
            return;
        }

        _values[ToSubscript(x, y)] *= value;
    }

    public void DivisionalSetValue(Vector2Int vector, int value)
    {
        int x = vector.x;
        int y = vector.y;

        if (IsOutOfEdge(x, y))
        {
            Debug.LogError($"DivisionalSetValue({x},{y})で領域外に値{value}を設定しようとしました");
            return;
        }
        if (GetValue(x, y) % value != 0)
        {
            Debug.LogError($"DivisionalSetValue(vector:{vector},value:{value})で余りが出たため実行できません");
            return;
        }

        _values[ToSubscript(x, y)] /= value;
    }


    //添え字を変換する
    protected int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    protected Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //ここは割り算
        return new Vector2Int(xSub, ySub);
    }

    //判定用関数
    protected bool IsOutOfEdge(int x, int y) //edgeの外側。つまり、データがedgeValueすらない
    {
        if (x < -1 || x > _width) { return true; }
        if (y < -1 || y > _height) { return true; }

        //mapの中
        return false;
    }

    protected bool IsOnTheEdge(int x, int y) //データは存在しないが、_initValueを返すマス
    {
        if (x == -1 || x == _width) { return true; }
        if (y == -1 || y == _height) { return true; }
        return false;
    }

    protected bool IsOutOfDataRange(int x, int y) //座標(0,0)～(mapWidht-1,mapHeight-1)のデータが存在する領域の外側
    {
        if (x < 0 || x > _width - 1) { return true; }
        if (y < 0 || y > _height - 1) { return true; }
        return false;
    }

    //初期化で利用
    public void FillAll(int value)
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                _values[ToSubscript(i, j)] = value;
            }
        }
    }
}
public class EntityMap<EntityType> : NumericMap
{
    //数字に加えてゲームオブジェクトにアタッチされたスクリプトのインスタンスも格納できるマップ


    List<EntityType>[][] entityMaps;

    //コンストラクタ
    public EntityMap(int initValue, int width, int height, int entityNum) : base(initValue, width, height)
    {
        entityMaps = new List<EntityType>[entityNum][];
        for (int i = 0; i <entityNum;i++)
        {
            entityMaps[i] = new List<EntityType>[GetLength()];
            for(int j = 0; j <GetLength(); j++)
            {
                entityMaps[i][j] = new List<EntityType>();
            }
        }
    }


    //Getter
    public List<EntityType> GetEntityList(int x, int y, int entityIndex)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return null; //例外用の数字を設定できないため、nullを返す
        }
        return entityMaps[entityIndex][ToSubscript(x, y)];
    }
    public List<EntityType> GetEntityList(Vector2Int vetor, int entityIndex)
    {
        return GetEntityList(vetor.x, vetor.y, entityIndex);
    }
    public List<EntityType> GetEntityList(int index, int entityIndex)
    {
        if (index < 0 || index > GetLength())
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return null; //例外用の数字を設定できないため、nullを返す
        }
        return entityMaps[entityIndex][index];
    }

    //Setter
    public void AddEntity(int x, int y, int entityIndex, EntityType entity)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return;
        }
        entityMaps[entityIndex][ToSubscript(x, y)].Add(entity);
    }
    public void AddEntity(Vector2Int vector, int entityIndex, EntityType entity)
    {
        AddEntity(vector.x, vector.y, entityIndex, entity);
    }
    public void RemoveEntity(int x, int y, int entityIndex, EntityType entity)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return;
        }
        entityMaps[entityIndex][ToSubscript(x, y)].Remove(entity);
    }
    public void RemoveUnit(Vector2Int vector, int entityIndex, EntityType entity)
    {
        RemoveEntity(vector.x, vector.y, entityIndex, entity);
    }

}
