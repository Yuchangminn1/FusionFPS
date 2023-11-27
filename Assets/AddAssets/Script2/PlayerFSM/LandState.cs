using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandState : PlayerState
{
    [SerializeField] private float landTime = 1f;

    public LandState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleFly = true;
        endMotionChange = false;

        isAbleAttack = false;
        isAbleDodge = false;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("���� ����");
        player.isStop = true;
        //if(player.state2 >= 2)
        //{
        //    player.state2 = 1;
        //    player.SetState2(player.state2);
        //}
    }
    public override bool Update()
    {

        player.nextState = player.moveState;
        return true;
        if (!player.animationTrigger)
        {
            if (base.Update())
                return true;

            player.nextState = player.moveState;
            return true;
        }
        return false;
        
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.state2 = 0;
        player.isStop = false;
        player.animationTrigger = false;

    }


}
