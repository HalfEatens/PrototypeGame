using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.VersionControl;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
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


    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;
    public GameObject AbilityPanel;

    //attacks hero
    public Transform ActionSpacer;
    public Transform AbilitySpacer;
    public GameObject ActionButton;
    public GameObject AbilityButton;
    private List<GameObject> atkBtns = new List<GameObject>();

    //enemy buttons
    private List<GameObject> enemyBtns = new List<GameObject>();

    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        HeroInput = HeroGUI.ACTIVATE;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        AbilityPanel.SetActive(false);

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
                        for(int i = 0; i < HerosInBattle.Count; i++)
                        {
                            if(performList[0].AttackersTarget == HerosInBattle[i])
                            {
                                ESM.HeroToAttack = performList[0].AttackersTarget;
                                ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                                break;
                            }
                            else
                            {
                                performList[0].AttackersTarget = HerosInBattle[Random.Range(0,HerosInBattle.Count)];
                                ESM.HeroToAttack = performList[0].AttackersTarget;
                                ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                            }
                        }

                }

                if(performList[0].Type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.EnemyToAttack = performList[0].AttackersTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFORMACTION;
            break;
            case (PerformAction.PERFORMACTION):
                //idle
            break;
            case (PerformAction.CHECKALIVE):
                if(HerosInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                    //lose game
                }
                else if(EnemiesInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                    //win game
                }
                else
                {
                    //call func
                    clearAttackPanel();
                    HeroInput = HeroGUI.ACTIVATE;
                    //battleStates = PerformAction.WAIT;
                }
            break;
            case (PerformAction.LOSE):
                Debug.Log("lose");
            break;
            case (PerformAction.WIN):
                Debug.Log("win");
                for(int i = 0; i < HerosInBattle.Count; i++)
                {
                    HerosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                }
            break;
        }

        switch (HeroInput)
        {
            case(HeroGUI.ACTIVATE):
                if(HeroesToManage.Count > 0)
                {
                    HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    HeroChoice = new HandleTurns();

                    AttackPanel.SetActive(true);
                        
                        CreateAttackButtons();

                    HeroInput = HeroGUI.WAITING;
                }
            break;

            case(HeroGUI.WAITING):
                //idle
            break;

            //case(HeroGUI.INPUT1):

            //break;

            //case(HeroGUI.INPUT2):

            //break;

            case(HeroGUI.DONE):
                HeroInputDone();
            break;
        }
    }

    public void CollectActions(HandleTurns input)
    {
        performList.Add(input);
    }

    public void EnemyButtons()
    {
        //cleanup
        foreach(GameObject enemyBtn in enemyBtns)
        {
            Destroy(enemyBtn);
        }
        enemyBtns.Clear();
        //create buttons
        foreach(GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = currentEnemy.enemy.theName;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer, false);
            enemyBtns.Add(newButton);
        }
    }

    public void Input1() //attack button
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";
        HeroChoice.chosenAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0];
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject chosenEnemy) //enemy select button
    {
        HeroChoice.AttackersTarget = chosenEnemy;
        HeroInput = HeroGUI.DONE;
    }

    void HeroInputDone()
    {
        performList.Add(HeroChoice);
        clearAttackPanel();
        //cleanup attackpanel
        
        HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HeroesToManage.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
    }

    void clearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        AttackPanel.SetActive(false);
        AbilityPanel.SetActive(false);

        foreach(GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }

    //create actionbut
    void CreateAttackButtons()
    {
        GameObject AttackButton = Instantiate(ActionButton) as GameObject;
        TMP_Text AttackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        AttackButton.transform.SetParent(ActionSpacer, false);
        atkBtns.Add(AttackButton);

        GameObject AbilityAttackButton = Instantiate(ActionButton) as GameObject;
        TMP_Text AbilityAttackButtonText = AbilityAttackButton.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        AbilityAttackButtonText.text = "Abilities";
        AbilityAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        AbilityAttackButton.transform.SetParent(ActionSpacer, false);
        atkBtns.Add(AbilityAttackButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.AbilityAttacks.Count > 0)
        {
            foreach(BaseAttack abilAtk in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.AbilityAttacks)
            {
                GameObject abilityButton = Instantiate(AbilityButton) as GameObject;
                TMP_Text abilityButtonText = abilityButton.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
                abilityButtonText.text = abilAtk.attackName;
                AttackButton ATB = abilityButton.GetComponent<AttackButton>();
                ATB.abilityAttackToPerform = abilAtk;
                abilityButton.transform.SetParent(AbilitySpacer, false);
                atkBtns.Add(abilityButton);

            }
        }
        else
        {
            AbilityAttackButton.GetComponent<Button>().interactable = false;
        }
    }

    public void Input4(BaseAttack chosenAbility)//chosen ability attack
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";

        HeroChoice.chosenAttack = chosenAbility;
        AbilityPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input3()//switching to ability atk
    {
        AttackPanel.SetActive(false);
        AbilityPanel.SetActive(true);
    }
}
