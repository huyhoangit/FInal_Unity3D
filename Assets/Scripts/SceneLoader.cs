using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Trong hàm Update, chúng ta kiểm tra mỗi frame xem người chơi có nhấn phím Escape không.
    // Nếu có, sẽ gọi hàm QuitGame để thoát game.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quitting");
            QuitGame(); // Gọi hàm QuitGame khi người chơi nhấn Escape
        }
    }

    // Hàm này dùng để tải lại scene "Game". Sau khi tải lại, chúng ta cũng đảm bảo game không bị tạm dừng (nếu trước đó có bị tạm dừng).
    // Thực hiện bằng cách đặt Time.timeScale = 1.
    public void ReloadGame()
    {
        SceneManager.LoadScene("Game"); // Tải lại scene "Game"
        Time.timeScale = 1; // Đảm bảo game tiếp tục hoạt động bình thường nếu trước đó bị dừng
    }

    // Hàm này dùng để tải scene "MainMenu". Khi người chơi muốn quay lại menu chính, hàm này sẽ được gọi.
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Tải scene "MainMenu"
    }

    // Hàm này dùng để tải scene "End". Dùng khi game kết thúc và cần chuyển sang màn hình kết thúc.
    public void LoadEndGame()
    {
        SceneManager.LoadScene("End"); // Tải scene "End"
    }

    // Hàm này dùng để thoát game. Khi người chơi nhấn phím Escape hoặc gọi hàm này từ UI, game sẽ bị đóng.
    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit(); // Thoát game
    }
}
