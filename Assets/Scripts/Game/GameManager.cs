using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float timeRemaining = 600f; // 10 minutes
    public bool timerIsRunning = false;

    public bool isPowerRestored = false;
    public bool isCodeSolved = false;
    public bool hasKey = false;
    public bool hasEscaped = false;

    public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
    public GameState currentState = GameState.Playing;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timerIsRunning = true;
        currentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                GameOver();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public bool IsPlaying => currentState == GameState.Playing;

    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        UIManager.Instance.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        UIManager.Instance.HidePauseMenu();
    }

    public void GameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        UIManager.Instance.ShowGameOver();
    }

    public void Victory()
    {
        hasEscaped = true;
        timerIsRunning = false;
        currentState = GameState.Victory;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("EscapeEnding");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
