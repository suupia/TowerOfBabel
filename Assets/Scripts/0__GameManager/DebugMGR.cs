using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugMGR : MonoBehaviour
{
    [SerializeField] GameObject debugCanvas;
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
        GameObject textGO;

        //行でループのあと、列のループをする
        for(int i = 0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                Debug.Log($"i:{i}, j:{j}");
                textGO = debugMapTextParent.transform.GetChild(i).GetChild(j).gameObject;

                //textの内容を変更
                textGO.GetComponent<TextMeshProUGUI>().text = $"({i}, {j})";

                //textの位置を変更
                textGO.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector3(i+0.5f, j+0.5f, 0));


                //Vector2Int vector = new Vector2Int(j, j);
                //var newPos = Vector2.zero;
                //var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector3(vector.x, vector.y,0));
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(debugCanvas.GetComponent<RectTransform>(), screenPos, Camera.main, out newPos);
                //textGO.GetComponent<RectTransform>().localPosition = newPos;


            }
        }

    }
}
