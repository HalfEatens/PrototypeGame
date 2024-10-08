using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    //for the progress bar
    private float currentCooldown = 0f;
    private float maxCooldown = 5f;
    //this gameobject
    private Vector3 startPosition;
    public GameObject Selector;
    //time for action stuffs
    private bool actionStarted = false;
    public GameObject HeroToAttack;
    private float animSpeed = 10f;

    //alive
    private bool alive = true;


    void Start()
    {
        currentState = TurnState.PROCESSING;
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }

    void Update()
    {
        //Debug.Log(currentState);
        switch(currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
            break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                currentState = TurnState.WAITING;
            break;
            case (TurnState.WAITING):
                //idle state
            break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
            break;
            case (TurnState.DEAD):
                if(!alive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadEnemy";
                    //not attackable
                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    //disable selector
                    Selector.SetActive(false);
                    //remove inputs
                    for(int i = 0; i < BSM.performList.Count; i++)
                    {
                        if(BSM.performList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.performList.Remove(BSM.performList[i]);
                        }
                    }
                    //change color (animate later)
                    this.gameObject.GetComponent<Renderer>().material.color = Color.grey;
                    alive = false;
                    //reset enemy buttons
                    BSM.EnemyButtons();
                    //check alive
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }

            break;
        }
    }

        void UpgradeProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime;

        if(currentCooldown >= maxCooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        HandleTurns myAttack = new HandleTurns();
        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];
        
        int num = Random.Range(0, enemy.attacks.Count);
        myAttack.chosenAttack = enemy.attacks[num];

        //Debug.Log(this.gameObject.name + " has chosen " + myAttack.chosenAttack.attackName + " and deals " + myAttack.chosenAttack.attackDamage + " damage");

        BSM.CollectActions(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        if(actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate enemy near hero to attack
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x - 1.5f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
        while(MoveTowardsEnemy(heroPosition))
        {
            yield return null;
        }

        //wait
        yield return new WaitForSeconds(0.5f);

        //do damage
        DoDamage();
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

    void DoDamage()
    {
        float calc_damage = Mathf.Round(enemy.curATK + Mathf.Pow(enemy.rarityValue, 1/3f)*Mathf.Pow(enemy.lvl, 1.5f) + BSM.performList[0].chosenAttack.attackDamage);
        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
        Debug.Log(this.gameObject.name + " did " + calc_damage + " damage to " + HeroToAttack.name);
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.curHP -= getDamageAmount;
        if(enemy.curHP <= 0)
        {
            currentState = TurnState.DEAD;
        }
    }
}
