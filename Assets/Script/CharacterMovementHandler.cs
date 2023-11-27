using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.EventSystems;
using System;

public class CharacterMovementHandler : NetworkBehaviour
{
    public CharacterInputhandler characterInputhandler;
    //��ǲ ����
    PlayerStateHandler playerStateHandler;

    PlayerInput input;
    Vector3 moveDirection;

    public GameObject playerWeaponHandle;
    public GameObject playerEquipWeapon;
    public GameObject[] playerWeaponPrefab;
    [Networked(OnChanged = nameof(ChangeWeaponNum))]
    int weaponNum { get; set; }

    //InputAction moveAction;

    //[Networked(OnChanged = nameof(OnFireNumChanged))]
    //�� ��Ʈ��ũ Ŭ������ 
    //public NetworkedAttribute()
    //{
    //    OnChangedTargets = OnChangedTargets.All;
    //}
    //�� ���� �������ִ°� 
    //������Ƽ ����� ���� ������ 
    //int fireNum { get; set; }

    //TextMeshPro textMeshPro;


    public bool isRespawnRequsted = false;

    Camera localCamera;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    HPHandler hpHandler;

    public int jumpcountHas { get; set; }

    float jumpTime = 0f;

    float jumpCooldown = 0.15f;



    //public int playerstate { get; set; }

    public int playerjumpcount { get; set; }

    //public int jumpcount2 = 0;
    void Awake()
    {

        characterInputhandler = GetComponent<CharacterInputhandler>();
        hpHandler = GetComponent<HPHandler>();
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
        playerStateHandler = GetComponent<PlayerStateHandler>();

    }
    // Start is called before the first frame update
    void Start()
    {

        ChangeWeapon(0);
        //textMeshPro = GetComponentInChildren<TextMeshPro>();

    }
    static void ChangeWeaponNum(Changed<CharacterMovementHandler> changed)
    {
        int newWeaponNum = changed.Behaviour.weaponNum;
        changed.LoadOld();
        int oldWeaponNum = changed.Behaviour.weaponNum;
        if (newWeaponNum != oldWeaponNum)
        {
            changed.Behaviour.ChangeWeapon(changed.Behaviour.weaponNum);

        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_WeaponNum(int _weaponNum, RpcInfo info = default)
    {
        if (playerEquipWeapon == null)
            return;
        if (playerEquipWeapon.transform.localPosition != Vector3.zero)
        {
            playerEquipWeapon.transform.localPosition = Vector3.zero;
        }
        weaponNum = _weaponNum;
    }
    void ChangeWeapon(int num)
    {
        if (playerEquipWeapon != null)
            Destroy(playerEquipWeapon);
        weaponNum = num;

        if (Object.HasInputAuthority)
        {
            RPC_WeaponNum(weaponNum);
        }

        if (playerWeaponPrefab.Length > num && playerWeaponPrefab[num] != null)
        {
            playerEquipWeapon = Instantiate(playerWeaponPrefab[num], Vector3.zero, Quaternion.identity);

            playerEquipWeapon.transform.parent = playerWeaponHandle.transform;
            playerEquipWeapon.transform.localPosition = Vector3.zero;

        }

    }
    private void Update()
    {

    }
    //���� ī��Ʈ �� 0�� �Ǵ��� ���ذ� �Ȱ� 
    //static void jumpcountSet(Changed<CharacterMovementHandler> changed)
    //{
    //    int newC = changed.Behaviour.jumpcountHas;
    //    changed.LoadOld();
    //    int oldC = changed.Behaviour.jumpcountHas;
    //    if (newC != oldC)
    //    {
    //        //Debug.Log($"newC {newC},  oldC{oldC}");
    //        changed.Behaviour.setJumpcount(newC);
    //    }
    //}
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetJumpCount(int _jumpcount, RpcInfo info = default)
    {
        Debug.Log($"jumpcount {_jumpcount} ");
        jumpcountHas = _jumpcount;
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RPC_SetState(int _State, RpcInfo info = default)
    //{
    //    //Debug.Log($"_State {_State} ");
    //    playerstate = _State;
    //}
    //public void setJumpcount(int num)
    //{
    //    jumpcountHas = num;
    //}

    private void FixedUpdate()
    {

    }

    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        if (input != null)
        {
            moveDirection = new Vector3(dir.x, 0f, dir.y);

            Debug.Log("New INput System Value = " + moveDirection);

        }

    }

    //�׳� update �� ���÷θ� ������ 
    //�������� �����Ϸ��� fixedUpdateNetwork
    public override void FixedUpdateNetwork()
    {
        //if (Object.HasInputAuthority)
        //{
        //    playerStateHandler.StateChageUpdate();
        //}

        if (HasStateAuthority)
        {

            //��ȣ�ۿ� ������ ������
            if (isRespawnRequsted)
            {
                //�������� Ʈ���̸� ���������Ѷ�
                Respawn();
                return;
            }
            //���� ���¸� ���� ������ ���� ���� 
            if (hpHandler.isDead)
                return;
        }



        //Don't update the clients position when they ard dead


        //�÷��̾� �̵� 
        //get the input form the network

        //InputHandler ���� ������ ���� ���⼭ �޴°����� ���� 

        //�� ��Ʈ��ũ���� ���� ������ ������� ������ �ϴ� �κ� 
        //"���� �ùķ��̼� ƽ�� ���� ��ȿ�� Fusion.INetworkInput�� ã�� �� �ִٸ� true�� ��ȯ�մϴ�.
        //��ȯ�� �Է� ����ü�� Fusion.NetworkObject.InputAuthority���� �����ϸ�,
        // ��ȿ�� ��� ���� �ùķ��̼� ƽ�� ���� �ش� Fusion.PlayerRef�� ������ �Է��� �����մϴ�
        //GetInput >> NetworkBehavior �� �ִ� �Լ�
        if (Object.HasInputAuthority)
        {
            playerStateHandler.StateChageUpdate();
        }

        if (GetInput(out NetworkInputData networkInputData))
        {
            //playerstate = playerStateHandler.state;
            //RPC_SetState(playerstate);
            //Debug.Log($" has ���ñ�{transform.name} = " + networkCharacterControllerPrototypeCustom.Object.HasInputAuthority);w

            //Rotate the transform according to the client aim vector
            transform.forward = networkInputData.aimFowardVector;
            //cancel out rotation on X axis as we don't want our chracter to tilt
            //transform >> ������ ĳ���� ����


            //x�� ȸ���� �����ϴ� ��� rigidbody �����̶� ���ٰ� ����������

            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;


            //move input�� �޾ƿ°ɷ� ���

            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            playerStateHandler.SetInputVec(networkInputData.movementInput);
            //Debug.Log($"networkInputData.movementInput = {networkInputData.movementInput}");
            if (playerjumpcount != 0)
                Debug.Log($"JumpCount = {playerjumpcount}");

            //Debug.Log("moveDirection = " + moveDirection);
            //Jump 

            //if (networkInputData.isFireButtonPressed || playerstate == 4)
            //{
            //    if (Object.HasInputAuthority)
            //    {
            //        playerStateHandler.isFireButtonPressed = true;
            //    }

            //    if(playerstate < 3 && playerstate >=0)
            //    {
            //        playerStateHandler.nextState = playerStateHandler.attackState;
            //        playerStateHandler.StateChageUpdate();
            //    }
            //    moveDirection.x = 0f;
            //    moveDirection.z = 0f;

            //    networkCharacterControllerPrototypeCustom.Move(moveDirection);
            //    return;

            //}

            networkCharacterControllerPrototypeCustom.Move(moveDirection);
            if (networkInputData.isJumpButtonPressed)
            {
                bool jumpcount = false;

                if (Object.HasInputAuthority)
                {
                    if (jumpTime + jumpCooldown < Time.time)
                    {
                        if (playerStateHandler.state2 < 2)
                        {
                            jumpTime = Time.time;

                            playerStateHandler.isJumpButtonPressed = true;
                            playerStateHandler.StateChageUpdate();


                        }
                        if (playerStateHandler.state == 1)
                        {
                            playerStateHandler.state2 += 1;
                            playerStateHandler.SetState2(playerStateHandler.state2);
                        }

                    }
                }

                if (playerStateHandler.state == 1 && playerStateHandler.state2 <= 2)
                {
                    networkCharacterControllerPrototypeCustom.Jump();
                    Debug.Log($"JUMP���� ");
                }


            }
            //else if (playerStateHandler.state == 0)
            //{

            //    if (playerStateHandler.IsGround())
            //    {
            //        //Debug.Log("JUmpCountHas �ʱ�ȭ ");
            //        jumpcountHas = 0;
            //        RPC_SetJumpCount(jumpcountHas);
            //    }

            //}
            //if (Object.HasInputAuthority)
            //    characterInputhandler.isJumpButtonPressed = false;


            //if (networkInputData.isFireButtonPressed)
            //{
            //    ++fireNum;
            //    networkInputData.fireNum = fireNum;
            //}
            CheckFallRespawn();

            //�̻��ϰ� �������� ���������� ������




        }

    }
    //public void RPCJumpCount(int num)
    //{
    //    jumpcountHas = num;
    //    RPC_SetJumpCount(jumpcountHas);
    //}
    /// <summary>
    /// �̻��ϰ� �������� ���������� ������
    /// </summary>
    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log("CheckFallRespawn() �Լ��� ȣ��Ǿ� ������");

                Respawn();
            }

        }
    }

    /// <summary>
    /// isRespawnRequsted = ture
    /// </summary>

    public void RequestRespawn()
    {
        isRespawnRequsted = true;

    }
    /// <summary>
    /// �����̵� ��
    /// hpHandler.OnRespawned();
    /// isRespawnRequsted = false;
    /// </summary>
    void Respawn()
    {
        SetCharacterControllerEnabled(true);
        networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();

        isRespawnRequsted = false;
    }
    /// <summary>
    /// CharacterController bool������ Ű�� ���� 
    /// </summary>
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }

    //ONchage ������ ����ƽ���� ����ؾ���
    //�� �Լ��� ��� �׻� ����ǰ� �ִ°Ű� �� ��ȭ����  �ٸ��� if���� Ÿ�� �Ʒ� �Լ��� �����Ű�°��� ���� 
    //static void OnFireNumChanged(Changed<CharacterMovementHandler> changed)
    //{

    //    int fireNumCurrent = changed.Behaviour.fireNum;
    //    //Load the old value
    //    changed.LoadOld();
    //    int fireNumOld = changed.Behaviour.fireNum;

    //    if (fireNumCurrent != fireNumOld)
    //        changed.Behaviour.ChangeUItextFireNum(fireNumCurrent);

    //}

    //void ChangeUItextFireNum(int num)
    //{
    //    if (textMeshPro != null)
    //        textMeshPro.text = $"{num}";
    //}

}
