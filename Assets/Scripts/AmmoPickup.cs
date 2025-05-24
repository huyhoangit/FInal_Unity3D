using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script này gắn vào vật phẩm đạn (ammo pickup), giúp người chơi nhặt được đạn khi va chạm
public class AmmoPickup : MonoBehaviour
{
    // Số lượng đạn mà vật phẩm này sẽ cung cấp
    [SerializeField] int ammoAmount = 5;

    // Loại đạn mà vật phẩm này cung cấp (ví dụ: súng ngắn, súng trường,...)
    [SerializeField] AmmoType ammoType;

    // Âm thanh sẽ phát khi người chơi nhặt vật phẩm này
    [SerializeField] AudioClip pickupSound;

    // Biến để chứa thành phần AudioSource (giúp phát âm thanh)
    AudioSource audioSource;

    private void Start()
    {
        // Lấy AudioSource từ đối tượng cha (nếu AudioSource không nằm trực tiếp trên chính đối tượng này)
        audioSource = GetComponentInParent<AudioSource>();
    }

    // Hàm này được gọi khi có một collider khác đi vào vùng trigger của vật phẩm
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có tag là "Player" hay không
        if (other.gameObject.tag == "Player")
        {
            // Tìm đối tượng có script "Ammo" và gọi hàm tăng lượng đạn
            FindObjectOfType<Ammo>().IncreaseCurrentAmmo(ammoType, ammoAmount);

            // In log ra để kiểm tra (có thể bỏ dòng này nếu không cần debug)
            Debug.Log("Sounds");

            // Phát âm thanh nhặt vật phẩm
            audioSource.PlayOneShot(pickupSound);

            // Hủy vật phẩm sau khi đã nhặt (để nó biến mất khỏi màn chơi)
            Destroy(gameObject);
        }
    }
}
