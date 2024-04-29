using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{

    public string uID;
    GameObject player;
    NavMeshAgent agent;
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;

    [SerializeField] LayerMask groundLayer, playerLayer;

    //patro
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;

    // state range
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    Animator animator;
    float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (SceneManager.GetActiveScene().name != "Combat Arena")
        {
            //agent.updateRotation = false;
            agent.speed = 2;
            player = GameObject.Find("GhostPC");
            animator = GetComponent<Animator>();
            timer = 0; // TIMING FOR IDLE
            agent.isStopped = false;
            agent.updatePosition = true;
        }
        else
        {
            // DON'T DO A THING IN COMBAT
            agent.updatePosition = false;
            agent.nextPosition = transform.position;
            agent.isStopped = true; 
        }
	}

	void Update()
    {
        if (SceneManager.GetActiveScene().name != "Combat Arena" && dialogueUI.IsOpen)
        {
            agent.SetDestination(transform.position);
            animator.SetBool("isChasing", false);
            animator.SetBool("isPatroling", false);
            return;
        }

        if (SceneManager.GetActiveScene().name != "Combat Arena" && !(dialogueUI.IsOpen))
        {
            Vector3 newPosition = transform.position;
		    newPosition.z = player.transform.position.z;
		    transform.position = newPosition;
		
			playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
			playerInSight = Physics.CheckSphere(transform.position, attackRange, playerLayer);

            timer += Time.deltaTime;
            agent.isStopped = false; // MOVE BY DEFAULT
            if (timer < 5){
                if (animator.GetBool("isChasing") == false) // if not chasing, prevent movement
                    agent.isStopped = true;
                else
                    agent.isStopped = false; // otherwise MOVE
            }

            if (!playerInSight && !playerInAttackRange && timer > 5)
                    Patrol(); // IF IDLE FOR A WHILE PLAY PATROL
            else {
                    animator.SetBool("isPatroling", false); // NOT PATROL
                 }

            if (playerInSight && !playerInAttackRange)
                    Chase(); 
            else {
                    animator.SetBool("isChasing", false); // NOT CHASING
                 }

            if (playerInSight && playerInAttackRange) Attack();

        }
    }

    void Chase()
    {
		if (SceneManager.GetActiveScene().name != "Combat Arena")
        {
            // ROTATION ADJUSTMENT
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);

            animator.SetBool("isChasing", true); // CHASE ANIM

            //Debug.Log("Chase");
			agent.SetDestination(player.transform.position);
			agent.speed = 3.5f;

            if (timer > 10 && animator.GetBool("isChasing") == false) // RESET TIMER WHEN DONE
                timer = 0;
        }
    }

    // function does not seem to be executing ? 
    void Attack()
    {
		//Debug.Log("Attack");
   
	}


    void Patrol()
    {
        animator.SetBool("isPatroling", true); // PATROL ANIM

        if (!walkpointSet) SearchForDest();
        if (walkpointSet) agent.SetDestination(destPoint);
		// If the distance between the enemy's current position and the destPoint is less than 10 units
        // Sets walkpointSet to false, indicating that a new destination needs to be found
		if (Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;

        if (timer > 10) // SWITCH TO IDLE AFTER A WHILE 
        {
            animator.SetBool("isPatroling", false);
            timer = 0;
        }
    }

    void SearchForDest()
    {
        // float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
        // Debug.Log("DesPoint" + destPoint);

		//Performs a raycast from destPoint downwards to check if it hits an object on the groundLayer
        //If it does, walkpointSet is set to true, indicating that this point is a valid destination for the enemy to walk towards
		if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
