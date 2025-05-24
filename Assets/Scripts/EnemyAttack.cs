using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Tham chiếu đến máu của người chơi
    PlayerHealth target;

    // Các thông số có thể điều chỉnh trong Inspector
    [SerializeField] float damage = 40f; // Lượng sát thương gây ra
    [SerializeField] AudioClip provokedSound; // Âm thanh khi tấn công
    [SerializeField] AudioClip idleSound; // Âm thanh khi idle (rảnh rỗi)

    AudioSource audioSource; // Thành phần phát âm thanh

    void Start()
    {
        // Tìm đối tượng PlayerHealth trong scene
        target = FindObjectOfType<PlayerHealth>();
        // Lấy component AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    // Hàm này được gọi từ Animation Event (sự kiện trong animation)
    public void AttackHitEvent()
    {
        // Kiểm tra nếu không có mục tiêu thì thoát
        if (target == null) { return; }

        // Gây sát thương cho người chơi
        target.TakeDamage(damage);
        // Phát âm thanh tấn công (volume giảm còn 20%)
        audioSource.PlayOneShot(provokedSound, 0.2f);
        // Hiển thị hiệu ứng bị đánh trúng trên màn hình
        target.GetComponent<DamageDisplayer>().ShowDamageImpact();
    }

    // Hàm phát âm thanh idle (có thể gọi từ animation hoặc ngẫu nhiên)
    public void ZombieIdleGrowl()
    {
        // Phát âm thanh idle (volume 50%)
        audioSource.PlayOneShot(idleSound, 0.5f);
    }
}