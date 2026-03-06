using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score;
    public int targetScore = 100;

    public TextMeshProUGUI scoreText;

    public GameObject winPanel;

    void Awake()
    {
        Instance = this;
    }

    public void AddPoints(int amount)
    {
        score += amount;

        scoreText.text = "Score: " + score;

        if (score >= targetScore)
        {
            Win();
        }
    }

    void Win()
    {
        winPanel.SetActive(true);
    }

    public void NextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}