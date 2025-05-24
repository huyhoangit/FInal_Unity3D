using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunsMenu : MonoBehaviour
{
    public GameObject Buttons;  // Tham chiếu đến đối tượng chứa các nút trên menu
    public GameObject[] Guns;   // Mảng chứa các khẩu súng mà người chơi có thể chọn
    int currentGun = 0;         // Biến lưu trữ khẩu súng hiện tại mà người chơi đang sử dụng

    void Start()
    {
        // Ban đầu, chỉ súng đầu tiên trong danh sách được hiển thị
        Guns[0].SetActive(true);
    }

    // Hàm chuyển sang súng tiếp theo trong danh sách
    public void NextGun()
    {
        // Tắt khẩu súng hiện tại
        Guns[currentGun].SetActive(false);
        // Tăng chỉ số súng hiện tại
        currentGun++;
        // Nếu chỉ số súng vượt quá phạm vi, quay lại súng đầu tiên
        if (currentGun >= Guns.Length)
            currentGun = 0;
        // Bật khẩu súng mới
        Guns[currentGun].SetActive(true);
    }

    // Hàm chuyển về súng trước đó trong danh sách
    public void PreviousGun()
    {
        // Tắt khẩu súng hiện tại
        Guns[currentGun].SetActive(false);
        // Giảm chỉ số súng hiện tại
        currentGun--;
        // Nếu chỉ số súng dưới 0, quay lại súng cuối cùng
        if (currentGun < 0)
            currentGun = Guns.Length - 1;
        // Bật khẩu súng mới
        Guns[currentGun].SetActive(true);
    }

    private void Update()
    {
        // Kiểm tra xem người chơi có nhấn chuột trái hoặc chạm vào màn hình không và không chạm vào các nút UI
        if ((Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
        {
            // Ẩn các nút trên menu nếu người chơi nhấn chuột trái hoặc chạm vào màn hình
            Buttons.SetActive(false);
        }
        else if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            // Nếu không có thao tác chạm hoặc nhấn chuột, hiện lại các nút menu
            Buttons.SetActive(true);
        }
    }
}
