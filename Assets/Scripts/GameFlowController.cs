using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private string menuSceneName = "MainMenu";

    private InputAction pauseAction;
    private bool isPaused;
    private bool gameEnded;
    private int enemyHits;
    private bool metricsLogged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;

        if (inputActions != null)
        {
            pauseAction = inputActions.FindActionMap("Player", true).FindAction("Pause", true);
        }

        SetPanelActive(pausePanel, false);
        SetPanelActive(victoryPanel, false);
        SetPanelActive(defeatPanel, false);
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.FindActionMap("Player").Enable();
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.FindActionMap("Player").Disable();
        }
    }

    private void Update()
    {
        if (!gameEnded && pauseAction != null && pauseAction.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    public void RegisterEnemyHit()
    {
        if (!gameEnded)
        {
            enemyHits++;
        }
    }

    public void ShowDefeat(int remainingLives = 0)
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        isPaused = false;
        SetPanelActive(defeatPanel, true);
        LogSessionMetrics("Defeat", remainingLives);
        Time.timeScale = 0f;
    }

    public void ShowVictory()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        isPaused = false;
        SetPanelActive(victoryPanel, true);
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        LogSessionMetrics("Victory", playerHealth != null ? playerHealth.CurrentHealth : -1);
        // Play bell sound effect
        Time.timeScale = 0f;
    }

    private void LogSessionMetrics(string result, int remainingLives)
    {
        if (metricsLogged)
        {
            return;
        }

        metricsLogged = true;
        Debug.Log($"GAME_METRICS;result={result};enemyHits={enemyHits};playerLivesAtEnd={remainingLives}");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        SetPanelActive(pausePanel, isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        SetPanelActive(pausePanel, false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private static void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }
}
