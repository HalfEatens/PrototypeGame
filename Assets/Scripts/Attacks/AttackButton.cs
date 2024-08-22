using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack abilityAttackToPerform;

    public void CastAbilityAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input4(abilityAttackToPerform);
    }
}
