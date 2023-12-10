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
    public List<GameObject> playerWeaponPrefab;
    TestWeapon testWeapon;


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

    //NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    public Rigidbody rb;
    public NetworkRigidbody networkRigidbody;
    HPHandler hpHandler;

    float moveSpeed = 10f;


    float jumpTime = 0f;

    float jumpCooldown = 0f;

    float jumpForce = 10f;

    bool doJump = false;

    

    public float rotationSpeed = 15.0f;
    public float viewUpDownRotationSpeed = 50.0f;


    //public int playerstate { get; set; }
    //[Networked(OnChanged = nameof(ChangeJumpCount))]
    //private int playerJumpCount11 { get; set; }
    [Networked(OnChanged = nameof(ChangeJump))]
    private int playerJumpCount { get; set; }
    private int playerJumpCount11 { get; set; }

    int jumpCountOld { get; set; }


    public int playerDodgeCount { get; set; }

    public int playerAttackCount { get; set; }

    private NetworkObject networkObject;
    //public int jumpcount2 = 0;
    void Awake()
    {

        characterInputhandler = GetComponent<CharacterInputhandler>();
        hpHandler = GetComponent<HPHandler>();
        networkRigidbody = GetComponent<NetworkRigidbody>();
        rb = GetComponent<Rigidbody>();
        //networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        networkObject = GetComponent<NetworkObject>();

    }
    // Start is called before the first frame update
    void Start()
    {
        playerJumpCount = 0;
        jumpForce = 10f;

        testWeapon = GetComponentInChildren<TestWeapon>();

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
    static void ChangeJump(Changed<CharacterMovementHandler> changed)
    {
        int newWeaponNum = changed.Behaviour.playerJumpCount;
        changed.LoadOld();
        int oldWeaponNum = changed.Behaviour.playerJumpCount;
        Debug.Log($"New {newWeaponNum} / Old {oldWeaponNum}");
        if (newWeaponNum != oldWeaponNum)
        {
            changed.Behaviour.DoJump();
        }
    }
    void DoJump()
    {
        //Debug.Log($"����ī��Ʈ = {playerJumpCount}");
        //if(playerJumpCount!=0)
        RPC_DoJump();
        Debug.Log("DOJUMP���� doJump =  true");
        //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    //static void ChangeJumpCount(Changed<CharacterMovementHandler> changed)
    //{
    //    int newWeaponNum = changed.Behaviour.playerJumpCount11;
    //    changed.LoadOld();
    //    int oldWeaponNum = changed.Behaviour.playerJumpCount11;
    //    if (newWeaponNum!= oldWeaponNum)
    //    {
    //        Debug.Log($"��{oldWeaponNum} �� {newWeaponNum}");
    //    }
    //}
    //public void jumpcountUP()
    //{
    //    playerJumpCount11 += 1;
    //}
    public void JUMPCOUNT()
    {
        playerJumpCount11 += 1;
        Debug.Log($"playerJumpCount11 = {playerJumpCount11}");
        playerJumpCount = playerJumpCount11;
        Debug.Log($"playerJumpCount = {playerJumpCount}");
    }
    public void JUMPCOUNTRESET()
    {
        ;
        //playerJumpCount11 = 0;
        //playerJumpCount = playerJumpCount11;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_DoJump(RpcInfo info = default)
    {
        doJump = true;
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

        if (playerWeaponPrefab.Count > num && playerWeaponPrefab[num] != null)
        {
            playerEquipWeapon = Instantiate(playerWeaponPrefab[num], Vector3.zero, Quaternion.identity);

            playerEquipWeapon.transform.parent = playerWeaponHandle.transform;
            playerEquipWeapon.transform.localPosition = Vector3.zero;

        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("QQQ");
            JUMPCOUNT();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ;
        }
    }


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
            if (testWeapon != null)
            {
                if (testWeapon.IsHit())
                {
                    testWeapon.AttackPlayer();
                }
            }
            else
            {
                testWeapon = GetComponentInChildren<TestWeapon>();
                Debug.Log("testWeapon is Null");
            }
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
            //if (playerjumpcount != 0)
            //    Debug.Log($"����ī��Ʈ  = {playerjumpcount}");

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
            //networkRigidbody.WriteVelocity(moveDirection);
            networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, networkRigidbody.ReadVelocity().y, moveDirection.z * moveSpeed));
            //networkRigidbody.WriteVelocity(networkRigidbody.ReadVelocity());

            //networkCharacterControllerPrototypeCustom.Move(moveDirection);
            //���� 

            if (networkInputData.isJumpButtonPressed)
            {
                // if (playerStateHandler.state2 < 2)
                //if (playerJumpCount < 2)
                //{
                //Debug.Log($"playerJumpCount = {playerJumpCount}");
                //int nowJumpCount = playerJumpCount;
                if (Object.HasInputAuthority)
                {
                    Debug.Log($"playerJumpCount  Network V = {playerJumpCount}");

                    playerStateHandler.isJumpButtonPressed = true;
                    //playerStateHandler.StateChageUpdate();
                    //playerStateHandler.SetState2(1);
                    //playerStateHandler.SetState2(playerJumpCount);
                }

                //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                //}

            }
            else
            {

                playerStateHandler.isJumpButtonPressed = false;
            }

            if (doJump)
            {
                //���� �ѹ��� �ʴ°� ó���ϴ°� ���� ��Ʈ��ũƽ���� �з��� �׷��� �̰͵� ���� �� ����
                jumpTime = Time.time;
                Debug.Log("������");
                //rb.AddForceAtPosition(Vector3.up * jumpForce, transform.position, ForceMode.Impulse);

                //rb.AddForce(Vector3.up * jumpForce*300f);
                if (networkRigidbody.ReadVelocity().y < 0.5f) //���ϸ� Ŭ���̾�Ʈ ��ü���� �����ؼ� �̻�����
                    networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                doJump = false;
            }

            //���� 
            if (networkInputData.isFireButtonPressed)
            {

                if (Object.HasInputAuthority)
                {
                    //attackTime = Time.time;

                    playerStateHandler.isFireButtonPressed = true;

                    //Debug.Log($"���ù�ư ON ");
                }
            }
            //else
            //{
            //    playerStateHandler.isFireButtonPressed = false;

            //}

            //ȸ�� 
            if (networkInputData.isDodgeButtonPressed)
            {
                if (Object.HasInputAuthority)
                {
                    playerStateHandler.isDodgeButtonPressed = true;
                    //playerStateHandler.StateChageUpdate();
                    ++playerDodgeCount;
                }
            }
            else
            {
                playerStateHandler.isDodgeButtonPressed = false;

            }
            if (playerStateHandler.IsGround() && playerStateHandler.state == 0)
            {
                if (Object.HasInputAuthority && rb.velocity.y < 0.2f)
                {
                    //playerJumpCount = 0;
                    ;
                }

                playerDodgeCount = 0;

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
        //networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());
        networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();

        isRespawnRequsted = false;
    }
    /// <summary>
    /// CharacterController bool������ Ű�� ���� 
    /// </summary>
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        //networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
        networkRigidbody.enabled = isEnabled;
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
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log($"{other}");
    //}
}
