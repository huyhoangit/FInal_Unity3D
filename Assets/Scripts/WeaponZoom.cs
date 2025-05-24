using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponZoom : MonoBehaviour
{
    // Các tham số cấu hình trong Inspector
    [SerializeField] Camera fpsCamera; // Camera góc nhìn thứ nhất
    [SerializeField] RigidbodyFirstPersonController firstPersonController; // Điều khiển nhân vật góc nhìn thứ nhất

    [SerializeField] float zoomedOutFOV = 60f; // FOV (Field of View) khi không zoom
    [SerializeField] float zoomedInFOV = 20f; // FOV khi zoom vào
    [SerializeField] float zoomedOutSensivity = 2f; // Độ nhạy chuột khi không zoom
    [SerializeField] float zoomedInSensivity = 0.5f; // Độ nhạy chuột khi zoom vào

    bool zoomedInToogle = false; // Biến kiểm tra xem có đang zoom vào hay không

    // Khi đối tượng bị vô hiệu hóa, trả lại trạng thái ban đầu (zoom out)
    private void OnDisable()
    {
        ZoomOut(); // Đảm bảo khi đối tượng này bị vô hiệu hóa, zoom sẽ được thoát ra
    }

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn chuột phải (phím Mouse Button 1) không
        if (Input.GetMouseButtonDown(1))
        {
            if (!zoomedInToogle) // Nếu hiện tại không phải đang zoom vào
            {
                ZoomIn(); // Thực hiện zoom vào
            }
            else if (zoomedInToogle) // Nếu đang zoom vào
            {
                ZoomOut(); // Thực hiện zoom ra
            }
        }
    }

    // Hàm thực hiện zoom out
    private void ZoomOut()
    {
        zoomedInToogle = false; // Đánh dấu trạng thái không còn zoom vào
        fpsCamera.fieldOfView = zoomedOutFOV; // Đặt lại FOV về giá trị không zoom
        // Đặt lại độ nhạy chuột khi không zoom
        firstPersonController.mouseLook.XSensitivity = zoomedOutSensivity;
        firstPersonController.mouseLook.YSensitivity = zoomedOutSensivity;
    }

    // Hàm thực hiện zoom in
    private void ZoomIn()
    {
        zoomedInToogle = true; // Đánh dấu trạng thái đang zoom vào
        fpsCamera.fieldOfView = zoomedInFOV; // Thay đổi FOV để zoom vào
        // Đặt lại độ nhạy chuột khi zoom vào
        firstPersonController.mouseLook.XSensitivity = zoomedInSensivity;
        firstPersonController.mouseLook.YSensitivity = zoomedInSensivity;
    }
}
