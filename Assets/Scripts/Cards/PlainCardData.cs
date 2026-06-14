using System;
using UnityEngine;

[Serializable]
public class PlainCardData
{

    public int _health;
    public int _damage;
    public bool isDead;
    public PlainCardData(int health, int damage)
    {
        this._health = health;
        this._damage = damage;
    }

}
