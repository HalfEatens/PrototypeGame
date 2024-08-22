using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockDownAbility : BaseAttack
{
    public KnockDownAbility()
    {
        attackName = "Knock Down";
        attackDescription = "Knock down an enemy.";
        attackDamage = 150f;
        attackCost = 20f;
    }
}
