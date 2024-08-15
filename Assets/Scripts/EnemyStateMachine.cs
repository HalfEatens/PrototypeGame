using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    //for the progress bar
    private float currentCooldown = 0f;
    private float maxCooldown = 5f;

    void Start()
    {
        currentState = TurnState.PROCESSING;
    }

    void Update()
    {
        //Debug.Log(currentState);
        switch(currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
            break;
            case (TurnState.ADDTOLIST):

            break;
            case (TurnState.WAITING):

            break;
            case (TurnState.SELECTING):

            break;
            case (TurnState.ACTION):

            break;
            case (TurnState.DEAD):

            break;
        }
    }

        void UpgradeProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime;

        if(currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }
}
