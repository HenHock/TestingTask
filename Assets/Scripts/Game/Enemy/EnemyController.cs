using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    /*
     * Script describe behavior of enemy: patroling, chasing and attack.
     * 
     * I use NavMeshAgent for control movement
     */

    [SerializeField] private int health;
    [SerializeField, Range(1f, 10f)] private float radiusDetectPlayer;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private ProgressBar healthBar;

    private Transform target;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] private Vector3 walkPoint;

    private bool isPlayerInAttackRange = false;
    private bool isPlayerDetected = false;
    private bool isWalkinPoint = false;

    // Animation parametres
    private int animationAttackHash;
    private int animationSpeedHash;

    public static bool isAttacking { get; private set; } = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        animationAttackHash = Animator.StringToHash("Attack");
        animationSpeedHash = Animator.StringToHash("Speed");

        healthBar.Initialize(health);
    }

    private void Update()
    {
        if (!isPlayerDetected)
        {
            Patroling();
            DetectePlayer();
        }
        else if (isPlayerDetected && !isPlayerInAttackRange)
            Chasing();
        else if (isPlayerDetected && isPlayerInAttackRange)
            Attack();
    }

    private void DetectePlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusDetectPlayer, playerLayerMask);
        if (colliders.Length > 0)
        {
            target = colliders[0].transform;
            isPlayerDetected = true;
        }
    }

    public void Hurt(float damage)
    {
        healthBar.Value -= damage.ToPercent(healthBar.MaxValue);

        if(healthBar.Value <= 0.1f)
            Dead();
    }

    private void Dead()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UIManager.SetActivePanel(PanelType.endGamePanel, true);
    }


    private void Attack()
    {
        Chasing();

        if (target != null)
        {
            //Rotate to the player
            var newRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.forward);
            newRotation.x = 0.0f;
            newRotation.z = 0.0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 2);

            isAttacking = true;
            animator.SetFloat(animationSpeedHash, 0, 0.1f, Time.deltaTime);
            animator.SetTrigger(animationAttackHash);
        }
    }

    private bool wasTakeDamage = false;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Weapon>() != null)
        {
            if (PlayerController.isAttacking && !wasTakeDamage)
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

    private void Chasing()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance >= 2f)
            {
                isAttacking = false;
                isPlayerInAttackRange = false;
                agent.SetDestination(target.position);
                animator.SetFloat(animationSpeedHash, 0.5f, 0.1f, Time.deltaTime);
            }
            else if (!isPlayerInAttackRange)
            {
                agent.SetDestination(transform.position);
                isPlayerInAttackRange = true;
            }
        }
    }
    /// <summary>
    /// Set patroling enemy to random point on the map
    /// </summary>
    private void Patroling()
    {
        if (!isWalkinPoint)
        {
           StartCoroutine(GeneratePoint(3f));
        }
        else
        {
            animator.SetFloat(animationSpeedHash, 0.5f, 0.1f, Time.deltaTime);
        }

        Vector3 distance = transform.position - walkPoint;
        if(distance.magnitude <= 1f)
            isWalkinPoint = false;
    }
    /// <summary>
    /// Generate the random point to patroling after delay
    /// </summary>
    /// <param name="time">Delay</param>
    /// <returns></returns>
    private IEnumerator GeneratePoint(float time)
    {
        yield return new WaitForSeconds(time);
        float randoX = Random.Range(-10f, 10f);
        float randoZ = Random.Range(-10f, 10f);

        walkPoint = new Vector3(transform.position.x + randoX, transform.position.y, transform.position.z + randoZ);

        if (SetDestination(walkPoint))
            isWalkinPoint = true;
    }

    /// <summary>
    /// Draw in the develope mode wire sphere for radius in which player able to be detected
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusDetectPlayer);
    }
    /// <summary>
    /// Check if point is on navmesh and set distination to targetDestination
    /// </summary>
    /// <param name="targetDestination"></param>
    /// <returns>True if point is in the navmesh (Set distination). False if point is not in the navmesh (Does't set distination).</returns>
    private bool SetDestination(Vector3 targetDestination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            return true;
        }
        return false;
    }
}
