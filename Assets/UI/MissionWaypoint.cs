using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWaypoint : MonoBehaviour
{
    // Các thành phần UI
    public Image img;          // Hình ảnh chỉ đường (icon)
    public Transform target;   // Mục tiêu cần chỉ đến (vị trí, kẻ địch...)
    public Text meter;         // Text hiển thị khoảng cách
    public Vector3 offset;     // Độ lệch so với vị trí mục tiêu

    private void Update()
    {
        // Tính toán giới hạn để icon luôn nằm trong màn hình
        // Giả định điểm neo (anchor) của icon ở giữa hình

        // Tính toán giới hạn X:
        float minX = img.GetPixelAdjustedRect().width / 2;  // Giới hạn trái
        float maxX = Screen.width - minX;                    // Giới hạn phải

        // Tính toán giới hạn Y:
        float minY = img.GetPixelAdjustedRect().height / 2; // Giới hạn dưới
        float maxY = Screen.height - minY;                   // Giới hạn trên

        // Chuyển vị trí 3D của mục tiêu sang tọa độ 2D trên màn hình
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        // Kiểm tra nếu mục tiêu ở phía sau người chơi
        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            // Nếu mục tiêu ở bên trái màn hình
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX; // Hiển thị icon bên phải (ngược lại)
            }
            else
            {
                pos.x = minX; // Hiển thị icon bên trái
            }
        }

        // Giới hạn vị trí icon trong phạm vi màn hình
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // Cập nhật vị trí icon
        img.transform.position = pos;

        // Cập nhật text khoảng cách (làm tròn số và thêm đơn vị 'm')
        meter.text = ((int)Vector3.Distance(target.position, transform.position)).ToString() + "m";
    }
}