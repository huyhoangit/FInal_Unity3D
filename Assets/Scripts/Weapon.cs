using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    // Các thuộc tính liên quan đến vũ khí
    [SerializeField] Camera FPCamera; // Camera góc nhìn thứ nhất (First Person)
    [SerializeField] float range = 100f; // Tầm bắn của súng
    [SerializeField] float damage = 30f; // Sát thương của vũ khí
    [SerializeField] float timeBetweenShots = 0f; // Thời gian chờ giữa các lần bắn

    // Các hiệu ứng và âm thanh khi bắn
    [SerializeField] ParticleSystem muzzleFlash; // Hiệu ứng flash tại nòng súng
    [SerializeField] GameObject hitEffect; // Hiệu ứng va chạm khi trúng mục tiêu
    [SerializeField] Ammo ammoSlot; // Đối tượng chứa thông tin về số lượng đạn
    [SerializeField] AmmoType ammoType; // Loại đạn
    [SerializeField] TextMeshProUGUI ammoText; // UI hiển thị số lượng đạn

    [SerializeField] AudioClip gunSound; // Âm thanh khi bắn
    [SerializeField] AudioClip emptyGunSound; // Âm thanh khi súng hết đạn

    // Các biến để xử lý âm thanh và trạng thái bắn
    AudioSource audioSource;
    bool canShoot = true; // Biến kiểm tra xem súng có thể bắn hay không

    // Khi đối tượng này được kích hoạt (enable), cho phép bắn lại
    private void OnEnable()
    {
        canShoot = true;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Lấy component AudioSource trên đối tượng
    }

    void Update()
    {
        DisplayAmmo(); // Cập nhật số đạn hiển thị trên UI
        // Nếu người chơi nhấn chuột trái và súng có thể bắn
        if (Input.GetMouseButton(0) && canShoot == true)
        {
            StartCoroutine(Shoot()); // Bắt đầu chuỗi công việc bắn súng
        }
    }

    // Hàm hiển thị số đạn còn lại trên UI
    void DisplayAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType); // Lấy số đạn hiện tại
        ammoText.text = currentAmmo.ToString(); // Hiển thị số đạn trên UI
    }

    // Hàm xử lý việc bắn súng
    IEnumerator Shoot()
    {
        canShoot = false; // Đảm bảo súng không bắn liên tục mà không có thời gian nghỉ
        if (ammoSlot.GetCurrentAmmo(ammoType) > 0) // Kiểm tra xem có đạn không
        {
            PlayMuzzleFlash(); // Chạy hiệu ứng flash tại nòng súng
            ProcessRaycast(); // Xử lý bắn theo hướng nhìn
            audioSource.PlayOneShot(gunSound); // Phát âm thanh bắn súng
            ammoSlot.ReduceCurrentAmmo(ammoType); // Giảm số đạn đi 1
        }
        else
        {
            PlayEmptyGunSound(); // Nếu hết đạn, phát âm thanh súng hết đạn
        }
        yield return new WaitForSeconds(timeBetweenShots); // Đợi một khoảng thời gian giữa các lần bắn
        canShoot = true; // Cho phép bắn lại
    }

    // Phát âm thanh khi súng hết đạn
    private void PlayEmptyGunSound()
    {
        audioSource.PlayOneShot(emptyGunSound); // Phát âm thanh hết đạn
    }

    // Chạy hiệu ứng flash tại nòng súng khi bắn
    void PlayMuzzleFlash()
    {
        muzzleFlash.Play(); // Phát hiệu ứng flash nòng súng
    }

    // Xử lý việc bắn với raycast (tia sáng)
    void ProcessRaycast()
    {
        RaycastHit hit; // Lưu trữ thông tin khi raycast va chạm
        // Bắn một tia từ vị trí camera, hướng về phía trước trong phạm vi nhất định
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            // Tạo hiệu ứng va chạm khi trúng đối tượng
            CreateHitImpact(hit);
            // Kiểm tra xem có đối tượng EnemyHealth không
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target == null) { return; } // Nếu không phải kẻ thù, thoát hàm
            target.TakeDamage(damage); // Gọi hàm nhận sát thương của kẻ thù
        }
        else
        {
            return; // Nếu không trúng gì, thoát hàm
        }
    }

    // Tạo hiệu ứng va chạm khi raycast trúng đối tượng
    private void CreateHitImpact(RaycastHit hit)
    {
        // Tạo hiệu ứng va chạm tại vị trí trúng
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 0.1f); // Hủy hiệu ứng sau 0.1 giây
    }
}
