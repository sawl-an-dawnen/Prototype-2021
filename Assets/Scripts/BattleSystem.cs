using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Rune;

public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public enum CombatOptions//rename
{
    Slam = 20,
    Firebolt = 25,
    Electrocute = 4,
    Icefreeze= 5
}

public class TurnActions {
    public CombatOptions action;
    public float waitTime;
    public Func<GameObject> actionFunc;
    public TurnActions(CombatOptions co, float f, Func<GameObject> a)
    {
        action = co;
        waitTime = f;
        actionFunc = a;
    }
}

public class BattleSystem : MonoBehaviour
{
    public float dialogWaitTime = 2f;

    GameObject player;
    PlayerHealthBar playerHP;
    RigidbodyConstraints playerRBConstraints;
    GameObject enemy;
    PlayerHealthBar enemyHP;

    public GameObject fireboltAsset;
    public GameObject lightningAsset;

    TextMeshProUGUI battleDialog;

    BattleState state;
    public bool isPlayerFirstTurn;
    private readonly List<Func<string>> playerTurnBeginListeners = new();
    private readonly List<Func<string>> playerTurnEndListeners = new();

    bool playerDodged = false;

    Animator animator;
    readonly List<TurnActions> turnActions = new ();
    public ResourceHandler resourceHandler = new();
    public Spell[] spells = new Spell[5];
    void Start()
    {
        animator = GetComponent<Animator>();
        state = BattleState.START;
        player = GameObject.FindWithTag("Player");
        enemy = GameObject.FindWithTag("Enemy");
        playerHP = player.GetComponent<PlayerHealthBar>();
        playerHP.TakeDamage(100 - GameManager.Instance.playerHealth);
        enemyHP = enemy.GetComponent<PlayerHealthBar>();
        battleDialog = GameObject.FindWithTag("BattleDialog").GetComponent<TextMeshProUGUI>();

        //freeze rotation/position
        playerRBConstraints = player.GetComponentInChildren<Rigidbody>().constraints;
        player.GetComponentInChildren<Rigidbody>().constraints = (RigidbodyConstraints)122;//freeze position xz, rotation
        enemy.GetComponentInChildren<Rigidbody>().constraints = (RigidbodyConstraints)122;

        SetupSpells();

        StartCoroutine(SetupBattle());
    }
    
    public void SetupSpells()
    {
        var slamCost = new int[4];
        slamCost[(int)FIRE] = 1;
        slamCost[(int)EARTH] = 1;
        spells[0] = new Spell {
            name = "Slam",
            effect = () =>
            {
                OnSlamButton();
                return "Pressed spell 1!";
            },
            cost = slamCost
        };
        
        var fireBallCost = new int[4];
        fireBallCost[(int)FIRE] = 2;
        fireBallCost[(int)AIR] = 1;
        spells[1] = new Spell {
            name = "Fire Ball",
            effect = () =>
            {
                OnFireButton();
                return "Pressed spell 2!";
            },
            cost = fireBallCost
        };

        var dodgeCost = new int[4];
        dodgeCost[(int)WATER] = 2;
        dodgeCost[(int)AIR] = 1;
        spells[2] = new Spell {
            name = "Dodge",
            effect = () =>
            {
                OnDodgeButton();
                return "Pressed spell 3!";
            },
            cost = dodgeCost
        };
        
        var healCost = new int[4];
        healCost[(int)WATER] = 3;
        spells[3] = new Spell {
            name = "Electrocute",
            effect = () =>
            {

        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.Electrocute, 1f, sendLightning));
        }
                return "Pressed spell 4!";
            },
            cost = healCost
        };
        
        var lightningCost = new int[4];
        lightningCost[(int)FIRE] = 3;
        spells[4] = new Spell {
            name = "Electrocute",
            effect = () =>
            {
                return "Pressed spell 5!";
            },
            cost = lightningCost
        };
    }

    IEnumerator SetupBattle()
    {
        battleDialog.text = "Fighting the enemy!";

        yield return new WaitForSeconds(dialogWaitTime);

        if (isPlayerFirstTurn)
            PlayerTurn();
        else
            EnemyTurn();
    }

    void PlayerTurn()
    {
        battleDialog.text = "What will you do?";
        state = BattleState.PLAYER_TURN;
        foreach (var fun in playerTurnBeginListeners)
        {
            fun();
        }
    }

    IEnumerator ProcessTurn()
    {
        foreach (var fun in playerTurnEndListeners)
        {
            fun();
        }

        foreach (var action in turnActions)
        {
            //todo: maybe add code for enemy to randomly be able to dodge?
            battleDialog.text = "The enemy takes " + action.action.ToString();
            var gameObj = action.actionFunc();
            yield return new WaitForSeconds(action.waitTime);
            int enemyNewHP = enemyHP.TakeDamage((int)action.action);
            GameObject.Destroy(gameObj);
            yield return new WaitForSeconds(dialogWaitTime);
            if (enemyNewHP == 0)
            {
                state = BattleState.WON;
                break;
            }

        }
        turnActions.Clear();

        if (enemyHP.currentHealth > 0)
        {
            state = BattleState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
        else
            EndBattle();
    }

    IEnumerator EnemyTurn()
    {
        battleDialog.text = "The enemy attacks!";
        const int enemySlamDamage = 15; //todo: add other attack abilities
        if (!playerDodged)
        {
            int playerNewHP = playerHP.TakeDamage(enemySlamDamage);
            battleDialog.text = "The enemy slammed you!";
            animator.SetTrigger("PlayerSlam");
        }
        else
        {
            battleDialog.text = "You dodged, the enemy's slam attack failed";
        }

        playerDodged = false;

        yield return new WaitForSeconds(dialogWaitTime);

        if (playerHP.currentHealth <= 0)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        player.GetComponentInChildren<Rigidbody>().constraints = playerRBConstraints;//restore ability to move/rotate
        if (state == BattleState.WON)
        {
            battleDialog.text = "You have prevailed!";
            //move on w/ quest
        }
        else
        {
            battleDialog.text = "You were vanquished!";
            //move back to checkpoint
        }
    }

    GameObject sendFirebolt()//use for enemy as well
    {
        var currentPrefabObject = GameObject.Instantiate(fireboltAsset);
        currentPrefabObject.transform.position = player.transform.position + Vector3.right;
        currentPrefabObject.transform.rotation = new Quaternion(0, 0.70711f, 0, 0.70711f);//from player to enemy, might need change for backward

        return currentPrefabObject;
    }
    GameObject sendSlam()//use for enemy as well?
    {
        animator.SetTrigger("EnemySlam");

        return null;
    }
    GameObject sendLightning()//use for enemy as well?
    {
        var lightningObj = GameObject.Instantiate(lightningAsset);
        var lightningComp = lightningObj.GetComponent<LightningBoltScript>();
        lightningComp.StartObject = player;
        lightningComp.EndObject = enemy;
        lightningComp.Generations = 3;

        return lightningObj;
    }

    void Update()
    {
    }


    /// <summary>
    /// combat buttons handlers
    /// </summary>
    public void OnSlamButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new (CombatOptions.Slam, 1, sendSlam)) ;
        }
    }

    public void OnFireButton()//todo: remove listener on mouse click
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new (CombatOptions.Firebolt, .1f, sendFirebolt));
        }
    }

    public void OnDodgeButton()//todo: remove listener on mouse click, need this func?
    {
        if (state == BattleState.PLAYER_TURN)
        {
            playerDodged = true;
        }
    }

    public void OnElectrocuteButton() //for quick debug, not really needed
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.Electrocute, 1f, sendLightning));
        }
    }


    public void OnEndTurnButton()//todo: remove listener on mouse click
    {
        if (state == BattleState.PLAYER_TURN)
        {
            StartCoroutine(ProcessTurn());
        }
    }
    
    public void RegisterPlayerTurnBeginListener(Func<string> fun)
    {
        playerTurnBeginListeners.Add(fun);
    }

    public void RegisterPlayerTurnEndListener(Func<string> fun)
    {
        playerTurnEndListeners.Add(fun);
    }
}
