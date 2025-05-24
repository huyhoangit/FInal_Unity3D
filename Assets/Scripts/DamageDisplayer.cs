using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDisplayer : MonoBehaviour
{
    [SerializeField] Canvas impactCanvas; // Canvas hiển thị hiệu ứng máu khi bị trúng đạn
    [SerializeField] float impactTime = 0.3f; // Thời gian hiển thị hiệu ứng (giây)

    void Start()
    {
        // Tắt canvas hiệu ứng ngay từ đầu
        impactCanvas.enabled = false;
    }

    // Hàm công khai để kích hoạt hiệu ứng máu
    public void ShowDamageImpact()
    {
        StartCoroutine(ShowSplatter());
    }

    // Coroutine xử lý hiển thị hiệu ứng trong khoảng thời gian ngắn
    IEnumerator ShowSplatter()
    {
        impactCanvas.enabled = true; // Bật hiệu ứng
        yield return new WaitForSeconds(impactTime); // Chờ trong khoảng thời gian impactTime
        impactCanvas.enabled = false; // Tắt hiệu ứng
    }
}