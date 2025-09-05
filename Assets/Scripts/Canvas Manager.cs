using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteManager : MonoBehaviour
{
    public Button nextButton;
    public Button menuButton;

    private void Start()
    {
        nextButton.onClick.AddListener(LoadNextLevel);
        menuButton.onClick.AddListener(GoToMenu);

        nextButton.gameObject.SetActive(false);
    }

    public void OnLevelCompleted()
    {
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name);

        UnlockNextLevel(currentLevel);

        SceneManager.LoadScene("LevelMenu");
    }

    void LoadNextLevel()
    {
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name);
        int nextLevel = currentLevel + 1;

        if (Application.CanStreamedLevelBeLoaded(nextLevel.ToString()))
        {
            SceneManager.LoadScene(nextLevel.ToString());
        }
        else
        {
            Debug.Log("Tebrikler! Tüm levelleri bitirdin 🎉");
        }
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }

    void UnlockNextLevel(int currentLevel)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}
