using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : EntityState
{
    protected PlayerStateHandler player;
    
    protected float airTime;


    //protected int currentStateNum;        ���� ������Ʈ ��
    //protected float stateTimer;
    //protected float startTime;                
    //protected bool endMotionChange = true; ������ �ִϸ��̼� �ٲ��Ʈ����
    //protected bool isAbleFly = false; �̰� ���� 
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

        //����
        if(Input.GetKeyDown(KeyCode.X) && isAbleAttack) 
        {
            player.StateChange(player.attackState);
            return true;
        }
        if (!isAbleFly)
        {
            //ü�� �Ұ��� �����϶�
            if (!player.IsGround())
            {
                if(airTime == 0f)
                {
                    airTime = Time.time;
                }
                if(Time.time - airTime > 0.25f)
                {
                    player.StateChange(player.fallState);
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
            player.StateChange(player.attackState);
            return true;
        }
        return BaseState();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Debug.Log($"currentStateNum = {currentStateNum}");
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
                player.StateChange(player.moveState);
                return true;
            }
            else
            {
                player.StateChange(player.fallState);
                return true;

            }
        }
        return false;

    }


}
