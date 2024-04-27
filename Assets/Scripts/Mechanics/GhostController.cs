using UnityEngine;

namespace Platformer.Mechanics
{
    using static Platformer.Mechanics.TurnDirection;
    using static Platformer.Mechanics.JumpState;
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// </summary>
    public class GhostController : MonoBehaviour
    {
        // Dialogue part
        [SerializeField] private DialogueUI dialogueUI;
        public DialogueUI DialogueUI => dialogueUI;
        public IInteractable Interactable { get; set; }

        private GameObject _ghost_model;
        private const float floatSpeed = 0.125f;
        private const float moveSpeed = 5f;
        public JumpState jump = InFlight;
        public bool controlEnabled = true;
        private bool usedDoubleJump = false;
        private float keyHoriz = 0f;
        private float keyVert = 0f;
        private bool upPressed = false;
        private bool jumpPending = false;
		public GameObject ghostPrefab;
		/*internal new*/
		public Collider _collider;

        public Rigidbody _rigidbody;
        [SerializeField] AudioSource jumpSound;
        public Bounds Bounds => _collider.bounds;
        public bool SaveCheckpoint = true;
        public ShopManager shopManager;

        Animator anim; //
        int horizontal; //
        int vertical; //
        GameObject anchor; //
        Transform anchorManip; //

        void Awake()
        {
            _ghost_model = GameObject.Find("ghost basic");
            _rigidbody = GetComponent<Rigidbody>();
            anim = _ghost_model.GetComponent<Animator>(); // animator inside ghost basic
            horizontal = Animator.StringToHash("Horizontal"); // reference values in the Animmator called Horizontal/Vertical
            vertical = Animator.StringToHash("Vertical"); //
            anchor = GameObject.Find("Anchor"); // for rotation
            anchorManip = anchor.transform; // to tell rotation direction

            var gm = GameManager.Instance;
            if (gm != null)
            {
                Debug.Log("Player health before saving checking point " + gm.GetPlayerHealth());
                if (SaveCheckpoint)
                {
                    gm.SaveCheckpoint();
                }
                if (gm.PlayerPos.TryGetValue(gm.SceneName, out Vector3 pos))
                {
                    transform.position = pos;
                }
				Color color = gm.GetPlayerColor();
				Renderer ghostRenderer = ghostPrefab.GetComponent<Renderer>();
				Material ghostMaterial = ghostRenderer.sharedMaterial;
				ghostMaterial.SetColor("_Color_Me", color);
				gm.SetPlayerColor(color);
			}

            if (shopManager == null)
            {
                Debug.LogWarning("ShopManager not found in the scene.");
            }
        }

        //void Update()
        //{
        //    // Dialogue part
        //    if (dialogueUI != null && (shopManager.gameObject.activeSelf || dialogueUI.IsOpen))
        //    {
        //        DisableControl();
        //        return;
        //    }

        //    keyHoriz = Input.GetAxis("Horizontal");
        //    keyVert = Input.GetAxis("Vertical");
        //    jumpPending = jumpPending || keyVert > 0;

        //    // Dialogue part
        //    if (Input.GetButtonDown("Jump") && !upPressed && !shopManager.gameObject.activeSelf)
        //    {
        //        upPressed = true;
        //        Interactable?.Interact(this);
        //        dialogueUI?.RegisterCloseAction(EnableControl);
        //        /*
        //        meaning:
        //        if (Interactable != null)
        //        {
        //            Interactable.Interact(this);
        //        }
        //        */
        //    }
        //    else
        //    {
        //        upPressed = false;
        //    }
        //}


        void Update()
        {
            // Dialogue part
            if (shopManager != null)
            {
                if (dialogueUI != null && (shopManager.gameObject.activeSelf || dialogueUI.IsOpen))
                {
                    DisableControl();
                    return;
                }
            }
            else
            {
                if (dialogueUI != null && dialogueUI.IsOpen)
                {
                    DisableControl();
                    return;
                }
            }

            keyHoriz = Input.GetAxis("Horizontal");
            keyVert = Input.GetAxis("Vertical");

            if (keyVert < 0) keyVert = 0; // when pressing S it goes below 0 , just making sure no issues with rotation

            jumpPending = jumpPending || keyVert > 0;

            // Dialogue part
            if (shopManager != null)
            {
                if (Input.GetButtonDown("Jump") && !upPressed && !shopManager.gameObject.activeSelf)
                {
                    upPressed = true;
                    Interactable?.Interact(this);
                    dialogueUI?.RegisterCloseAction(EnableControl);
                    /*
                    meaning:
                    if (Interactable != null)
                    {
                        Interactable.Interact(this);
                    }
                    */
                }
                else
                {
                    upPressed = false;
                }
            }
            else
            {
                if (Input.GetButtonDown("Jump") && !upPressed)
                {
                    upPressed = true;
                    Interactable?.Interact(this);
                    dialogueUI?.RegisterCloseAction(EnableControl);
                    /*
                    meaning:
                    if (Interactable != null)
                    {
                        Interactable.Interact(this);
                    }
                    */
                }
                else
                {
                    upPressed = false;
                }
            }
        }

        protected void FixedUpdate()
        {
            // froze the player when playing dialogue
            if (!controlEnabled)
            {
                anim.SetBool("isJumping", false);
                return;
            }

            // Spoooky float!
            var curr_pos = _ghost_model.transform.position;
            _ghost_model.transform.position = new Vector3(
                curr_pos.x,
                curr_pos.y + floatSpeed * Mathf.Cos(Time.time) * Time.fixedDeltaTime,
                curr_pos.z
            );

            var sideMove = keyHoriz * moveSpeed;
            HandleRotation(keyHoriz, keyVert); // rotate
            var upMove = HandleJump();

            // Debugging 
            //print("Side move is: " + sideMove + " | keyhoriz is: " + keyHoriz);
            //print("Up move is: " + upMove + " | keyvert is: " + keyVert);
            // if (keyHoriz != 0 || keyVert != 0)
                // print("Pressed A(-1) or D(+1): " + keyHoriz + " | Pressed W(+1): " + keyVert);

            UpdateAnimatorValues(keyHoriz, keyVert); //Right or Left movement animation
            if(keyVert > 0)
            {
                anim.SetBool("isJumping", true);
            }
            else
            {
                anim.SetBool("isJumping", false);
            }

            // Move horizontally
            _rigidbody.MovePosition(_rigidbody.transform.position + new Vector3(sideMove * Time.deltaTime, 0, 0));
            // Move vertically
            _rigidbody.AddForce(new Vector3(0, upMove, 0), ForceMode.Impulse);
        }

        public bool GetControlStatus()
        {
            return controlEnabled;
        }

        public void DisableControl()
        {
            controlEnabled = false;
        }

        public void EnableControl()
        {
            controlEnabled = true;
        }

        private void HandleRotation(float keyHoriz, float keyVert)
        {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = anchorManip.forward * keyVert; 
            targetDirection += anchorManip.right * keyHoriz;
            targetDirection.y = 0; 
            targetDirection.z = 0;
            targetDirection.Normalize();

            // if we're moving right or left we can decide which way to rotate
            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection); //tarot = lookrot(ta)
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, 0); // 2*rotationSpeed * Time.deltaTime / rotationSpeed); // dividing slows rotation
                                                                                          // ^ setting to 0 seems to fix anim rotation issues idk why
            transform.rotation = playerRotation; 
        }

        private float HandleJump()
        {
            // Jump!
            var upMove = 0f;
            switch (jump)
            {
                case InFlight:
                    if (_rigidbody.velocity.y == 0)
                    {
                        jump = Grounded;
                        usedDoubleJump = false;
                        jumpPending = false;
                    }
                    else if (!usedDoubleJump && jumpPending)
                    {
                        usedDoubleJump = true;
                        jumpSound.Play();
                        upMove = moveSpeed;
                        //print("InFlight.Pending.UpMove: " + upMove);
                        jumpPending = false;
                        var curr_vel = _rigidbody.velocity;
                        _rigidbody.velocity = new Vector3(
                            curr_vel.x,
                            0,
                            curr_vel.z
                        );
                     
                    }
                    break;
                case Grounded:
                    if (jumpPending)
                    {
                        upMove = moveSpeed;
                        //print("Grounded.upMove: " + upMove);
                        jumpSound.Play();
                        jump = Jumping;
                        jumpPending = false;
                        
                    }
                    break;
                case Jumping:
                    if (Input.GetAxis("Vertical") <= 0)
                    {
                        jump = InFlight;
                        jumpPending = false;
                    }
                    break;

            }
            return upMove;
        }

        public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement)
        {
            //Animation snapping < optional
            float snappedHoriz;
            float snappedVerti;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                snappedHoriz = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                snappedHoriz = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                snappedHoriz = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                snappedHoriz = -1;
            }
            else
            {
                snappedHoriz = 0;
            }

            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                snappedVerti = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                snappedVerti = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                snappedVerti = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                snappedVerti = -1;
            }
            else
            {
                snappedVerti = 0;
            }

            anim.SetFloat(horizontal, snappedHoriz, 0.1f, Time.deltaTime);
            anim.SetFloat(vertical, snappedVerti, 0.1f, Time.deltaTime);
        }

    }
    public enum TurnDirection
    {
        LEFT,
        RIGHT,
        FRONT,
        BACK,
        NOT_TURNING
    }

    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Landed
    }
}