using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTrigger : NetworkBehaviour
{
    PlayerStateHandler player;
    Animator animator;
    TestWeapon testweapon;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GetComponentInParent<PlayerStateHandler>();
        animator = transform.GetComponent<Animator>();
    }
    void Start()
    {
        testweapon = GetComponentInChildren<TestWeapon>();
        if(testweapon != null)
        {
            testweapon.meshcol.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void AnimationTrigger()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log("Ʈ���ſ��� OFF");
            player.animationTrigger = false;
        }
    }

    void AttackColOn()
    {
        if (Object.HasInputAuthority)
        {
            //�ӽ÷� ���� ���߿� ���� �ڵ鷯�� �ٸ� ���� ��ġ
            testweapon.SetDirect(true);
            //Debug.Log("���ݽõ�");
            if (testweapon != null)
                testweapon.WeaponColOn();
            else
            {
                Debug.Log($"testweapon = Null");
            }
        }
    }
    void AttackColOff()
    {
        if (Object.HasInputAuthority)
        {
            //�ӽ÷� ���� ���߿� ���� �ڵ鷯�� �ٸ� ���� ��ġ
            testweapon.SetDirect(true);
            //Debug.Log("���ݽõ�");
            if (testweapon != null)
                testweapon.WeaponColOff();
            else
            {
                Debug.Log($"testweapon = Null");
            }
        }
    }
}
