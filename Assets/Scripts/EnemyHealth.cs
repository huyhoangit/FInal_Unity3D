using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Các thông số cấu hình
    [SerializeField] float hitPoints = 100f;    // Máu tối đa của kẻ địch
    [SerializeField] AudioClip deathSound;     // Âm thanh khi chết
    [SerializeField] AudioClip hitSound;       // Âm thanh khi bị đánh trúng

    AudioSource audioSource;                   // Thành phần phát âm thanh
    bool isDead = false;                       // Trạng thái sống/chết

    private void Start()
    {
        // Lấy component AudioSource khi bắt đầu
        audioSource = GetComponent<AudioSource>();
    }

    // Hàm kiểm tra trạng thái sống/chết
    public bool IsDead()
    {
        return isDead;
    }

    // Hàm xử lý khi nhận sát thương
    public void TakeDamage(float damage)
    {
        // Thông báo cho các script khác biết kẻ địch bị tấn công
        BroadcastMessage("OnDamageTaken");

        // Trừ máu
        hitPoints -= damage;

        // Phát âm thanh bị đánh trúng
        audioSource.PlayOneShot(hitSound);

        // Kiểm tra nếu máu <= 0 thì chết
        if (hitPoints <= 0)
        {
            Die();
        }
    }

    // Hàm xử lý khi chết
    private void Die()
    {
        // Nếu đã chết rồi thì không xử lý lại
        if (isDead) { return; }

        // Đánh dấu trạng thái chết
        isDead = true;

        // Phát âm thanh chết
        audioSource.PlayOneShot(deathSound);

        // Kích hoạt animation chết
        GetComponent<Animator>().SetTrigger("Die");

        // Có thể thêm các xử lý khác ở đây như:
        // - Tắt collider
        // - Thêm hiệu ứng particle
        // - Rơi đồ...
    }
}