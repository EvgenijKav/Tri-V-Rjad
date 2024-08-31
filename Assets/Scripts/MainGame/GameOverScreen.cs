using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour {
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private Button RestartButton;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start() {
        GameOverPanel.SetActive(false);
        RestartButton.onClick.AddListener(RestartGame);
    }

    public void ShowGameOver(int finalScore, int bestScore) {
        finalScoreText.text = "Итоговый счёт: " + finalScore;
        bestScoreText.text = "Лучший счёт: " + bestScore;
        GameOverPanel.SetActive(true);
    }

    private void RestartGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}