using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockUpAbility : BaseAttack
{
    public KnockUpAbility()
    {
        attackName = "Knock Up";
        attackDescription = "Knock up an enemy";
        attackDamage = 100f;
        attackCost = 15f;
    }
}
