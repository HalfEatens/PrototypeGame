using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{

    private BattleStateMachine BSM;
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
    public GameObject Selector;
    //IeNumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10f;

    void Start()
    {
        startPosition = transform.position;
        currentCooldown = Random.Range(0, 2.5f); //probs dont want this (luck stat)
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
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
                BSM.HeroesToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
            break;
            case (TurnState.WAITING):
                //idle
            break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
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

 private IEnumerator TimeForAction()
    {
        if(actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate enemy near hero to attack
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x + 1.5f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while(MoveTowardsEnemy(enemyPosition))
        {
            yield return null;
        }

        //wait
        yield return new WaitForSeconds(0.5f);

        //do damage

        //animate back to start position
        Vector3 firstPosition = startPosition;
        while(MoveTowardsStart(firstPosition))
        {
            yield return null;
        }

        //remove this performer from list in BSM
        BSM.performList.RemoveAt(0);

        //reset BSM -> wait
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        //end coroutine
        actionStarted = false;

        //reset enemy state
        currentCooldown = 0f;
        currentState = TurnState.PROCESSING;

    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}
