using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    // Tốc độ giảm cường độ sáng của đèn theo thời gian                           //
    [SerializeField] float lightDecay = 0.1f;                                     //
                                                                                  //
    // Tốc độ giảm góc chiếu sáng theo thời gian                                  //  ---> biến thành viên để giảm dần cường độ và góc chiếu của đèn pin.
    [SerializeField] float angleDecay = 1f;                                       //
                                                                                  //
    // Giới hạn nhỏ nhất của góc chiếu sáng, đèn sẽ không thu hẹp hơn mức này     //
    [SerializeField] float minimumAngle = 40f;                                    //

    // Biến lưu trữ component Light của GameObject
    Light light;

    private void Start()
    {
        // Lấy component Light gắn trên GameObject này
        light = GetComponent<Light>();
    }

    // Phục hồi góc chiếu sáng về một giá trị cụ thể
    public void RestoreLightAngle(float restoreAngle)
    {
        light.spotAngle = restoreAngle;
    }

    // Tăng cường độ sáng của đèn lên một giá trị nhất định
    public void AddLightIntensity(float intensiyAmount)
    {
        light.intensity += intensiyAmount;
    }

    private void Update()
    {
        // Giảm dần góc chiếu sáng
        DecreaseLightAngle();

        // Giảm dần cường độ sáng
        DecreaseLightIntensity();
    }

    private void DecreaseLightAngle()
    {
        // Nếu góc chiếu sáng đã nhỏ hơn hoặc bằng mức tối thiểu, dừng lại
        if (light.spotAngle <= minimumAngle) return;

        // Giảm dần góc chiếu sáng theo thời gian
        light.spotAngle -= angleDecay * Time.deltaTime;
    }

    private void DecreaseLightIntensity()
    {
        // Giảm dần cường độ sáng theo thời gian, nhưng không để nó xuống dưới 0
        light.intensity = Mathf.Max(light.intensity - lightDecay * Time.deltaTime, 0);
    }
}
