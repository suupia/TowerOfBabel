using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugMGR : MonoBehaviour
{
    [SerializeField] GameObject debugMapTextParent;
    GameObject[] debugMapTextRows;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) DebugMapCoordinate();
    }
    public void DebugMapCoordinate()
    {
        Debug.Log($"DebugMapCoordinateを実行します");
        int mapWidth = GameManager.instance.battleMGR.mapMGR.GetMapWidth();
        int mapHeight = GameManager.instance.battleMGR.mapMGR.GetMapHeight();

        //行でループのあと、列のループをする
        for(int i = 0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                Debug.Log($"i:{i}, j:{j}");
                debugMapTextParent.transform.GetChild(i).GetChild(j).gameObject.GetComponent<TextMeshProUGUI>().text = $"({i}, {j})";
            }
        }

    }
}
