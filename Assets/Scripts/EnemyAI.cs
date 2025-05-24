using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Các thông số cấu hình
    [SerializeField] float chaseRange = 5f;     // Phạm vi phát hiện và đuổi theo player
    [SerializeField] float turnSpeed = 5f;      // Tốc độ xoay hướng
    [SerializeField] AudioClip walkingSound;    // Âm thanh khi di chuyển

    // Các thành phần tham chiếu
    NavMeshAgent navMeshAgent;                  // Thành phần điều hướng
    float distanceToTarget = Mathf.Infinity;    // Khoảng cách tới mục tiêu
    AudioSource audioSource;                    // Nguồn phát âm thanh
    bool isProvoked = false;                    // Trạng thái bị khiêu khích

    EnemyHealth health;                         // Máu của kẻ địch
    Transform target;                           // Mục tiêu (player)

    void Start()
    {
        // Lấy các component cần thiết khi khởi động
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        target = FindObjectOfType<PlayerHealth>().transform; // Tìm player
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Nếu kẻ địch đã chết thì tắt script này
        if (health.IsDead())
        {
            enabled = false;
            navMeshAgent.enabled = false;
            return;
        }

        // Tính khoảng cách tới player
        distanceToTarget = Vector3.Distance(target.position, transform.position);

        // Xử lý hành vi theo trạng thái
        if (isProvoked)
        {
            EngageTarget(); // Tấn công nếu bị khiêu khích
        }
        else if (distanceToTarget <= chaseRange) // Nếu player vào phạm vi đuổi bắt
        {
            isProvoked = true;
            // Có thể thêm âm thanh báo động ở đây
        }
    }

    // Hàm này được gọi khi kẻ địch bị tấn công
    public void OnDamageTaken()
    {
        isProvoked = true; // Kích hoạt chế độ truy đuổi
    }

    // Xử lý tương tác với mục tiêu
    void EngageTarget()
    {
        FaceTarget(); // Luôn hướng về phía player

        // Nếu còn cách xa thì đuổi theo
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }
        // Nếu đủ gần thì tấn công
        else if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }

    // Hàm tấn công mục tiêu
    void AttackTarget()
    {
        GetComponent<Animator>().SetBool("Attack", true); // Kích hoạt animation tấn công
    }

    // Hàm đuổi theo mục tiêu
    void ChaseTarget()
    {
        // Kích hoạt animation di chuyển
        GetComponent<Animator>().SetBool("Attack", false);
        GetComponent<Animator>().SetTrigger("Move");

        // Di chuyển tới vị trí player
        navMeshAgent.SetDestination(target.position);
    }

    // Hàm xoay hướng về phía mục tiêu
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    // Vẽ phạm vi đuổi bắt trong Scene view để dễ debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}