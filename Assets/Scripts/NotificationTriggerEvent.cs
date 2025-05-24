using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NotificationTriggerEvent : MonoBehaviour
{
    [Header("UI Content")]
    [SerializeField] TextMeshProUGUI notificationText;  // TextMeshPro dùng để hiển thị thông báo trên UI

    [Header("Message Customization")]
    [SerializeField][TextArea] string notificationMessage; // Nội dung thông báo sẽ hiển thị

    [Header("Notification Removal")]
    [SerializeField] bool removeafterExit = false;  // Xác định xem thông báo có bị ẩn khi người chơi rời đi không
    [SerializeField] bool disableAfterTimer = false; // Xác định xem thông báo có tự động ẩn sau thời gian không
    [SerializeField] float disableTimer = 0.1f; // Thời gian chờ trước khi tự động ẩn thông báo

    [Header("Notification Animation")]
    [SerializeField] Animator notificationAnim;  // Animator dùng để xử lý hiệu ứng hiển thị và ẩn thông báo

    BoxCollider objectCollider;  // Collider để phát hiện khi người chơi bước vào vùng trigger

    private void Awake()
    {
        // Lấy component BoxCollider gắn trên object này
        objectCollider = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu là player và có thiết lập tự động ẩn khi ra khỏi vùng
        if (other.CompareTag("Player") && removeafterExit)
        {
            // Bắt đầu coroutine để hiển thị thông báo
            StartCoroutine(EnableNotification());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Kiểm tra khi player rời khỏi vùng trigger và tự động ẩn thông báo nếu cần
        if (other.CompareTag("Player") && removeafterExit)
        {
            RemoveNotification();
        }
    }

    // Coroutine xử lý hiển thị thông báo
    IEnumerator EnableNotification()
    {
        // Tắt collider để tránh việc trigger lại nhiều lần khi player vẫn còn trong vùng
        objectCollider.enabled = false;

        // Bắt đầu hiệu ứng Fade In
        notificationAnim.Play("Fade In");

        // Cập nhật nội dung thông báo lên UI
        notificationText.text = notificationMessage;

        // Nếu thiết lập có hẹn giờ tắt thông báo sau một thời gian
        if (disableAfterTimer)
        {
            // Chờ một khoảng thời gian trước khi tắt thông báo
            yield return new WaitForSeconds(disableTimer);
            RemoveNotification();
        }
    }

    // Hàm xử lý ẩn thông báo
    private void RemoveNotification()
    {
        // Bắt đầu hiệu ứng Fade Out
        notificationAnim.Play("Fade Out");

        // Tắt object này đi (thay vì dùng `gameObject.active = false`, ta dùng `SetActive(false)`)
        gameObject.SetActive(false); // Dùng SetActive thay vì .active để tắt object trong Unity
    }
}
