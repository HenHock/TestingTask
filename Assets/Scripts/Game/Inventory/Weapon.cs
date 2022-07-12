using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponsType
{
    Axe,
    Sword,
    GreatSword,
}

public class Weapon : Item
{
    [SerializeField] private int _damage;
    public int damage => _damage;

    [SerializeField] private WeaponsType weaponsType;
    
    public WeaponsType GetWeaponType()
    {
        return weaponsType;
    }
}
