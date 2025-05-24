using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ScriptableObject này dùng để chứa dữ liệu cho thông báo trong game (ví dụ: hướng dẫn, gợi ý, cảnh báo...)
[CreateAssetMenu(fileName = "NotificationSc")] // Cho phép tạo asset này trong Unity bằng cách click chuột phải
public class NotifcationScriptable : ScriptableObject
{
    [Header("Message Customization")]
    // Nội dung thông báo sẽ hiển thị trên màn hình (có thể là đoạn văn ngắn)
    [SerializeField][TextArea] public string notificationMessage;

    [Header("Notification Removal")]
    // Nếu bật tùy chọn này, thông báo sẽ biến mất sau khi người chơi rời khỏi vùng trigger
    [SerializeField] public bool removeAfterExit = false;

    // Nếu bật cái này, thông báo sẽ tự động tắt sau một khoảng thời gian
    [SerializeField] public bool disableAfterTimer = false;

    // Khoảng thời gian để ẩn thông báo nếu disableAfterTimer = true
    [SerializeField] public float disableTimer = 0.1f;
}
