using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private float maxCooldown = 5f;
    private Image ProgressBar;
    public GameObject Selector;
    //IeNumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10f;
    //dead
    private bool alive = true;
    //heropanel
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    void Start()
    {
        //find spacer 
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        //create panel fill in info
        CreateHeroPanel();
        
        startPosition = transform.position;
        //currentCooldown = Random.Range(0, 2.5f); //probs dont want this (luck stat)
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
                if (!alive)
                {
                    return;
                }
                else
                {
                    // change tag
                    this.gameObject.tag = "DeadHero";
                    // not attackable
                    BSM.HerosInBattle.Remove(this.gameObject);
                    //not managable
                    BSM.HeroesToManage.Remove(this.gameObject);
                    //deactivate selector
                    Selector.SetActive(false);
                    //reset gui
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //remove from list
                    for (int i = 0; 1 < BSM.performList.Count; i++)
                    {
                        if(BSM.performList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.performList.Remove(BSM.performList[i]);
                        }
                    }
                    //change color or something (dead animation)
                    this.gameObject.GetComponent<Renderer>().material.color = Color.grey;
                    //reset input
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    //BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
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
        if(BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
            //reset enemy state
            currentCooldown = 0f;
            currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.WAITING;
        }
        
        //end coroutine
        actionStarted = false;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        hero.curHP -= getDamageAmount;

        if(hero.curHP <= 0)
        {
            hero.curHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }
    //do damage
    void DoDamage()
    {
        float calc_damage = Mathf.Round(hero.curATK + Mathf.Pow(hero.rarityValue, 1/3f)*Mathf.Pow(hero.lvl, 1.5f) + BSM.performList[0].chosenAttack.attackDamage);
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
        Debug.Log(this.gameObject.name + " did " + calc_damage + " damage to " + EnemyToAttack.name);
    }

    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.theName;
        stats.HeroHP.text = "HP: " + hero.curHP + "/" + hero.baseHP;
        stats.HeroMOM.text = "MOM: " + hero.curMOM + "/" + hero.baseMOM;

        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }

    void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: " + hero.curHP + "/" + hero.baseHP;
        stats.HeroMOM.text = "MOM: " + hero.curMOM + "/" + hero.baseMOM;
    }
}
