using System;
using UnityEngine;

[Serializable]
public class PlainCardData
{

    public int _health;
    public int _attackDamage;
    public int _cost;
    public bool isDead = false;
    public PlainCardData(int health, int damage, int cost)
    {
        _health = health;
        _attackDamage = damage;
        _cost = cost;
    }

}
