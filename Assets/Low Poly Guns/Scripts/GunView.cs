using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunView : MonoBehaviour
{
    // Kích thước cơ bản của vũ khí
    Vector3 baseScale = new Vector3(0.7f, 0.7f, 0.7f);

    // Biến để kiểm tra xem người chơi đang sử dụng điện thoại hay không, và để theo dõi trạng thái lần đầu tiên
    bool isPhone = false, isFirst = true;

    // Góc quay trên trục Z và Y
    float zRot = 0, yRot = 0;

    // Vị trí bắt đầu của thao tác chạm hoặc di chuyển chuột
    Vector3 startPos;

    // Khi đối tượng được bật lên
    private void OnEnable()
    {
        transform.localScale = baseScale; // Đặt lại kích thước ban đầu của vũ khí
        if (SystemInfo.deviceType == DeviceType.Handheld) // Kiểm tra xem thiết bị có phải là điện thoại không
            isPhone = true;
        isFirst = true; // Đặt lại trạng thái lần đầu tiên
    }

    void Update()
    {
        // Nếu là điện thoại, xử lý sự kiện cảm ứng
        if (isPhone)
        {
            if (Input.touchCount > 0) // Kiểm tra xem có thao tác chạm nào không
            {
                if (isFirst)
                {
                    isFirst = false;
                    startPos = Input.GetTouch(0).position; // Lưu vị trí chạm đầu tiên
                    return;
                }
                else
                {
                    // Tính toán góc quay theo trục Z (dọc) và Y (ngang) từ sự thay đổi vị trí chạm
                    zRot += (Input.GetTouch(0).position.y - startPos.y) * 0.09f;
                    zRot = Mathf.Clamp(zRot, -55, 55); // Giới hạn góc quay Z
                    yRot += (Input.GetTouch(0).position.x - startPos.x) * 0.25f;
                    // Không giới hạn góc quay Y để cho phép xoay tự do
                    startPos = Input.GetTouch(0).position; // Cập nhật vị trí chạm mới
                }
                ScaleUp(); // Tăng kích thước vũ khí khi có thao tác
            }
            else
            {
                // Nếu không có thao tác chạm, quay về trạng thái ban đầu
                isFirst = true;
                zRot = Mathf.Lerp(zRot, 0, 0.08f); // Làm mềm quay về góc ban đầu
                yRot = Mathf.Lerp(yRot, 0, 0.08f);
                ScaleDown(); // Giảm kích thước vũ khí
            }
            // Áp dụng các góc quay cho vũ khí
            transform.eulerAngles = new Vector3(0, 90 + yRot, zRot);
        }
        else // Nếu là máy tính, xử lý sự kiện chuột
        {
            if (Input.GetMouseButton(0)) // Kiểm tra nếu chuột trái được nhấn
            {
                if (isFirst)
                {
                    isFirst = false;
                    startPos = Input.mousePosition; // Lưu vị trí chuột ban đầu
                    return;
                }
                else
                {
                    // Tính toán góc quay theo trục Z (dọc) và Y (ngang) từ sự thay đổi vị trí chuột
                    zRot += (Input.mousePosition.y - startPos.y) * 0.1f;
                    zRot = Mathf.Clamp(zRot, -55, 55); // Giới hạn góc quay Z
                    yRot += (Input.mousePosition.x - startPos.x) * 0.3f;
                    // Không giới hạn góc quay Y
                    startPos = Input.mousePosition; // Cập nhật vị trí chuột mới
                }
                ScaleUp(); // Tăng kích thước vũ khí khi có thao tác chuột
            }
            else
            {
                // Nếu không có thao tác chuột, quay về trạng thái ban đầu
                isFirst = true;
                zRot = Mathf.Lerp(zRot, 0, 0.2f); // Làm mềm quay về góc ban đầu
                yRot = Mathf.Lerp(yRot, 0, 0.2f);
                ScaleDown(); // Giảm kích thước vũ khí
            }
            // Áp dụng các góc quay cho vũ khí
            transform.eulerAngles = new Vector3(0, 90 + yRot, zRot);
        }
    }

    // Tăng kích thước vũ khí
    void ScaleUp()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.075f);
    }

    // Giảm kích thước vũ khí về kích thước ban đầu
    void ScaleDown()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, baseScale, 0.075f);
    }
}
