using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script này đánh dấu điểm kết thúc màn chơi (ví dụ như đi đến cổng thoát, hoàn thành nhiệm vụ...)
public class EndGame : MonoBehaviour
{
    // Thời gian chờ trước khi thật sự kết thúc màn (để có thể hiển thị animation, âm thanh,... nếu cần)
    [SerializeField] float levelEndDelay = 8f;

    // Tham chiếu đến script xử lý việc kết thúc game
    DeathHandler death;

    private void Awake()
    {
        // Tự động tìm đối tượng có gắn script DeathHandler (thường là Player hoặc GameManager)
        death = FindObjectOfType<DeathHandler>();
    }

    // Hàm này được gọi khi có vật thể (có collider) đi vào vùng trigger của điểm kết thúc
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu người chơi chạm vào điểm kết thúc màn
        if (other.gameObject.tag == "Player")
        {
            // Bắt đầu quá trình kết thúc màn chơi
            StartCoroutine(EndSession());
        }

        // Coroutine giúp đợi một khoảng thời gian rồi mới thực hiện hành động kết thúc
        IEnumerator EndSession()
        {
            // Đợi trong vài giây (tùy vào giá trị đã gán), có thể để chạy nhạc kết thúc hoặc animation
            yield return new WaitForSeconds(levelEndDelay);

            // Gọi hàm kết thúc game (hoặc chuyển sang màn khác, hoặc hiện giao diện kết thúc,...)
            death.EndGame();
        }
    }
}
