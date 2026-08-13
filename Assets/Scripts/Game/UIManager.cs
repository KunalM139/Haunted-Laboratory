using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text timerText;
    public Slider timerBar;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    
    private float initialTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            initialTime = GameManager.Instance.timeRemaining;
        }
        if (timerBar != null)
        {
            timerBar.maxValue = initialTime;
            timerBar.value = initialTime;
        }
        HidePauseMenu();
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.timerIsRunning)
        {
            float time = GameManager.Instance.timeRemaining;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                timerText.text = $"TIME REMAINING: {minutes:00}:{seconds:00}";
            }
            if (timerBar != null)
            {
                timerBar.value = time;
            }
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverMenu != null) gameOverMenu.SetActive(true);
    }
    
    public void ResumeButton()
    {
        GameManager.Instance.ResumeGame();
    }

    public void RestartButton()
    {
        GameManager.Instance.RestartGame();
    }

    public void MainMenuButton()
    {
        GameManager.Instance.MainMenu();
    }
}
