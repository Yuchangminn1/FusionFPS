using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState 
{
    public int currentStateNum { get; protected set; }
    public EntityState currentState { get;protected set; }
    //protected float stateTimer;
    protected float startTime;
    protected bool endMotionChange = false;    
    protected bool isAbleFly = false;
    protected bool isState2 = false;
    public bool isAbleAttack { get; protected set; } = true;
    protected bool isAbleDodge = true;
    public bool isAbleJump { get; protected set; } = true;
    public bool isCancel { get; protected set; } = false;

    public virtual void Enter()
    {
        startTime = Time.time;
    }
    public virtual bool Update()
    {
        return false;
    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void LateUpdate()
    {

    }

    public virtual void Exit()
    {

    }
    


    



}