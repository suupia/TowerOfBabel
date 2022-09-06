using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMGR : EntityMGR
{


    protected Animator animator;

    List<Vector2Int> routeList;
    bool isMoving;

    protected float spd = 3; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
    protected float moveTime; // movetime = 1/spd [s]
    protected int moveAlongWithCounter = 0;

    protected Direction direction;
    protected enum Direction
    {
        Front,
        Back,
        Right,
        Left,
        DiagRightFront,
        DiagLeftFront,
        DiagRightBack,
        DiagLeftBack
    }

    //仮置き
    bool isOnMoveAlongWith;


    private void Update()
    {
        if(isOnMoveAlongWith) MoveAlongWith();
    }
    public  void Init(Vector2Int brickYardPos, Vector2Int towerPos)
    {
        EntityInit();

        animator = this.gameObject.GetComponent<Animator>();

        moveTime = 1.0f / spd;

        DecideRoute(brickYardPos,towerPos);
        isOnMoveAlongWith = true;
    }

    private void DecideRoute(Vector2Int brickYardPos, Vector2Int towerPos)
    {
        Vector2Int startPos;
        Vector2Int endPos;

        startPos = gridPos;
        endPos = new Vector2Int (1,1);

        int mapWidth = GameManager.instance.battleMGR.mapMGR.GetMap().GetWidth();
        int mapHeight = GameManager.instance.battleMGR.mapMGR.GetMap().GetHeight();

        //routeList = WaveletSearch.DiagonalSearchShortestRoute(startPos, endPos, WaveletSearch.OrderInDirection.LeftDown, mapWidth, mapHeight, 2, (vector, wallID) => { return GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(vector) < 0 ? true : false; });

        routeList = WaveletSearch.DiagonalSearchShortestRoute(startPos, brickYardPos, WaveletSearch.OrderInDirection.LeftDown, mapWidth, mapHeight, 2, (vector, wallID) => { return GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(vector) < 0 ? true : false; });
        routeList.RemoveAt(routeList.Count - 1); //BrickYardがあるマスには行かない
        Debug.LogWarning($"brickYardPosまでのルート:{string.Join(",", routeList)}");
        Vector2Int halfPos = routeList[routeList.Count - 1];
            routeList.RemoveAt(routeList.Count - 1); //連結のために削除する必要がある
        routeList.AddRange(WaveletSearch.DiagonalSearchShortestRoute(halfPos, towerPos, WaveletSearch.OrderInDirection.LeftDown, mapWidth, mapHeight, 2, (vector, wallID) => { return GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(vector) < 0 ? true : false; }));
        routeList.RemoveAt(routeList.Count - 1); //Towerがあるマスには行かない
        Debug.LogWarning($"towerPosまでのルート:{string.Join(",", routeList)}");




    }

    protected virtual void March() //Update()で呼ばれることに注意
    {
        MoveAlongWith();
    }
    protected void MoveAlongWith()
    {
        Vector2Int nextPos;

        if (isMoving) return;

        ////ターゲットのユニットが攻撃範囲内にいるとき、InBattleに切り替える
        //if (Function.isWithinTheAttackRange(gridPos, atkRange, targetUnitID, out targetUnitPos))
        //{
        //    Debug.Log($"攻撃範囲内にユニットがあるのでInBattleに切り替えます targetUnitPos:{targetUnitPos}");
        //    if (isAttackOfArea)
        //    {
        //        //範囲攻撃
        //        targetUnits.Clear();
        //        foreach (Unit unit in GameManager.instance.mapMGR.GetMap().GetUnitList(targetUnitPos))
        //        {
        //            targetUnits.Add(unit);
        //        }
        //    }
        //    else
        //    {
        //        //単体攻撃
        //        targetUnits.Clear();
        //        targetUnits.Add(GameManager.instance.mapMGR.GetMap().GetUnitList(targetUnitPos)[0]);  //マスの中の先頭のユニットのスクリプトを取得
        //    }
        //    Debug.Log($"targetUnit:{targetUnits}");
        //    SetDirection(targetUnitPos - gridPos);
        //    isTargetUnit = true;
        //    state = State.InBattle;
        //    return;
        //}

        ////ターゲットの施設が攻撃範囲内にいるとき、InBattleに切り替える
        //if (Function.isWithinTheAttackRange(gridPos, atkRange, targetTowerID, out targetFacilityPos) || Function.isWithinTheAttackRange(gridPos, atkRange, targetCastleID, out targetFacilityPos)) //ルートに沿って移動しているときに、攻撃範囲内にタワー（城を除く）があるとき
        //{
        //    Debug.Log($"攻撃範囲内にタワーがあるのでInBattleに切り替えます targetFacilityPos:{targetFacilityPos}");
        //    targetFacility = GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos); //タワーのスクリプトを取得
        //    SetDirection(targetFacilityPos - gridPos);
        //    isTargetUnit = false;
        //    state = State.InBattle;
        //    return;
        //}

        //if (moveAlongWithCounter == routeList.Count - 1)  //ルートの最終地点に到達したら城への攻撃を開始する
        //{
        //    Debug.Log($"ルートの最終地点にいるためInBattleに切り替えます");
        //    SetDirection(GameManager.instance.mapMGR.GetEnemysCastlePos() - gridPos);
        //    state = State.InBattle;
        //    return;
        //}


        nextPos = routeList[moveAlongWithCounter + 1];

        SetDirection(nextPos - gridPos);

        if (CanMove(nextPos - gridPos))
        {
            MoveForward();
            moveAlongWithCounter++;
        }
        else
        {
            Debug.LogError($"CanMove({nextPos - gridPos})がfalseなので移動できません");
        }

    }



    private bool CanMove(Vector2Int vector)
    {

        if (GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(gridPos + vector) < 0)
        {
            Debug.LogError($"移動先が壁のため、移動できません(gridPos:{gridPos}vector:{vector})\nGameManager.instance.mapMGR.GetMapValue(gridPos + vector)={GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(gridPos + vector)} GetDirectionVector={GetDirectionVector()}");
            return false;
        }

        //斜め移動の時にブロックの角を移動することはできない
        if (vector.x != 0 && vector.y != 0)
        {
            //水平方向の判定
            if (GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(gridPos.x + vector.x, gridPos.y)< 0)
            {
                Debug.LogError($"水平方向に壁があるため、移動できません。");

                return false;
            }

            //垂直方向の判定
            if (GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(gridPos.x, gridPos.y + vector.y)< 0)
            {
                Debug.LogError($"鉛直方向に壁があるため、移動できません。");
                return false;
            }
        }

        return true;
    }
    private void MoveForward()
    {
        //Debug.Log("MoveForwardを実行します");
        if (!isMoving) StartCoroutine(MoveForwardCoroutine());
    }

    private IEnumerator MoveForwardCoroutine()  //Characterをゆっくり動かす関数
    {
        Debug.Log("MoveCoroutineを実行します");
        Vector2 startPos;
        Vector2 endPos;

        isMoving = true;

        MoveData(GetDirectionVector()); //先にMoveDateを行う



        startPos = transform.position;
        endPos = startPos + GetDirectionVector();

        //Debug.Log($"MoveForwardCoroutineにおいてstartPos:{startPos},endPos{endPos}");


        float remainingDistance = (endPos - startPos).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / moveTime * Time.deltaTime * GameManager.instance.gameSpeed);
            //3つ目の引数は"1フレームの最大移動距離"　単位は実質[m/s](コルーチンが1フレームずつ回っているからTime.deltaTimeが消える。moveTime経った時に1マス進む。)

            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;


            //while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める


            yield return null;  //1フレーム停止させる。
        }
        transform.position = endPos;//ループを抜けた時はきっちり移動させる。



        isMoving = false;
        //Debug.Log($"MoveCoroutine()終了時のendPosは{endPos}");
    }

    private void MoveData(Vector2Int directionVector)
    {

        if (GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(gridPos) % GameManager.instance.p1UnitID ==0 ) //p1
        {
            //数値データの移動
            GameManager.instance.battleMGR.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.p1UnitID);
            GameManager.instance.battleMGR.mapMGR.GetMap().MultiplySetValue(gridPos + directionVector, GameManager.instance.p1UnitID);

            //スクリプトの移動
            GameManager.instance.battleMGR.mapMGR.GetMap().RemoveP1UnitMGR(gridPos, this);
            GameManager.instance.battleMGR.mapMGR.GetMap().AddP1UnitMGR(gridPos + directionVector, this.gameObject.GetComponent<UnitMGR>());
        }
        else if (GameManager.instance.battleMGR.mapMGR.GetMap().GetValue(gridPos) % GameManager.instance.p2UnitID == 0) //p2
        {
            //数値データの移動
            GameManager.instance.battleMGR.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.p2UnitID);
            GameManager.instance.battleMGR.mapMGR.GetMap().MultiplySetValue(gridPos + directionVector, GameManager.instance.p2UnitID);

            //スクリプトの移動
            GameManager.instance.battleMGR.mapMGR.GetMap().RemoveP2UnitMGR(gridPos, this);
            GameManager.instance.battleMGR.mapMGR.GetMap().AddP2UnitMGR(gridPos + directionVector, this.gameObject.GetComponent<UnitMGR>());
        }
        else
        {
            Debug.LogError("MoveDateで移動前のmapValueにunitIDが含まれていません");
            return;
        }

        //gridPosを移動させる。これは最後に行うことに注意！
        gridPos += directionVector;

    }

    public override void OnClicked(bool isP1)
    {
        //何もしない
    }

    //Getter
    public Vector2Int GetGridPos()
    {
        return gridPos;
    }
    public Vector2Int GetDirectionVector()
    {
        Vector2Int resultVector2Int = new Vector2Int(0, 0);
        switch (direction)
        {
            case Direction.Back:
                resultVector2Int = new Vector2Int(0, 1);
                break;

            case Direction.DiagLeftBack:
                resultVector2Int = new Vector2Int(-1, 1);
                break;

            case Direction.Left:
                resultVector2Int = new Vector2Int(-1, 0);
                break;

            case Direction.DiagLeftFront:
                resultVector2Int = new Vector2Int(-1, -1);
                break;

            case Direction.Front:
                resultVector2Int = new Vector2Int(0, -1);
                break;

            case Direction.DiagRightFront:
                resultVector2Int = new Vector2Int(1, -1);
                break;

            case Direction.Right:
                resultVector2Int = new Vector2Int(1, 0);
                break;

            case Direction.DiagRightBack:
                resultVector2Int = new Vector2Int(1, 1);
                break;
        }
        if (resultVector2Int == new Vector2Int(0, 0))
        {
            Debug.LogError("GetDirectionVector()の戻り値が(0,0)になっています");
        }
        return resultVector2Int;
    }


    //Setter
    public void SetDirection(Vector2 directionVector)
    {
        if (directionVector == Vector2.zero) //引数の方向ベクトルがゼロベクトルの時は何もしない
        {
            return;
        }

        float angle = Vector2.SignedAngle(Vector2.right, directionVector);
        //Debug.Log($"SetDirectionのangleは{angle}です");


        //先に画像の向きを決定する
        if (directionVector.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); //元の画像が左向きのため
        }
        else if (directionVector.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            //前か後ろ向きの時は何もしない
        }

        //directionとanimationを決定する
        if (-22.5f <= angle && angle < 22.5f)
        {
            direction = Direction.Right;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 0);
        }
        else if (22.5f <= angle && angle < 67.5f)
        {
            direction = Direction.DiagRightBack;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 1);
        }
        else if (67.5f <= angle && angle < 112.5f)

        {
            direction = Direction.Back;
            animator.SetBool("Horizontal", false);
            animator.SetInteger("Vertical", 1);
        }
        else if (112.5f <= angle && angle < 157.5f)
        {
            direction = Direction.DiagLeftBack;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 1);
        }
        else if (-157.5f <= angle && angle < -112.5f)
        {
            direction = Direction.DiagLeftFront;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", -1);
        }
        else if (-112.5f <= angle && angle < -67.5f)
        {
            direction = Direction.Front;
            animator.SetBool("Horizontal", false);
            animator.SetInteger("Vertical", -1);
        }
        else if (-67.5f <= angle && angle < -22.5f)
        {
            direction = Direction.DiagRightFront;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", -1);
        }
        else //角度は-180から180までで端点は含まないらしい。そのため、Direction.Leftはelseで処理することにした。
        {
            direction = Direction.Left;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 0);
        }
    }
}
