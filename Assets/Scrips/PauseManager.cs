using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button pauseButton;
    public Button resumeButton;
    public Button exitButton;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenu.SetActive(false);

        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(TogglePause);
        exitButton.onClick.AddListener(ExitToMainMenu);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f; // Остановить время во время паузы
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Возвращаем скорость времени в норму перед сценой
        SceneManager.LoadScene("Menu"); // Здесь напиши имя твоей сцены главного меню
    }
}
