using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] int currentWeapon = 0; // Chỉ số vũ khí hiện tại được chọn

    [SerializeField] AudioClip weaponSwitchSound; // Âm thanh phát khi chuyển đổi vũ khí

    AudioSource audioSource; // Thành phần AudioSource để phát âm thanh

    void Start()
    {
        // Lấy component AudioSource từ GameObject này
        audioSource = GetComponent<AudioSource>();
        // Kích hoạt vũ khí đầu tiên khi bắt đầu game
        SetWeaponActive();
    }

    void Update()
    {
        int previousWeapon = currentWeapon; // Lưu vũ khí hiện tại trước khi xử lý

        // Xử lý đầu vào từ bàn phím và cuộn chuột
        ProceesKeyInput();
        ProceesScrollWheel();

        // Nếu vũ khí đã thay đổi, cập nhật trạng thái vũ khí
        if (previousWeapon != currentWeapon)
        {
            SetWeaponActive(); // Kích hoạt vũ khí mới được chọn
        }
    }

    // Xử lý chuyển đổi vũ khí bằng cuộn chuột
    void ProceesScrollWheel()
    {
        // Nếu cuộn chuột xuống
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            audioSource.PlayOneShot(weaponSwitchSound); // Phát âm thanh chuyển đổi vũ khí
            // Kiểm tra nếu vũ khí hiện tại là vũ khí cuối cùng
            if (currentWeapon >= transform.childCount - 1)
            {
                currentWeapon = 0; // Quay lại vũ khí đầu tiên
            }
            else
            {
                currentWeapon++; // Chọn vũ khí tiếp theo
            }
        }

        // Nếu cuộn chuột lên
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            audioSource.PlayOneShot(weaponSwitchSound); // Phát âm thanh chuyển đổi vũ khí
            // Kiểm tra nếu vũ khí hiện tại là vũ khí đầu tiên
            if (currentWeapon <= 0)
            {
                currentWeapon = transform.childCount - 1; // Chọn vũ khí cuối cùng
            }
            else
            {
                currentWeapon--; // Chọn vũ khí trước đó
            }
        }
    }

    // Xử lý nhấn phím số để chọn vũ khí
    void ProceesKeyInput()
    {
        // Kiểm tra nhấn phím số từ 1 đến 3 để chuyển đổi vũ khí
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0; // Chọn vũ khí đầu tiên
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1; // Chọn vũ khí thứ hai
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2; // Chọn vũ khí thứ ba
        }
    }

    // Kích hoạt vũ khí hiện tại và vô hiệu hóa các vũ khí khác
    void SetWeaponActive()
    {
        int weaponIndex = 0; // Biến chỉ mục để duyệt qua các vũ khí

        // Duyệt qua tất cả các con trong transform (vũ khí)
        foreach (Transform weapon in transform)
        {
            // Nếu chỉ mục vũ khí bằng chỉ số vũ khí hiện tại
            if (weaponIndex == currentWeapon)
            {
                weapon.gameObject.SetActive(true); // Kích hoạt vũ khí hiện tại
            }
            else
            {
                weapon.gameObject.SetActive(false); // Vô hiệu hóa vũ khí không được chọn
            }

            weaponIndex++; // Tăng chỉ mục cho vũ khí tiếp theo
        }
    }
}
