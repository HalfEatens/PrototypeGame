using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;   //
    public string attackDescription;
    public float attackDamage; //Base damage = round ( lvl^(3/2) * star^(1/3) ) + 100 )
    public float attackCost; //momentum cost


}
