using DigitalRuby.LightningBolt;
using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Rune;
using Cinemachine;

[Serializable]
public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

[Serializable]
public class CombatOptions
{
    public string name;
    public int damage;
	public SpellType Type;

	public CombatOptions(string name, int damage, SpellType type)
    {
        this.name = name;
        this.damage = damage;
		this.Type = type;
	}
    public static CombatOptions Stun { get; } = new CombatOptions("Stun", 6, SpellType.Normal);
    public static CombatOptions Heal { get; } = new CombatOptions("Heal", 15, SpellType.Normal);
    public static CombatOptions Knife { get; } = new CombatOptions("Knife", 9, SpellType.Normal);
    public static CombatOptions Slam { get; } = new CombatOptions("Slam", 14, SpellType.Normal);
    public static CombatOptions Electrocute { get; } = new CombatOptions("Electrocute", 14, SpellType.Normal);
    public static CombatOptions Fireball { get; } = new CombatOptions("Fireball", 16, SpellType.Fire);
    public static CombatOptions FireElement { get; } = new CombatOptions("FireElement", 3, SpellType.Fire);
    public static CombatOptions EarthElement { get; } = new CombatOptions("EarthElement", 3, SpellType.Earth);
    public static CombatOptions WaterElement { get; } = new CombatOptions("WaterElement", 3, SpellType.Water);
    public static CombatOptions ElementalInfluence { get; } = new CombatOptions("ElementalInfluence", 0, SpellType.Special);
    public static CombatOptions Hydrosphere { get; } = new CombatOptions("Hydrosphere", 4, SpellType.Water);
}

public enum SpellType
{
	Normal,
	Special,
	Fire,
	Earth,
	Water,
	Wind
}
public class TurnActions
{
    public CombatOptions action;
    public float waitTime;
    public Func<bool, GameObject> actionFunc;

    public TurnActions(CombatOptions action, float waitTime, Func<bool, GameObject> actionFunc)
    {
        this.action = action;
        this.waitTime = waitTime;
        this.actionFunc = actionFunc;
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
    [SerializeField] AudioSource winSound;
    [SerializeField] AudioSource loseSound;
    [SerializeField] AudioSource slamSound;
    [SerializeField] AudioSource knifeSound;
    [SerializeField] AudioSource electrocuteSound;
    [SerializeField] AudioSource healSound;
    [SerializeField] AudioSource stunSound;
    [SerializeField] AudioSource dodgeSound;
    [SerializeField] AudioSource fireElementSound;
    [SerializeField] AudioSource waterElementSound;
    [SerializeField] AudioSource earthElementSound;



    public GameObject fireboltAsset;
    public GameObject lightningAsset;
    public GameObject knifeAsset;
    public CinemachineVirtualCamera primaryCamera;
    public CinemachineVirtualCamera mcCamera;
    public CinemachineVirtualCamera enemyCamera;

  
    public GameObject enemyStunAsset;
    public GameObject stunObj;
    public GameObject healAsset;
    public GameObject healObj;
    public int remaningStunTurns = 0;


    TextMeshProUGUI battleDialog;

    public BattleState state;
    public bool isPlayerFirstTurn;
    private readonly List<Func<string>> battleStartListeners = new();
    private readonly List<Func<string>> playerTurnBeginListeners = new();
    private readonly List<Func<string>> playerTurnEndListeners = new();

    bool playerDodged = false;

    Animator animator;

    Animator enemyAnimator;
    Animator ghostAnimator;
    WaitForSeconds jumpWait = new WaitForSeconds(5.6f); // use to wait for anim to finish
    WaitForSeconds wait3sec = new WaitForSeconds(3f);
    WaitForSeconds wait2sec = new WaitForSeconds(2f);
    WaitForSeconds wait1sec = new WaitForSeconds(1f);
    GameObject ghostBasic;
    GameObject enemyReference;

    readonly List<TurnActions> turnActions = new ();
    public ResourceHandler resourceHandler = new();
    public GameManager.Spell[] spells = new GameManager.Spell[4];
    private int playerPowerBoost = 2;
    private int DialogueCounter = 0;    //Used for player corresponding dialogue options by enemy type

	private int fireCount;
	private int earthCount;
	private int waterCount;
	private int EleInfluenceDamange;
    private TurnbasedDialogHandler turnbasedDialogHandler;
    private float enemyDifficulty = 1f;

    public void StartCombatRound()
	{
		fireCount = 0;
		earthCount = 0;
		waterCount = 0;
		EleInfluenceDamange = 0;
    }

	private float playerTurnTimer = 11f;
	private bool isTimerStarted = false;
    public bool hasSubmitted = false;
	[SerializeField] TextMeshProUGUI timerText;

	void Update()
	{
		UpdateDifficulty();
		if (state == BattleState.PLAYER_TURN)
		{
			int minutes = Mathf.FloorToInt(playerTurnTimer / 60);
			int seconds = Mathf.FloorToInt(playerTurnTimer % 60);
			if (!isTimerStarted)
			{
				StartPlayerTurnTimer();
				isTimerStarted = true;
			}
			if (playerTurnTimer > 0)
			{
				playerTurnTimer -= Time.deltaTime;
				timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
			}
			else if (!hasSubmitted && playerTurnTimer < 0)
			{
				playerTurnTimer = 0;
				timerText.text = "00:00";
				timerText.color = Color.red;
				SubmitAndEndPlayerTurn();
			}
		}
		else
		{
			isTimerStarted = false;
            timerText.text = "";
		}
	}

    private void SubmitAndEndPlayerTurn()
    {
		turnbasedDialogHandler.SubmitSelected();
	}

    public void StartPlayerTurnTimer()
	{
		timerText.color = Color.white;
		hasSubmitted = false;
		playerTurnTimer = 8f;
	}

	public void StopPlayerTurnTimer()
	{
		playerTurnTimer = 0;
		timerText.text = "";
		hasSubmitted = true;
	}


	void Start()
    {
        float gametime = GameManager.Instance.GetGameTime();
		animator = GetComponent<Animator>();
        state = BattleState.START;
        player = GameObject.FindWithTag("Player");
        enemy = GameObject.FindWithTag("Enemy");
        playerHP = player.GetComponent<PlayerHealthBar>();
        enemyHP = enemy.GetComponentInChildren<PlayerHealthBar>();
        battleDialog = GameObject.FindWithTag("BattleDialog").GetComponent<TextMeshProUGUI>();
		turnbasedDialogHandler = GetComponentInChildren<TurnbasedDialogHandler>();
		//freeze rotation/position
		playerRBConstraints = player.GetComponentInChildren<Rigidbody>().constraints;
        player.GetComponentInChildren<Rigidbody>().constraints = (RigidbodyConstraints)122;//freeze position xz, rotation

        enemyReference = GameObject.FindWithTag("enemyReference");

        enemyReference.GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)122;

        //set enemy difficulty
        int timeslot = (int)(gametime/120);
        float currentDifficulty = timeslot * 0.05f;
        enemyDifficulty = enemyDifficulty + currentDifficulty;
        int enemyhealth;

        // Adjust the enemy health based on enemy type
        if (enemyReference.name.ToLower().Contains("skel"))
        {
            enemyhealth = (int)(45*enemyDifficulty);
        }
        else if (enemyReference.name.ToLower().Contains("eye"))
        {
            enemyhealth = (int)(75*enemyDifficulty);
        }
        else if(enemyReference.name.ToLower().Contains("horse"))
        {
            enemyhealth = (int)(150*enemyDifficulty);      
        }
        else if(enemyReference.name.ToLower().Contains("enemyghost"))
        {
            enemyhealth = (int)(50*enemyDifficulty);      
        }
        else if (enemyReference.name.ToLower().Contains("mushr"))
        {
            enemyhealth = (int)(75*enemyDifficulty);
        }
        else
        {
            enemyhealth = (int)(100*enemyDifficulty);
        }
        enemyHP.healthBar.SetMaxHealth(enemyhealth);
        enemyHP.currentHealth = enemyhealth;

        enemyAnimator = enemyReference.GetComponent<Animator>(); // enemy animation controller
        ghostBasic = GameObject.Find("ghost basic");
        ghostAnimator = ghostBasic.GetComponent<Animator>();

        //enable primary camera and disable other cameras
        primaryCamera.enabled = true;
        mcCamera.enabled = false;
        enemyCamera.enabled = false;

    }

    private void FixedUpdate()
    {
        battleDialog = GameObject.FindWithTag("BattleDialog").GetComponent<TextMeshProUGUI>();
        enemyReference = GameObject.FindWithTag("enemyReference");
        //Debug.Log(enemyReference.name);

        player.GetComponentInChildren<Rigidbody>().constraints = (RigidbodyConstraints)122;//freeze position xz, rotation
        enemyReference.GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)122;
        enemyAnimator = enemyReference.GetComponent<Animator>();
	    ghostAnimator = ghostBasic.GetComponent<Animator>();
    }

    public void Resume()
    {
        GetComponentInChildren<TurnbasedDialogHandler>().Disable();
        StartCoroutine(SetupBattle());
    }

    /** 
     *  Going to need to move all of the spell creation to the GameManager,
     *  or at least some of it. We need to know the spells the player has
     *  when we replace them.
     */
    public void SetupSpells(string[] selectedSpells)
    {
        for (int i = 0; i < 4; i++)
        {
            Action eventFunc = selectedSpells[i] switch
            {
                "Dodge"       => OnDodgeButton,
                "Electrocute" => OnElectrocuteButton,
                "Fireball"    => OnFireButton,
                "Heal"        => OnHealButton,
                "Slam"        => OnSlamButton,
                "Stun"        => OnStunButton,
                "Knife Throw" => OnKnifeButton,
                "Fire Element" => OnFireEleButton,
                "Earth Element" => OnEarthEleButton,
                "Water Element" => OnWaterEleButton,
                "Elemental Influence" => OnElementalButton,
                "Hydrosphere" => OnHydrosphereButton,
                _             => null
            };
            
            if (eventFunc != null)
            {
                GameManager.Instance.RegisterSpellEventFunc(selectedSpells[i], eventFunc);
            }
            else {
                Debug.Log("Error: Could not find spell named " + selectedSpells[i]);
            }
            spells[i] = GameManager.Instance.spells.Find(s => s.name.Equals(selectedSpells[i]));
        }
    }

    IEnumerator SetupBattle()
    {
        battleDialog.text = "Combat Start!";
        
        foreach (var fun in battleStartListeners)
        {
            fun();
        }

        yield return new WaitForSeconds(dialogWaitTime);

        if (isPlayerFirstTurn)
            PlayerTurn();
        else
            EnemyTurn();
    }

    void PlayerTurn()
    {
        battleDialog.color = Color.white;
        battleDialog.text = "";
        state = BattleState.PLAYER_TURN;
        foreach (var fun in playerTurnBeginListeners)
        {
            fun();
        }
    }

    IEnumerator ProcessTurn()
    {
        StartCombatRound();
        // enable player camera and disable other cameras
        primaryCamera.enabled = false;
        mcCamera.enabled = true;
        enemyCamera.enabled = false;

        foreach (var fun in playerTurnEndListeners)
        {
            fun();
        }

        List<TurnActions> elementalInfluences = new List<TurnActions>();
        List<TurnActions> otherActions = new List<TurnActions>();

        foreach (var action in turnActions)
        {
            if (action.action.name == "ElementalInfluence")
            {
                elementalInfluences.Add(action);
            }
            else
            {
                otherActions.Add(action);
            }
        }

        otherActions.AddRange(elementalInfluences);
        turnActions.Clear();
        turnActions.AddRange(otherActions);

        foreach (var action in turnActions)
        {

            if (action.action.name == "Heal")
            {
                selfHeal();
            }
            else
            {
                battleDialog.text = "The enemy takes " + action.action.name + "!";
                var gameObj = action.actionFunc(true);
                yield return new WaitForSeconds(action.waitTime);
                GameObject.Destroy(gameObj);
                yield return SpellEffectByEnemy(action);
            }

            yield return new WaitForSeconds(dialogWaitTime);
            if (enemyHP.currentHealth <= 0)
            {
                state = BattleState.WON;
                break;
            }
        }
		turnActions.Clear();
		if (state == BattleState.WON)
        {
            StartCoroutine(EndBattle());
        }
        else if (remaningStunTurns < 1)
        {
            // enable enemy camera and disable other cameras
            primaryCamera.enabled = false;
            mcCamera.enabled = false;
            enemyCamera.enabled = true;

            state = BattleState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (--remaningStunTurns == 0)
                Destroy(stunObj);
            PlayerTurn();
        }
    }




    //This Function plays goofy dialogues based on the spells used and also updates the enemy health.
    IEnumerator SpellEffectByEnemy(TurnActions action)
    {
        int enemyNewHP;
		int playerAttackPower = GameManager.Instance.GetPlayerAttack();
        int playerAttack = Mathf.CeilToInt(action.action.damage * (1 + playerAttackPower / 100.0f));
        Debug.Log("got" + playerAttack);

		if (action.action.name == "Slam" && enemyReference.name.ToLower().Contains("skel"))
        {
            enemyNewHP = enemyHP.TakeDamage((int)(1.5f * playerPowerBoost/2 * playerAttack), false);
            GameManager.Instance.foundWeakness("Slam");
            switch (DialogueCounter)
            {
                case 0:
                    DialogueCounter++;
                    battleDialog.color = Color.white;
                    battleDialog.text = "Skeleton's bones rattled";
                    yield return new WaitForSeconds(1.5f);
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> Be careful! My bones are brittle from lack of vitamin D";
                    yield return new WaitForSeconds(2.5f);
                    break;
                case 1:
                    DialogueCounter++;
                    battleDialog.color = Color.white;
                    battleDialog.text = "Skeleton's tooth fell out";
                    yield return new WaitForSeconds(1.5f);
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> Just my luck, if only we had a dentist in this mansion";
                    yield return new WaitForSeconds(2.5f);
                    break;
                case 2:
                    DialogueCounter++;
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> I got no hair, but I sure got some hairline fractures now";
                    yield return new WaitForSeconds(2.5f);
                    break;
            }
        }
        else if (action.action.name == "Electrocute" && enemyReference.name.ToLower().Contains("skel"))
        {
            enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost * playerAttack / 4), false);
            battleDialog.color = Color.red;
            battleDialog.text = "<size=60%> was that supposed to hurt?";
            yield return new WaitForSeconds(2.5f);
        }
        else if (action.action.name == "Knife" && enemyReference.name.ToLower().Contains("eye"))
        {
            enemyNewHP = enemyHP.TakeDamage((int)(1.5f * playerPowerBoost/2 * playerAttack), false);
            switch (DialogueCounter)
            {
                case 0:
                    DialogueCounter++;
                    battleDialog.color = Color.white;
                    battleDialog.text = "One of the Monster's eyes popped";
                    yield return new WaitForSeconds(2f);
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> I would have been in trouble if not for my 11 other eyes";
                    yield return new WaitForSeconds(2.5f);
                    break;
                case 1:
                    DialogueCounter++;
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> That was my favorite eye!!";
                    yield return new WaitForSeconds(2f);
                    break;
                case 2:
                    DialogueCounter++;
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> Are you in a Knife Throwing Competition, and my eye is the bullseye???";
                    yield return new WaitForSeconds(2.5f);
                    break;
                default:
                    DialogueCounter++;
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> Stop it!! I am gonna run out of eyes!!";
                    yield return new WaitForSeconds(2.5f);
                    break;
            }
        }
        else if (action.action.name == "Slam" && enemyReference.name.ToLower().Contains("eye"))
        {
            enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost * playerAttack / 4), false);
            battleDialog.color = Color.red;
            battleDialog.text = "<size=60%> I won't go easy on you just cause you offer hugs!";
            yield return new WaitForSeconds(2.5f);
        }
        else if (enemyReference.name.ToLower().Contains("horse"))
        {
            Debug.Log("about to deal horse some damage");
            if ((action.action.name == "Fireball" || action.action.name == "FireElement") && enemyReference.name.ToLower().Contains("horse"))
            {
                switch (DialogueCounter)
                {
                    case 0:
                        DialogueCounter++;
                        enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost/2 * 1.1 * playerAttack), false);
                        battleDialog.color = Color.white;
                        battleDialog.text = "The boss started to glow slightly red";
                        yield return new WaitForSeconds(1.7f);
                        battleDialog.color = Color.red;
                        battleDialog.text = "<size=60%> Who turned on the heater?";
                        yield return new WaitForSeconds(2f);
                        break;
                    case 1:
                        DialogueCounter++;
                        enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost/2 * 1.2 * playerAttack), false);
                        battleDialog.color = Color.white;
                        battleDialog.text = "The boss turned even more red";
                        yield return new WaitForSeconds(1.7f);
                        battleDialog.color = Color.red;
                        battleDialog.text = "<size=60%>  Somebody! Turn on the A/C!";
                        yield return new WaitForSeconds(2f);
                        battleDialog.text = "<size=60%>  Oh wait... Our utility bills have been due for ages";
                        yield return new WaitForSeconds(2f);
                        break;
                    case 2:
                        DialogueCounter++;
                        enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost/2 * 1.3 * playerAttack), false);
                        battleDialog.color = Color.white;
                        battleDialog.text = "The boss started melting";
                        yield return new WaitForSeconds(1f);
                        battleDialog.color = Color.red;
                        battleDialog.text = "<size=60%> Go be a pyromaniac somewhere else!";
                        yield return new WaitForSeconds(2.5f);
                        break;
                    default:
                        DialogueCounter++;
                        enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost/2 * 1.5 * playerAttack), false);
                        battleDialog.color = Color.red;
                        battleDialog.text = "<size=60%> I am about to have a heat stroke!";
                        yield return new WaitForSeconds(2f);
                        break;
                }
            }
            else if (action.action.name == "Knife")
            {
                enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost * playerAttack / 4), false);
                battleDialog.color = Color.red;
                battleDialog.text = "<size=60%> Who brings a knife to a sword fight?";
                yield return new WaitForSeconds(2.5f);
            }
            else if ((action.action.name == "Slam" || action.action.name == "WaterElement" || action.action.name == "EarthElement"))
            {
                enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost * playerAttack / 4), false);
                battleDialog.color = Color.red;
                battleDialog.text = "<size=60%> You Call that an attack?";
                yield return new WaitForSeconds(2.5f);
            }
            else if (action.action.name == "Electrocute")
            {
                enemyNewHP = enemyHP.TakeDamage((int)(-1 * playerPowerBoost * playerAttack / 4), false);
                battleDialog.color = Color.red;
                battleDialog.text = "<size=60%> That gets my blood pumping!";
                yield return new WaitForSeconds(2.5f);
            }
            else if (action.action.name == "ElementalInfluence")
            {
			    enemyNewHP = enemyHP.TakeDamage(Mathf.CeilToInt(EleInfluenceDamange * (1 + playerAttackPower / 100.0f)), false);
		    }
            else
            {
                enemyNewHP = enemyHP.TakeDamage(playerPowerBoost/2 * playerAttack, false);
            }
        }
        else if (action.action.name == "Electrocute" && enemyReference.name.ToLower().Contains("enemyghost"))
        {
            enemyNewHP = enemyHP.TakeDamage((int)(3.0f*playerPowerBoost/4 * playerAttack), false);
            switch (DialogueCounter)
            {
                case 0:
                    DialogueCounter++;
                    battleDialog.color = Color.white;
                    battleDialog.text = "Enemy Ghost's figure started to flicker";
                    yield return new WaitForSeconds(1.5f);
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> I thought i saw my afterlife for a second there, oh wait...";
                    yield return new WaitForSeconds(2.5f);
                    break;
                case 1:
                    DialogueCounter++;
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> I think i just saw an old man waving to me from the other side of the river";
                    yield return new WaitForSeconds(2.5f);
                    break;
                case 2:
                    DialogueCounter++;
                    battleDialog.color = Color.red;
                    battleDialog.text = "<size=60%> I thought ghost's being weak to electric moves was a propaganda by pokemon!!";
                    yield return new WaitForSeconds(2.5f);
                    break;
            }
        }
        else if ((action.action.name == "EarthElement" || action.action.name == "WaterElement" || action.action.name == "Knife" || action.action.name == "Slam") && enemyReference.name.ToLower().Contains("enemyghost"))
        {
            enemyNewHP = enemyHP.TakeDamage((int)(playerPowerBoost * playerAttack / 8), false);
            battleDialog.color = Color.red;
            battleDialog.text = "<size=60%> That went right through!";
            yield return new WaitForSeconds(1.5f);
        }
		else if (action.action.name == "ElementalInfluence")
        {
			enemyNewHP = enemyHP.TakeDamage(Mathf.CeilToInt(EleInfluenceDamange * (1 + playerAttackPower / 100.0f)), false);
		}
		else
        {
            enemyNewHP = enemyHP.TakeDamage(playerPowerBoost/2 * playerAttack, false);
        }
	}

    public int getRandomAbilityBasedOnEnemyType()
    {
        var lowerCaseEnemyName = PlayerPrefs.GetString("ObjectToSpawn").ToLower();

        return lowerCaseEnemyName switch
        {
            string enemyName when enemyName.Contains("witch") => Time.renderedFrameCount % 50 + 50,
            string enemyName when enemyName.Contains("mushr") => Time.renderedFrameCount % 50,
            string enemyName when enemyName.Contains("enemyghost") => Time.renderedFrameCount % 50,
            string enemyName when enemyName.Contains("skeleton") => Time.renderedFrameCount % 50, //skeleton lower 2 abilities: knife or slam
            string enemyName when enemyName.Contains("monster") => Time.renderedFrameCount % 50 + 25, //monster middle 2: slam or fire
            string enemyName when enemyName.Contains("chess") || enemyName.Contains("horse") => Time.renderedFrameCount % 100, //final boss can do all 4 
            _ => 100  //default: electrocute
        };
    }

    IEnumerator EnemyTurn()
    {

        int randomInt = getRandomAbilityBasedOnEnemyType();
        CombatOptions enemyAction = CombatOptions.Knife;
        battleDialog.color = Color.white;
        string dialogText = "The enemy <harm> you";
		int playerDefensePower = GameManager.Instance.GetPlayerDefense();

		if (playerDodged)
        {
            animator.Play("PlayerDodge");
            dodgeSound.Play();
        }


        switch (randomInt)
        {
            case < 25:
                sendKnife(false);
                if (enemyReference.name.ToLower().Contains("skel"))
                {
                    battleDialog.text = playerDodged ? "You dodged the swinging sword!" : dialogText.Replace("<harm>", "threw a swinging sword at");
                    
                }
                else if (enemyReference.name.ToLower().Contains("horse"))
                {
                    battleDialog.text = playerDodged ? "You dodged the deadly katanas!" : dialogText.Replace("<harm>", "threw deadly katanas at");
                }
                else if (enemyReference.name.ToLower().Contains("enemyghost"))
                {
                    battleDialog.text = playerDodged ? "You dodged the strange hat!" : dialogText.Replace("<harm>", "threw a spinning hat at");
                }
                else if (enemyReference.name.ToLower().Contains("mushr"))
                {
                    battleDialog.text = playerDodged ? "You dodged the wild mushroom!" : dialogText.Replace("<harm>", "threw a poisonous spin at");
                }
                else if (enemyReference.name.ToLower().Contains("witch"))
                {
                    battleDialog.text = playerDodged ? "You dodged the witch's wrath!" : dialogText.Replace("<harm>", "threw a surprise attack at");
                }
                else
                {
                    battleDialog.text = playerDodged ? "You dodged the throwing knife!" : dialogText.Replace("<harm>", "threw a knife at");
                }
                yield return wait1sec;
                break;

            case < 50:
                enemyAction = CombatOptions.Slam;
                if (enemyReference.name.ToLower().Contains("skel"))
                {
                    battleDialog.text = playerDodged ? "You dodged the skeleton's slash!" : dialogText.Replace("<harm>", "unleashes a deadly slash at");

                }
                else if (enemyReference.name.ToLower().Contains("horse"))
                {
                    battleDialog.text = playerDodged ? "You dodged the knight's slam!" : dialogText.Replace("<harm>", "unleashes a furious slam at");
                }
                else if (enemyReference.name.ToLower().Contains("enemyghost"))
                {
                    battleDialog.text = playerDodged ? "You dodged a quick attack!" : dialogText.Replace("<harm>", "unleashes a quick attack at");
                }
                else if (enemyReference.name.ToLower().Contains("mushr"))
                {
                    battleDialog.text = playerDodged ? "You dodged the mushroom's squish!" : dialogText.Replace("<harm>", "unleashes a huge squish at");
                }
                else if (enemyReference.name.ToLower().Contains("witch"))
                {
                    battleDialog.text = playerDodged ? "You dodged the witch's wrath!" : dialogText.Replace("<harm>", "unleashes a wicked orb at");
                }
                else {
                    battleDialog.text = playerDodged ? "You dodged the enemy's slam!" : dialogText.Replace("<harm>", "slammed");
                }
                if (!playerDodged) { 
                    sendSlam(false);
                    yield return wait1sec;
                }
                break;

            case < 75:
                enemyAction = CombatOptions.Fireball;
                battleDialog.text = playerDodged ? "You dodged the fireball!" : dialogText.Replace("<harm>", "threw a fireball at");
                sendFireball(false);
                yield return wait1sec; //new WaitForSeconds(1f);
                break;

            default:
                enemyAction = CombatOptions.Electrocute;
                battleDialog.text = playerDodged ? "You're unable to dodge the lightning!" : dialogText.Replace("<harm>", "electrocutes");
                var lightning = sendLightning(false);
                yield return wait1sec; //new WaitForSeconds(1f);
                Destroy(lightning);
                break;
        }
        if (!playerDodged || enemyAction == CombatOptions.Electrocute) 
        {   
            var enemyName = PlayerPrefs.GetString("ObjectToSpawn").ToLower();
            var damage = (int)enemyAction.damage * (enemyName.Contains("chess") || enemyName.Contains("horse") ? 2 : 1);//if final boss x2 damage
            damage = damage / (enemyAction == CombatOptions.Electrocute && playerDodged ? 2 : 1);//id dodging electrocute, only 1/2 damage 
            playerHP.TakeDamage(Mathf.CeilToInt(enemyDifficulty * damage * (1 - playerDefensePower / 100.0f)), true);
        }
        playerDodged = false;

        yield return new WaitForSeconds(dialogWaitTime);

        if (playerHP.currentHealth <= 0)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            // enable the primary camera after the curernt turn is over
            primaryCamera.enabled = true;
            mcCamera.enabled = false;
            enemyCamera.enabled = false;
            state = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    IEnumerator EndBattle()
    {
        player.GetComponentInChildren<Rigidbody>().constraints = playerRBConstraints;//restore ability to move/rotate
        DialogueCounter = 0;    //Set the dialogue counter back to initial value
        yield return DeathDialogues();
        if (state == BattleState.WON)
        {
            winSound.Play();
            ghostAnimator.SetBool("isWin", true); //win anim
            battleDialog.color = Color.white;
            battleDialog.text = "You have prevailed!";
            // give random equipment
            InventoryItem equip = GameObject.Find("InventoryManager").GetComponent<InventoryManager>().GiveRandomEquipment();
            battleDialog.text += "\nGet " + equip.itemName + " (" + equip.specialInfo + ")";
            var lowerCaseEnemyName = PlayerPrefs.GetString("ObjectToSpawn").ToLower();
            if (lowerCaseEnemyName.Contains("skeleton"))
            {
				battleDialog.text += "\nCoin + 20";
				GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + 30);
			}
            else if (lowerCaseEnemyName.Contains("monster"))
            {
				battleDialog.text += "\nCoin + 10";
				GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + 30);
			}
            else if (lowerCaseEnemyName.Contains("enemyghost"))
            {
                battleDialog.text += "\nCoin + 20";
                GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + 20);
            }
            else if (lowerCaseEnemyName.Contains("mushr"))
            {
                battleDialog.text += "\nCoin + 10";
                GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + 10);
            }
            else if (lowerCaseEnemyName.Contains("witch"))
            {
                battleDialog.text += "\nCoin + 20";
                GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + 20);
            }
            // This can be replaced with a confirmation UI when we're ready
            yield return new WaitForSecondsRealtime(2f);

            var sceneChanger = GetComponent<SceneChangeInvokable>();
            if (lowerCaseEnemyName.Contains("horse"))
            {
                // if defeat the boss, go to ending scene
                sceneChanger.sceneName = "EndingStory";
            }
            else
            {
                sceneChanger.sceneName = GameManager.Instance.PrepareForReturnFromCombat();
            }
            sceneChanger.Invoke();
        }
        else
        {
            loseSound.Play();
            ghostAnimator.SetBool("isDead", true); //death anim
            battleDialog.text = "You were vanquished!";
            //move back to checkpoint
            yield return new WaitForSecondsRealtime(3f);
            // Debug.Log("AFter being defeated " + GameManager.Instance.GetPlayerHealth());
            GameManager.Instance.LoadCheckpoint();
        }
    }

    IEnumerator DeathDialogues()
    {
        if (state == BattleState.WON)
        {
            enemyAnimator.SetBool("isDead", true); // death anim
        }

        if (enemyReference.name.ToLower().Contains("skel"))
        {
            battleDialog.color = Color.red;
            battleDialog.text = "I will pick a bone with you next time!";
            yield return new WaitForSeconds(2.5f);
        }
        else if (enemyReference.name.ToLower().Contains("eye"))
        {
            battleDialog.color = Color.red;
            battleDialog.text = "I did not... see that coming...";
            yield return new WaitForSeconds(2.5f);
        }
        else if (enemyReference.name.ToLower().Contains("horse"))
        {
            battleDialog.color = Color.red;
            battleDialog.text = "This is just the beginning";
            yield return new WaitForSeconds(2f);
            battleDialog.color = Color.white;
            battleDialog.text = "The Boss disappeared into the ground";
            yield return new WaitForSeconds(2f);
        }
        else if (enemyReference.name.ToLower().Contains("enemyghost"))
        {
            battleDialog.color = Color.red;
            battleDialog.text = "What... are you...?";
            yield return new WaitForSeconds(2.5f);
        }
        else if (enemyReference.name.ToLower().Contains("mushr"))
        {
            battleDialog.color = Color.red;
            battleDialog.text = "I thought my poison was deadly...";
            yield return new WaitForSeconds(2.5f);
        }
        else if (enemyReference.name.ToLower().Contains("witch"))
        {
            battleDialog.color = Color.red;
            battleDialog.text = "May we meet again, J-";
            yield return new WaitForSeconds(2.5f);
        }
    }

    // FIRE ANIM
    private IEnumerator damagedFire()
    {
        yield return new WaitForSeconds(0.1f);
        ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
        yield return new WaitForSeconds(2f);
        ghostAnimator.SetBool("isDamaged", false);
    }

    private IEnumerator enemyDamagedFire()
    {
        yield return new WaitForSeconds(0.1f);
        enemyAnimator.SetBool("isDamaged", true); // enemy damaged anim
        yield return wait2sec;
        enemyAnimator.SetBool("isDamaged", false);
    }

    GameObject sendFireball(bool isFromPlayer = true)
    {
        if (isFromPlayer)
        {
            GameObject fire = GameObject.Instantiate(fireboltAsset);
            fire.transform.position = player.transform.position + new Vector3(1, .5f, 1);
            fire.transform.rotation = new Quaternion(0, 0.70711f, 0, 0.70711f);
            StartCoroutine(enemyDamagedFire());
        }
        else
        {
            GameObject fire = GameObject.Instantiate(fireboltAsset);
            fire.transform.position = GameObject.FindWithTag("enemyReference").transform.position + new Vector3(-2, .5f, -1);
            fire.transform.rotation = new Quaternion(0, 0.70711f, 0, -0.70711f);
            StartCoroutine(damagedFire());
        }
        return null;
    }
    
    // SLAM ANIM
    private IEnumerator animateAndWaitThenDeactivate(string anim)
    {
        enemyAnimator.SetBool(anim, true);
        
        if (enemyReference.name.ToLower().Contains("eye")) // eye slam
        {
            yield return new WaitForSeconds(1.9f);
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            slamSound.PlayDelayed(0.5f);
            yield return new WaitForSeconds(1.5f);
            ghostAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("horse")) // horse slam
        {
            yield return new WaitForSeconds(2f);
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            slamSound.Play();
            yield return new WaitForSeconds(1.5f);
            ghostAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("enemyghost")) // enemyghost slam
        {
            yield return new WaitForSeconds(1f);
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            slamSound.Play();
            yield return new WaitForSeconds(1.5f);
            ghostAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("mushr")) // mushroom slam
        {
            yield return new WaitForSeconds(1f);
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            slamSound.Play();
            yield return new WaitForSeconds(0.4f);
            ghostAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("witch")) // witch slam <<<<<<<<<< TO DO
        {
            //yield return new WaitForSeconds(1f);
            //ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            //slamSound.Play();
            //yield return new WaitForSeconds(0.4f);
            //ghostAnimator.SetBool("isDamaged", false);
        }
        else
        { //skel slam
            yield return new WaitForSeconds(1.7f);
            animator.Play("PlayerSlammed"); //ghost damaged anim
            slamSound.PlayDelayed(0.7f);
            yield return new WaitForSeconds(3.6f);
        }
        enemyAnimator.SetBool(anim, false);
    }

    private IEnumerator ghostSlam(string anim)
    {
        ghostAnimator.SetBool(anim, true);
        slamSound.PlayDelayed(1f);
        if (enemyReference.name.ToLower().Contains("eye"))
        {
            // check timing later <<<
            enemyAnimator.SetBool("isDamaged", true); //eye damaged anim
            yield return wait2sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("skel"))
        {
            // check timing later <<<
            enemyAnimator.SetBool("isDamaged", true); //skel damaged anim
            yield return wait2sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("enemyghost"))
        {
            // check timing later <<<
            enemyAnimator.SetBool("isDamaged", true); //enemyghost damaged anim
            yield return wait2sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("mushr"))
        {
            // check timing later <<<
            enemyAnimator.SetBool("isDamaged", true); //mushr damaged anim
            yield return wait2sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("witch"))
        {
            // check timing later <<<
            //enemyAnimator.SetBool("isDamaged", true); //witch damaged anim
            //yield return wait2sec;
            //enemyAnimator.SetBool("isDamaged", false);
        }
        // add horse
        else
        {
            yield return wait2sec;
        }
        ghostAnimator.SetBool(anim, false);
    }

    GameObject sendSlam(bool isFromPlayer = true)
    {
        if(isFromPlayer)
        {
            StartCoroutine(ghostSlam("isCombat"));
        }
        else
        {
         StartCoroutine(animateAndWaitThenDeactivate("isCombat"));
        }
        return null;
    }

    // LIGHTNING ANIM
    private IEnumerator damagedLightning()
    {
        ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
        yield return new WaitForSeconds(2f);
        ghostAnimator.SetBool("isDamaged", false);
    }

    private IEnumerator enemyhurtLightning()
    {
        enemyAnimator.SetBool("isDamaged", true); // enemy damaged anim
        yield return wait2sec;
        enemyAnimator.SetBool("isDamaged", false);
    }

    GameObject sendLightning(bool isFromPlayer = true)
    {
        var lightningObj = GameObject.Instantiate(lightningAsset);
        electrocuteSound.Play();
        var lightningComp = lightningObj.GetComponent<LightningBoltScript>();
        lightningComp.StartObject = GameObject.Find("ghost basic");
        lightningComp.EndObject = GameObject.FindWithTag("enemyReference");
        lightningComp.Generations = 3;

        if (!isFromPlayer)
        {
            StartCoroutine(damagedLightning()); 
        }
        else
        {
            StartCoroutine(enemyhurtLightning());
        }
        
        return lightningObj;
    }

    // THROW ANIM
    private IEnumerator animateThrow(string anim)
    {
        if (!playerDodged)
        {
            enemyAnimator.SetBool(anim, true);
            //check timing later
            if (enemyReference.name.ToLower().Contains("enemyghost")) // enemy ghost hat throw sound
            {
                knifeSound.PlayDelayed(1.2f);
                yield return new WaitForSeconds(1.5f);
            }
            else if (enemyReference.name.ToLower().Contains("mushr")) // mushr throw sound
            {
                knifeSound.PlayDelayed(0.7f);
                yield return new WaitForSeconds(1f);
                enemyAnimator.SetBool(anim, false);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                knifeSound.PlayDelayed(1.1f);
            }
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            yield return new WaitForSeconds(1.8f);
            ghostAnimator.SetBool("isDamaged", false);
        }
        else
        {
            yield return wait2sec;
        }
        enemyAnimator.SetBool(anim, false);
    }

    private IEnumerator bossThrow(string anim)
    {
        if (!playerDodged)
        {
            enemyAnimator.SetBool(anim, true);
            knifeSound.PlayDelayed(3.7f);
            yield return new WaitForSeconds(3.6f);
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            yield return new WaitForSeconds(2f);
            ghostAnimator.SetBool("isDamaged", false);
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }
        enemyAnimator.SetBool(anim, false);
    }

    private IEnumerator damagedThrow()
    {
        if (!playerDodged)
        {
            yield return new WaitForSeconds(1f);
            ghostAnimator.SetBool("isDamaged", true); //ghost damaged anim
            yield return new WaitForSeconds(2f);
            ghostAnimator.SetBool("isDamaged", false);
        }
    }

    private IEnumerator enemydamagedThrow()
    {
        if (enemyReference.name.ToLower().Contains("eye"))
        {
            // check timing later <<<
            yield return new WaitForSeconds(0.2f);
            enemyAnimator.SetBool("isDamaged", true); //eye damaged anim
            yield return wait2sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("skel"))
        {
            // check timing later <<<
            yield return new WaitForSeconds(0.2f);
            enemyAnimator.SetBool("isDamaged", true); //skel damaged anim
            yield return wait1sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("enemyghost"))
        {
            // check timing later <<<
            yield return new WaitForSeconds(0.2f);
            enemyAnimator.SetBool("isDamaged", true); //enemyghost damaged anim
            yield return wait1sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("mushr"))
        {
            // check timing later <<<
            yield return new WaitForSeconds(0.2f);
            enemyAnimator.SetBool("isDamaged", true); //mushroom damaged anim
            yield return wait1sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
        else if (enemyReference.name.ToLower().Contains("witch"))
        {
            // check timing later <<<
            //yield return new WaitForSeconds(0.2f);
            //enemyAnimator.SetBool("isDamaged", true); //mushroom damaged anim
            //yield return wait1sec;
            //enemyAnimator.SetBool("isDamaged", false);
        }
        else
        {
            // check timing later <<<
            yield return new WaitForSeconds(0.2f);
            enemyAnimator.SetBool("isDamaged", true); //horse damaged anim
            yield return wait2sec;
            enemyAnimator.SetBool("isDamaged", false);
        }
    }

    GameObject sendKnife(bool isFromPlayer = true)
    {
        if (isFromPlayer)
        {
            animator.Play("PlayerThrowKnife");
            knifeSound.PlayDelayed(0.05f);
            StartCoroutine(enemydamagedThrow());
        }
        else
        {
            if (enemyReference.name.ToLower().Contains("skel")) // sword throw skeleton
            {
                StartCoroutine(animateThrow("isThrow"));
            }
            else if (enemyReference.name.ToLower().Contains("enemyghost")) // enemy ghost hat throw
            {
                StartCoroutine(animateThrow("isThrow"));
            }
            else if (enemyReference.name.ToLower().Contains("mushr")) // mushroom throw
            {
                StartCoroutine(animateThrow("isThrow"));
            }
            else if (enemyReference.name.ToLower().Contains("horse")) // katana throw
            {
                StartCoroutine(bossThrow("isThrow"));
            }
            else
            {
                knifeSound.PlayDelayed(2);
                animator.Play("EnemyThrowKnife");
                StartCoroutine(damagedThrow());
                knifeSound.PlayDelayed(0.05f);
            }
        }
        return null;
    }

    // HEAL ANIM
    GameObject selfHeal(bool isFromPlayer = true)
    {
        try
        {
            GameObject.Instantiate(healAsset, GameObject.Find("ghost basic").transform);
        } catch (Exception) { }
        if (playerHP.currentHealth <= 85)
        {
            playerHP.TakeDamage(-(int)CombatOptions.Heal.damage);
            battleDialog.text = $"You gained {(int)CombatOptions.Heal.damage}HP";
        }
        else
        {
            int healing = 100 - playerHP.currentHealth;
            playerHP.TakeDamage(-healing);
            battleDialog.text = $"You gained {healing}HP";
        }
        healSound.Play();

        return null;
    }

    private IEnumerator enemyStunned()
    {
        //check timing later <<<
        enemyAnimator.SetBool("isDamaged", true); //enemy damaged anim
        yield return wait2sec;
        enemyAnimator.SetBool("isDamaged", false);
    }

    // STUN ANIM
    GameObject sendStun(bool isFromPlayer = true)
    {
        Destroy(stunObj);//remove prev stun effect if any

        animator.Play("PlayerStun");
        stunSound.PlayDelayed(0.2f);
        StartCoroutine(enemyStunned());
        
        try
        {
            stunObj = Instantiate(enemyStunAsset, gameObject.transform);
        }
        catch (Exception) { }

        remaningStunTurns += 2;

        return null;
    }

    GameObject sendFireEle(bool isFromPlayer = true)
    {
		animator.Play("PlayerThrowFireEle");
        fireElementSound.Play();
        StartCoroutine(enemydamagedThrow()); //check timing
		if (fireCount == 0)
		    fireCount++;
		return null;
	}

    GameObject sendEarthEle(bool isFromPlayer = true)
    {
		animator.Play("PlayerThrowEarthEle");
        earthElementSound.Play();
        StartCoroutine(enemydamagedThrow()); //check timing
        if (earthCount == 0)
		    earthCount++;
		return null;
    }

    GameObject sendWaterEle(bool isFromPlayer = true)
    {
		animator.Play("PlayerThrowWaterEle");
        waterElementSound.Play(); 
        StartCoroutine(enemydamagedThrow()); //check timing
        if (waterCount == 0)
		    waterCount++;
		return null;
    }

    GameObject sendElemental(bool isFromPlayer = true)
    {
        if (fireCount > 0)
        {
			animator.Play("PlayerThrowFireEle");
            fireElementSound.Play();
		}
		if (waterCount > 0)
		{
			animator.Play("PlayerThrowWaterEle");
            waterElementSound.Play();
		}
		if (earthCount > 0)
		{
			animator.Play("PlayerThrowEarthEle");
            earthElementSound.Play();
		}
		EleInfluenceDamange = (fireCount + earthCount + waterCount) * 10;
        if (fireCount > 0 && earthCount > 0 && waterCount > 0)
		{
			EleInfluenceDamange += 10;
		}
        StartCoroutine(enemydamagedThrow()); //check timing
        return null;
	}
    GameObject sendHydrophere(bool isFromPlayer = true)
    {
        if (fireCount > 0)
        {
            animator.Play("PlayerThrowFireEle");
            fireElementSound.Play();
        }
        if (waterCount > 0)
        {
            animator.Play("PlayerThrowWaterEle");
            waterElementSound.Play();
        }
        if (earthCount > 0)
        {
            animator.Play("PlayerThrowEarthEle");
            earthElementSound.Play();
        }
        EleInfluenceDamange = (fireCount + earthCount + waterCount) * 10;
        if (fireCount > 0 && earthCount > 0 && waterCount > 0)
        {
            EleInfluenceDamange += 10;
        }
        StartCoroutine(enemydamagedThrow()); //check timing
        return null;
    }

    private void UpdateDifficulty()
    {
        if (CheatCodeEntered())
        {
            playerPowerBoost = (playerPowerBoost + 1) % 7;
            Debug.Log($"Easier Difficulty, player's power boost = {playerPowerBoost}");
        }
        if (ResetDifficulty())
        {
            playerPowerBoost = 2;
            Debug.Log("Reset Difficulty");
        }
    }

    private static bool ResetDifficulty()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    private static bool CheatCodeEntered()
    {
        return Input.GetKeyDown(KeyCode.E);
    }


    /// <summary>
    /// combat buttons handlers
    /// </summary>
    public void OnSlamButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new (CombatOptions.Slam, 1.5f, sendSlam));
        }
    }

    public void OnFireButton()//todo: remove listener on mouse click
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new (CombatOptions.Fireball, 1f, sendFireball));
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
            turnActions.Add(new(CombatOptions.Electrocute, 3.5f, sendLightning));
        }
    }

    public void OnKnifeButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.Knife, 1f, sendKnife));
        }
    }
    public void OnStunButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.Stun, 1f, sendStun));
        }
    }
    public void OnHealButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.Heal, 0, selfHeal));
        }
    }

    public void OnFireEleButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.FireElement, 0, sendFireEle));
        }
    }
    public void OnEarthEleButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.EarthElement, 0, sendEarthEle));
        }
    }
    public void OnWaterEleButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.WaterElement, 0, sendWaterEle));
        }
    }
    public void OnElementalButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.ElementalInfluence, 0, sendElemental));
        }
    }
    public void OnHydrosphereButton()
    {
        if (state == BattleState.PLAYER_TURN)
        {
            turnActions.Add(new(CombatOptions.Hydrosphere, 0, sendHydrophere));
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

    public void RegisterStartBattleListener(Func<string> fun)
    {
        battleStartListeners.Add(fun);
    }
}
