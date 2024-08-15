using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

    }


    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):

            break;
            case (PerformAction.TAKEACTION):

            break;
            case (PerformAction.PERFORMACTION):

            break;
        }
    }

    public void CollectActions(HandleTurns input)
    {
        performList.Add(input);
    }
}
