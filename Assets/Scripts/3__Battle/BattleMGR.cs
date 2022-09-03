using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMGR : MonoBehaviour
{
    public MapMGR mapMGR;
    private void OnEnable()
    {
        EnableInit();
    }

    public void EnableInit()
    {
        Debug.Log($"BattleMGRのEnableInitを実行します");
        mapMGR.Init(0);
    }
}
