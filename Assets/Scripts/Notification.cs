using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// Script này dùng để hiển thị thông báo khi người chơi đi vào một vùng nào đó (dạng pop-up UI)
public class Notification : MonoBehaviour
{
    [Header("UI Content")]
    // Text UI để hiển thị nội dung thông báo
    [SerializeField] TextMeshProUGUI notificationText;

    [Header("Scriptable Object")]
    // ScriptableObject chứa nội dung và cách hiển thị thông báo (được tạo sẵn trong Inspector)
    [SerializeField] NotifcationScriptable notification;

    [Header("Notification Animation")]
    // Animator để điều khiển hiệu ứng hiển thị và ẩn thông báo
    [SerializeField] Animator notificationAnim;

    // Collider của object để kiểm tra va chạm với người chơi
    BoxCollider objectCollider;

    private void Awake()
    {
        // Lấy BoxCollider của object này để sau có thể bật/tắt khi cần
        objectCollider = gameObject.GetComponent<BoxCollider>();
    }

    // Khi người chơi bước vào vùng có trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Bắt đầu hiển thị thông báo
            StartCoroutine(EnableNotification());
        }
    }

    // Khi người chơi bước ra khỏi vùng trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && notification.removeAfterExit)
        {
            // Ẩn thông báo nếu cài đặt là tự xóa khi rời khỏi
            RemoveNotification();
        }
    }

    // Coroutine này xử lý việc hiển thị thông báo
    IEnumerator EnableNotification()
    {
        // Nếu muốn, bạn có thể tắt collider để không bị trigger nhiều lần
        // objectCollider.enabled = false;

        // Chạy animation "Fade In" để hiển thị thông báo
        notificationAnim.Play("Fade In");

        // Gán nội dung thông báo từ ScriptableObject
        notificationText.text = notification.notificationMessage;

        // Nếu được cấu hình là tự động ẩn sau một thời gian, thì chờ rồi gọi ẩn
        if (notification.disableAfterTimer)
        {
            yield return new WaitForSeconds(notification.disableTimer);
            RemoveNotification();
        }
    }

    // Hàm để ẩn thông báo bằng animation "Fade Out"
    private void RemoveNotification()
    {
        notificationAnim.Play("Fade Out");

        // Nếu muốn tắt luôn object chứa notification sau khi ẩn, có thể dùng:
        // gameObject.SetActive(false);
    }
}
