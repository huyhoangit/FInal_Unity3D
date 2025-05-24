using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas youWonCanvas;


    private void Start()
    {
        // tắt cả hai canvas khi bắt đầu game
        gameOverCanvas.enabled = false;
        youWonCanvas.enabled = false;
    }

    public void Handledeath()
    {
        // tắt canvas you won, bật canvas game over ( nếu chơi thua)
        youWonCanvas.enabled = false;
        gameOverCanvas.enabled = true;

        Freezegame();
    }

    public void Endgame()
    {
        // tắt canvas game over, bật canvas you won ( nếu chơi thắng)
        gameOverCanvas.enabled = false;
        youWonCanvas.enabled = true;

        Freezegame();
    }
    public void HandleDeath()
    {
        gameOverCanvas.enabled = true;
        Time.timeScale = 0;
        FindObjectOfType<WeaponSwitcher>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void EndGame()
    {
        youWonCanvas.enabled = true;
        Time.timeScale = 0;
        FindObjectOfType<WeaponSwitcher>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Freezegame()
    {
        Time.timeScale = 0;
        FindObjectOfType<WeaponSwitcher>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
}
