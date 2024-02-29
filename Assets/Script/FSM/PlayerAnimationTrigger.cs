using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if (testweapon != null)
        {
            testweapon.meshcol.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void AnimationTriggerOFF()
    {
        if (HasStateAuthority)
        {
            //Debug.Log("AnimationTrigger OFF");
            player.SetAnimationTrigger(false);
            player.AnimationTrigger = false;
        }

    }
    void AnimationTriggerOn()
    {
        if (HasStateAuthority)
        {
            //Debug.Log("AnimationTrigger On");
            player.SetAnimationTrigger(true);
            player.AnimationTrigger = true;

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
