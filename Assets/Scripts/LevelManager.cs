using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string word;  // doğru kelime (örn: "CAT")
        public int extraLettersCount; // fazladan harf sayısı (örn: 3)
    }

    public Button[] levelButtons;
    public LevelData[] levels;


    void Start()
    {
        if (SceneManager.GetActiveScene().name == "LevelMenu")
        {
            SetupLevelButtons();
        }
    }

    void SetupLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < unlockedLevel)
                levelButtons[i].interactable = true;
            else
                levelButtons[i].interactable = false;

            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex.ToString());
    }

    public void OnLevelCompleted()
    {
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene("LevelMenu");
    }
}
