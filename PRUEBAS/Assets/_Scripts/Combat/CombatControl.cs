using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CombatControl : MonoBehaviour
{
    [SerializeField] private Collider weaponTrigger;

    private Animator _anim;
    private bool _isAttacking = false;

    private readonly int Attack = Animator.StringToHash("Attack");
    private void Start()
    {
        _anim = GetComponent<Animator>();
        weaponTrigger.enabled = false;

        InputManager.Instance.FirePerformed += AttackOnPerformed;
    }

    private void AttackOnPerformed()
    {
        if (_isAttacking) return;
        _anim.CrossFadeInFixedTime(Attack, 0.1f, 1);
        _isAttacking = true;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => _anim.GetCurrentAnimatorStateInfo(1).shortNameHash == Attack);
        _isAttacking = false;
    }

    private void AttackAnimEvent() 
    {
        weaponTrigger.enabled= true;
    }

    private void EndAttackAnimEvent()
    {
        weaponTrigger.enabled = false;
    }
}
