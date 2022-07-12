using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*
     * Script describe behavior of player: walking, run, jump, attack and etc.
     */

    [SerializeField]
    private int health;
    [SerializeField]
    private int walkSpeed;
    [SerializeField]
    private int runSpeed;
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private ProgressBar healthBar;

    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.1f;

    private Animator animator;
    private CharacterController controller;
    private WeaponsInHandController handController;

    private bool isBlock;
    private bool isGrounded;
    private Vector3 velocity;

    private Vector3 direction;
    private float moveSpeed;

    //Animation paramatres:
    private int animationSpeedHash;
    private int animationJumpHash;
    private int animationAttackHash;
    private int animationShield;
    private int animationBlock;
    private int attackLayerID;

    public static bool isAttacking { get; private set; } = false;

    private void Awake()
    {
        isAttacking = false;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        handController = GetComponent<WeaponsInHandController>();

        animationSpeedHash = Animator.StringToHash("Speed");
        animationJumpHash = Animator.StringToHash("IsJump");
        animationAttackHash = Animator.StringToHash("Attack");
        animationBlock = Animator.StringToHash("IsBlock");
        animationShield = Animator.StringToHash("Shield");
        attackLayerID = animator.GetLayerIndex("Attack Layer");
    }

    private void Start()
    {
        healthBar.Initialize(health);
    }

    public void Hurt(float damage)
    {
        if (!isBlock)
            healthBar.Value -= damage.ToPercent(healthBar.MaxValue);
        else if(handController.GetWeaponInHand(HandType.leftHand).GetComponent<Shield>() != null)
        {
            Shield shield = handController.GetWeaponInHand(HandType.leftHand).GetComponent<Shield>();
            healthBar.Value -= (damage - (shield.damageAbsorption * damage)).ToPercent(healthBar.MaxValue);
        }

        if (healthBar.Value <= 0.1f)
            Dead();
    }

    private void Dead()
    {
        animator.Play("player_dead");
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, time);
    }

    private void OnDestroy()
    {
        UIManager.SetActivePanel(PanelType.endGamePanel, true);
    }

    private void Update()
    {
        Move();

        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                handController.SelecteWeapon(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                handController.SelecteWeapon(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                handController.SelecteWeapon(3);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                handController.SelecteWeapon(4);
            else if (Input.GetKeyDown(KeyCode.Q))
                handController.DropItemFromHand();
        }

        if (Input.GetMouseButton(0) && !isAttacking)
        {
            isAttacking = true;
            Attack();
        }

        if (Input.GetMouseButtonDown(1))
            SetShield(true);
        else if (Input.GetMouseButtonUp(1))
            SetShield(false);
    }

    #region Attack and block controller
    /// <summary>
    /// Start or stop animation to set shield
    /// </summary>
    /// <param name="flag">True - start animation. False - stop animation</param>
    private void SetShield(bool flag)
    {
        isBlock = flag;

        if (handController.isItemInLeftHand)
        {
            animator.SetLayerWeight(attackLayerID, 1);
            animator.SetBool(animationShield, flag);
            animator.SetBool(animationBlock, flag);
        }else
        {
            animator.SetLayerWeight(attackLayerID, 0);
            animator.SetBool(animationBlock, false);
            animator.SetBool(animationShield, false);
        }
    }

    private void Attack()
    {
        try {
            float animationTime = 0f;

            Weapon weapon = handController.GetWeaponInHand(HandType.rightHand)?.GetComponent<Weapon>();
            WeaponsType weaponType = weapon.GetWeaponType();

            animator.SetTrigger(animationAttackHash);
            switch ((int)weaponType)
            {
                case 0:
                    {
                        //Axe animation attack
                        animator.SetLayerWeight(attackLayerID, 0);
                        animator.SetBool("Axe", true);
                        animator.SetBool("Sword", false);
                        animator.SetBool("GreatSword", false);
                        animationTime = animator.GetCurrentAnimatorStateInfo(0).length - 1.2f;
                        break;
                    }
                case 1:
                    {
                        //Sword animation attack
                        animator.SetLayerWeight(attackLayerID, 1);
                        animator.SetBool("Axe", false);
                        animator.SetBool("Sword", true);
                        animator.SetBool("GreatSword", false);
                        animationTime = animator.GetCurrentAnimatorStateInfo(attackLayerID).length;
                        break;
                    }
                case 2:
                    {
                        //Great sword animation attack
                        animator.SetLayerWeight(attackLayerID, 1);
                        animator.SetBool("Axe", false);
                        animator.SetBool("Sword", false);
                        animator.SetBool("GreatSword", true);
                        animationTime = animator.GetCurrentAnimatorStateInfo(attackLayerID).length;
                        break;
                    }
            }

            StartCoroutine(StopAnimationByTime(animationTime));
        }catch (Exception e) { }
    }

    /// <summary>
    /// End attack animation after the time
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns></returns>
    private IEnumerator StopAnimationByTime(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Axe", false);
        animator.SetBool("Sword", false);
        animator.SetBool("GreatSword", false);
        isAttacking = false;
    }
    #endregion

    #region Move  and animation controller: Walk, Run, Jump, Idle;
    private void Move()
    {
        CheckMovemenType();

        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y -= 2;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0, vertical).normalized;

        if (isGrounded)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //    Jump();
            if (direction.magnitude >= 0.1f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    Run();
                else if (!Input.GetKey(KeyCode.LeftShift))
                    Walk();

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.localEulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            else Idle();
        }

        direction = camera.TransformDirection(direction);
        controller.Move(direction * Time.deltaTime * moveSpeed);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Check what move animation nead to play.
    /// </summary>
    private void CheckMovemenType()
    {
        GameObject weapon = handController.GetWeaponInHand(HandType.rightHand);

        if (weapon.GetComponent<Weapon>() != null && weapon.activeInHierarchy)
        {
            HandType handType = weapon.GetComponent<Weapon>().GetHand();
            if (handType == HandType.doubleHand)
                animator.SetBool("IsGreatSword", true);
            else animator.SetBool("IsGreatSword", false);
        }
        else animator.SetBool("IsGreatSword", false);
    }

    private void Idle()
    {
        moveSpeed = 0f;
        animator.SetFloat(animationSpeedHash, 0f, 0.25f, Time.deltaTime);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        animator.SetFloat(animationSpeedHash, 0.5f, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        moveSpeed = runSpeed;
        animator.SetFloat(animationSpeedHash, 1f, 0.1f, Time.deltaTime);
    }

    private void Jump()
    {
        animator.SetTrigger(animationJumpHash);
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }
    #endregion

    private bool wasTakeDamage = false;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Weapon>() != null)
        {
            if (EnemyController.isAttacking && !wasTakeDamage)
            {
                wasTakeDamage = true;
                Weapon weapon = other.gameObject.GetComponent<Weapon>();
                int damage = weapon.damage;

                if (weapon.inHand)
                    Hurt(damage);

                yield return new WaitForSeconds(1f);
                wasTakeDamage = false;
            }
        }
    }
}
