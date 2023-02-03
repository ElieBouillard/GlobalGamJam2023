using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class EnemyAttackAnimation : MonoBehaviour
{
    private EnemyIdentity _enemyIdentity;

    private void Awake()
    {
        _enemyIdentity = GetComponentInParent<EnemyIdentity>();
    }

    private void Attack()
    {
        _enemyIdentity.Attack();
    }

    private void EndAttack()
    {
        _enemyIdentity.EndAttack();
    }
}

