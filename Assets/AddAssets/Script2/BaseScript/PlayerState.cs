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
        player.state = currentStateNum;
        player.SetState(player.state);
        startTime = Time.time;
        

        if (currentStateNum != 0) { player.animationTrigger = true; }
    }
    public override bool Update()
    {
        base.Update();
        stateTimer = Time.time;
        
        //����
        if (player.isFireButtonPressed && isAbleAttack) 
        {
            player.nextState = player.attackState;

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
                if(Time.time - airTime > 0.4f)
                {
                    player.nextState = player.fallState;

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
            return true;
        }


        return BaseState();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        //Ʋ���� �ִϸ��̼� �����ֱ� ? 
        if (player.state != currentStateNum)
        {
            //Debug.Log($"player.state = {player.state} currentStateNum{currentStateNum}");
            player.SetState(currentStateNum);
        }
        //Debug.Log($"currentNum = {currentStateNum}");

    }
    public override void LateUpdate()
    {
        if (player.IsGround() && player.state != 1)
        {
            //Debug.Log("MoveState ���� ī��Ʈ �ʱ�ȭ");
            //�̰͵� ���߿� ��������
            player.dodgeCount = 0f;
        }

        if (player.nextState != this)
        {
            player.ChangeState();
        }
    }
    public override void Exit()
    {
        base.Exit();
        if (currentStateNum != 0)
        {
            player.animationTrigger = false;
        }
    }
    

    
    protected bool BaseState()
    {
        if (!player.animationTrigger && endMotionChange)
        {
            if (player.IsGround()) 
            {
                player.nextState = player.moveState;
                return true;
            }
            else
            {
                player.nextState = player.fallState;
                return true;
            }
        }
        return false;
    }
}
