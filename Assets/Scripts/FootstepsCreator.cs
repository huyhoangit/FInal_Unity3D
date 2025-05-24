using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsCreator : MonoBehaviour
{
    public GameObject footstep; // GameObject chứa hiệu ứng bước chân (particle system hoặc âm thanh)

    void Start()
    {
        footstep.SetActive(false); // Tắt hiệu ứng ngay khi bắt đầu
    }

    void Update()
    {
        // Kiểm tra input di chuyển
        if (Input.GetKey("w")) // Di chuyển tiến (giữ phím)
        {
            footsteps();
        }

        if (Input.GetKeyDown("s")) // Bắt đầu di chuyển lùi
        {
            footsteps();
        }

        if (Input.GetKeyDown("a")) // Bắt đầu di chuyển trái
        {
            footsteps();
        }

        if (Input.GetKeyDown("d")) // Bắt đầu di chuyển phải
        {
            footsteps();
        }

        // Kiểm tra khi ngừng di chuyển
        if (Input.GetKeyUp("w")) // Ngừng di chuyển tiến
        {
            StopFootsteps();
        }

        if (Input.GetKeyUp("s")) // Ngừng di chuyển lùi
        {
            StopFootsteps();
        }

        if (Input.GetKeyUp("a")) // Ngừng di chuyển trái
        {
            StopFootsteps();
        }

        if (Input.GetKeyUp("d")) // Ngừng di chuyển phải
        {
            StopFootsteps();
        }
    }

    // Kích hoạt hiệu ứng bước chân
    void footsteps()
    {
        footstep.SetActive(true); // Bật hiệu ứng
    }

    // Tắt hiệu ứng bước chân
    void StopFootsteps()
    {
        footstep.SetActive(false); // Tắt hiệu ứng
    }
}