using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attack;

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
    }

    public int GetAttack()
    {
        return attack;
    }
    
    public void SetAttack(int newAttack)
    {
        attack = newAttack;
    }
}