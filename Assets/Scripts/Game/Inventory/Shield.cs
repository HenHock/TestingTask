using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Item
{
    [SerializeField, Range(0f,1f)] private float _damageAbsorption; // How much damage shield ignore in the procent from 0 to 1
    public float damageAbsorption => _damageAbsorption;
}
