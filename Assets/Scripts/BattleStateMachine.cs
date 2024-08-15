using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }



    public PerformAction battleStates;

    public List<HandleTurns> performList = new List<HandleTurns>();
    public List<GameObject> HerosInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public HeroGUI HeroInput;

    public List<GameObject> HeroesToManage = new List<GameObject>();
    private HandleTurns HeroChoice;
    public GameObject enemyButton;
    public Transform Spacer;


    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        EnemyButtons();
    }


    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):
                if(performList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
            break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].Attacker);
                if(performList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.HeroToAttack = performList[0].AttackersTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                }

                if(performList[0].Type == "Enemy")
                {
                    
                }
                battleStates = PerformAction.PERFORMACTION;
            break;
            case (PerformAction.PERFORMACTION):

            break;
        }
    }

    public void CollectActions(HandleTurns input)
    {
        performList.Add(input);
    }

    void EnemyButtons()
    {
        foreach(GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = currentEnemy.enemy.name;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer, false);
        }
    }
}
