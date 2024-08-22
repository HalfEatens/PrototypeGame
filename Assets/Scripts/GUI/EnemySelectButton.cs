using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(EnemyPrefab); //save input of enemy prefab
        
    }

    public void HideSelector()
    {
            EnemyPrefab.transform.Find("Selector").gameObject.SetActive(false);

    }
    
    public void ShowSelector()
    {
            EnemyPrefab.transform.Find("Selector").gameObject.SetActive(true);

    }
}
