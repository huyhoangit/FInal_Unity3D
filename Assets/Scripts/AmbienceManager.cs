using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script này dùng để phát các đoạn âm thanh môi trường (ambience) một cách tuần tự
public class AmbienceManager : MonoBehaviour
{
    // Mảng chứa các đoạn âm thanh sẽ được phát lần lượt
    [SerializeField]
    AudioClip[] adClips;

    // Biến để điều khiển việc phát âm thanh
    AudioSource audioSource;

    // Khi game bắt đầu chạy
    private void Start()
    {
        // Lấy AudioSource gắn trên GameObject này để dùng
        audioSource = GetComponent<AudioSource>();

        // Bắt đầu chạy hàm phát âm thanh theo thứ tự
        StartCoroutine(PlayAudioSequentially());
    }

    // Hàm này phát từng đoạn âm thanh một cách tuần tự
    IEnumerator PlayAudioSequentially()
    {
        // Chờ một frame trước khi bắt đầu (cho chắc ăn)
        yield return null;

        // Duyệt qua từng đoạn âm thanh trong danh sách
        for (int i = 0; i < adClips.Length; i++)
        {
            // Gán đoạn âm thanh hiện tại vào AudioSource
            audioSource.clip = adClips[i];

            // Bắt đầu phát đoạn âm thanh
            audioSource.Play();

            // Chờ đến khi đoạn âm thanh phát xong
            while (audioSource.isPlaying)
            {
                // Mỗi frame đều kiểm tra xem đã phát xong chưa
                yield return null;
            }

            // Sau khi đoạn này phát xong, nó sẽ tự động chuyển sang đoạn tiếp theo
        }
    }
}
