using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.Simulation.Statistics;

public class PlayerState : EntityState
{
    protected PlayerStateHandler player;
    
    protected float airTime;


    //protected int currentStateNum;        현재 스테이트 넘
    //protected float stateTimer;
    //protected float startTime;                
    //protected bool endMotionChange = true; 끝나면 애니메이션 바꿔라트리거
    //protected bool isAbleFly = false; 이거 제거 
    //protected bool isAbleAttack = true;

    public PlayerState(PlayerStateHandler _player, int _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
    }
    public override void Enter()
    {
        base.Enter();
        startTime = Time.time;
        player.SetState(currentStateNum);
        player.SetState2(0);

        //player.SetInt("State", currentStateNum);
        if (currentStateNum != 0) { player.animationTrigger = true; }
    }
    public override bool Update()
    {
        base.Update();
        stateTimer = Time.time;

        //공격
        if(Input.GetKeyDown(KeyCode.X) && isAbleAttack) 
        {
            player.nextState = player.attackState;

            //player.StateChange(player.attackState);
            return true;
        }
        if (!isAbleFly)
        {
            //체공 불가능 상태일때
            if (!player.IsGround())
            {
                if(airTime == 0f)
                {
                    airTime = Time.time;
                }
                if(Time.time - airTime > 0.25f)
                {
                    player.nextState = player.fallState;

                    //player.StateChange(player.fallState);
                    return true;
                }
            }
            else
            {
                airTime = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.C) && isAbleDodge)
        {
            player.nextState = player.attackState;
            //player.StateChange(player.attackState);
            return true;
        }
        return BaseState();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }
    public override void LateUpdate()
    {
        //Debug.Log($"currentStateNum = {currentStateNum} this = {this}");

        if (player.nextState != this)
        {
            player.ChangeState();
        }
        //Debug.Log($"체인지 스태이트 player.nextState = {player.nextState}  this = {this}");
    }
    public override void Exit()
    {
        base.Exit();
    }
    

    
    protected bool BaseState()
    {
        
        if (!player.animationTrigger && endMotionChange)
        {
            if (player.IsGround()) 
            {
                player.nextState = player.moveState;

                //player.StateChange(player.moveState);
                return true;
            }
            else
            {
                player.nextState = player.fallState;

                //player.StateChange(player.fallState);
                return true;

            }
        }
        return false;

    }


}
