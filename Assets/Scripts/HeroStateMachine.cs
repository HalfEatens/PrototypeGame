using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{

    public BaseHero hero;

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
    private float maxCooldown = 2f;
    public Image ProgressBar;


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
        float calcCooldown = currentCooldown / maxCooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);

        if(currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }
}
