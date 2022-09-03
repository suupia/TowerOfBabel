using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TowerOfBabelで使いやすいようにEntityMapDataを改良する
public class BabelMapData : EntityMapData<global::Entity >
{
    //コンストラクタ
    public BabelMapData(int initValue, int width, int height, int entityNum) : base(initValue, width, height,entityNum)
    {

    }



    public void AddBrickYard(int x, int y, int entityIndex, BrickYard entity)
    {
        AddEntity(x,y,entityIndex,entity);
    }
}