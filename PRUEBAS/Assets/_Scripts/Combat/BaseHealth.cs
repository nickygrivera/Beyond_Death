using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHealth : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float damageCooldown;
    
    protected float _currentHealth;

    protected bool _canTakeDamage;

    protected virtual void Start()
    {
        _currentHealth = maxHealth;
        _canTakeDamage = true;
    }

    public virtual void TakeDamage(float damage)
    {
        if (_currentHealth <= 0 || !_canTakeDamage) return;
        
        _currentHealth -= damage;
        Debug.Log($"Health: {_currentHealth} / {maxHealth}");
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        _canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        _canTakeDamage = true;
    }

    protected abstract void Die();
}
