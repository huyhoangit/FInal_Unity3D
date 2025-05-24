using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // Các biến cấu hình
    [SerializeField] float hitPoints = 400f; // Máu tối đa của nhân vật
    [SerializeField] TextMeshProUGUI healthBar; // UI hiển thị máu
    [SerializeField] AudioClip deathSound; // Âm thanh khi chết
    [SerializeField] AudioClip hitSound; // Âm thanh khi bị đánh

    AudioSource audioSource; // Nguồn phát âm thanh

    private void Start()
    {
        // Lấy component AudioSource khi bắt đầu
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Cập nhật thanh máu mỗi frame
        DisplayHealth();
    }

    // Hiển thị máu lên UI
    public void DisplayHealth()
    {
        // Tính máu hiện tại (chia 4 để hiển thị %, vd: 400 máu = 100%)
        float currentHealth = hitPoints / 4;
        healthBar.text = currentHealth.ToString();
    }

    // Hàm xử lý khi nhân vật bị đánh
    public void TakeDamage(float damage)
    {
        // Trừ máu khi bị đánh
        hitPoints -= damage;
        // Phát âm thanh bị đánh
        audioSource.PlayOneShot(hitSound);

        // Kiểm tra nếu máu <= 0 thì xử lý chết
        if (hitPoints <= 0)
        {
            audioSource.PlayOneShot(deathSound);
            // Gọi hàm xử lý cái chết từ script DeathHandler
            GetComponent<DeathHandler>().HandleDeath();
        }
    }

    // Hồi phục máu đầy (khi hồi sinh hoặc nhận item máu)
    public void GiveHealth()
    {
        hitPoints = 400f; // Đặt lại máu tối đa
    }
}