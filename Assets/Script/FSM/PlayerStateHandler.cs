using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : NetworkBehaviour
{
 //   public NetworkCharacterControllerPrototypeCustom networkCC;
    [Networked(OnChanged = nameof(OnSetStateChanged))]
    public int state { get; set; }

    [Networked(OnChanged = nameof(OnSetState2Changed))]
    public int state2 { get; set; }

    public NetworkString<_16> nickName { get; set; }


    public float dodgeCount = 0f;

    public int jumpCount = 0;

    public bool isdead = false;

    public bool isStop = false;
    public bool isJumping = false;

    //체력회복 
    public bool isHeal = false;
    public int fHpHeal = 40;
    public int healNum;
    public int healNumMax = 3;

    

    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer; // 땅인지 확인하기 위한 레이어 마스크


    public bool animationTrigger = false;
    [SerializeField] Animator anima;

    CharacterController cc;

    #region FSM
    protected StateMachine stateMachine;

    public MoveState moveState { get; private set; }
    public JumpState jumpState { get; private set; }
    public FallState fallState { get; private set; }
    public LandState landState { get; private set; }
    public AttackState attackState { get; private set; }
    public HitState hitState { get; private set; }
    public DodgeState dodgeState { get; private set; }
    public DeathState deathState { get; private set; }
    public HealState healState { get; private set; }


    #endregion
    void Awake()
    {
        stateMachine = new StateMachine();

    }

    // Start is called before the first frame update
    void Start()
    {
        anima = transform.GetComponent<Animator>();
        cc = transform.GetComponent<CharacterController>();


        #region FSM_Initialize
        moveState = new MoveState(this, 0);
        jumpState = new JumpState(this, 1);
        fallState = new FallState(this, 2);
        landState = new LandState(this, 3);
        attackState = new AttackState(this, 4);
        hitState = new HitState(this, 5);
        dodgeState = new DodgeState(this, 6);
        deathState = new DeathState(this, 7);
        healState = new HealState(this, 8);
        #endregion
        if (Object.HasInputAuthority)
        {
            stateMachine.ChangeState(moveState);
        }
    }
    public void ChangeState(PlayerState state)
    {
        if (Object.HasInputAuthority)
        {
            stateMachine.ChangeState(state);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Object.HasInputAuthority)
        {
            stateMachine.Update();
        }
    }
    void FixedUpdate()
    {
        if (Object.HasInputAuthority)
        {
            stateMachine.FixedUpdate();
        }
    }
    private void LateUpdate()
    {
        Vector3 tmp = cc.velocity;
        tmp = tmp.normalized;
        SetFloat("InputX", tmp.x);
        SetFloat("InputZ", tmp.z);

    }

    public bool IsGround()
    {
        //발밑체크
        return Physics.Raycast(transform.position, Vector3.down, GroundCheckDis, groundLayer);
    }
    #region Animator
    public void SetInt(string _parameters, int _num) => anima.SetInteger(_parameters, _num);
    public void SetFloat(string _parameters, float value) => anima.SetFloat(_parameters, value);
    public void ZeroHorizontal() => anima.SetFloat("Horizontal", 0f);
    public void AnimaPlay(string _name) => anima.Play(_name);
    public void SetState(int _num)
    {
        state = _num;
        SetInt("State", state);
    }
    public void SetState2(int _num)
    {
        state2 = _num;
        SetInt("State2", state2);
    }

    static void OnSetState2Changed(Changed<PlayerStateHandler> changed)
    {
        changed.Behaviour.SetState2(changed.Behaviour.state2);
    }
    static void OnSetStateChanged(Changed<PlayerStateHandler> changed)
    {
        changed.Behaviour.SetState(changed.Behaviour.state);
    }
    public void StateChange(EntityState _newState)
    {
        stateMachine.ChangeState(_newState);
    }
    #endregion
}
