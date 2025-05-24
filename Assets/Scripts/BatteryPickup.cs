using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script này gắn vào vật phẩm pin (battery), khi người chơi nhặt sẽ giúp hồi sáng cho đèn pin và hồi máu
public class BatteryPickup : MonoBehaviour
{
    // Góc sáng của đèn pin sẽ được khôi phục (mở rộng lại)
    [SerializeField] float restoreAngle = 90f;

    // Mức độ tăng cường độ sáng cho đèn pin
    [SerializeField] float addIntensity = 1f;

    // Âm thanh phát ra khi người chơi nhặt pin
    [SerializeField] AudioClip batteryPickupSound;

    // Tham chiếu đến script PlayerHealth để hồi máu cho người chơi
    PlayerHealth player;

    // AudioSource để phát âm thanh nhặt pin
    AudioSource audioSource;

    private void Awake()
    {
        // Lấy AudioSource từ đối tượng cha (giả sử âm thanh được quản lý ở cấp cao hơn)
        audioSource = GetComponentInParent<AudioSource>();

        // Tìm đối tượng PlayerHealth trong scene (chỉ nên có một player chính)
        player = FindObjectOfType<PlayerHealth>();
    }

    // Khi có đối tượng khác đi vào vùng trigger (vùng va chạm) của pin
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đó có phải là người chơi hay không (tag là "Player")
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Sounds"); // Dòng này chủ yếu để kiểm tra trong Console

            // Tìm script Flashlight gắn trong player và phục hồi góc chiếu sáng của đèn
            other.GetComponentInChildren<Flashlight>().RestoreLightAngle(restoreAngle);

            // Tăng độ sáng của đèn pin
            other.GetComponentInChildren<Flashlight>().AddLightIntensity(addIntensity);

            // Gọi hàm hồi máu cho người chơi
            player.GiveHealth();

            // Phát âm thanh nhặt pin
            audioSource.PlayOneShot(batteryPickupSound);

            // Xoá vật phẩm pin sau khi đã nhặt
            Destroy(gameObject);
        }
    }
}
