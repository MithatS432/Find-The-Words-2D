using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public List<string> words;          // Leveldeki tüm kelimeler
        public int extraLettersCount = 5;   // Ekstra rastgele harf
        public Vector2Int gridSize = new Vector2Int(6, 6);
    }

    public Button[] levelButtons;
    public LevelData[] levels;

    public static LevelManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "LevelMenu")
            SetupLevelButtons();
    }

    void SetupLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].interactable = levelIndex <= unlockedLevel;

            Text btnText = levelButtons[i].GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = levelButtons[i].interactable ? levelIndex.ToString() : "🔒";

            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex <= levels.Length)
        {
            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
            // Sahne adını levelIndex'e göre yükle (örn: "1")
            SceneManager.LoadScene(levelIndex.ToString());
        }
        else Debug.Log("Tüm leveller tamamlandı!");
    }

    public LevelData GetCurrentLevelData()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel <= levels.Length)
            return levels[currentLevel - 1];

        return levels[0];
    }

    public void OnLevelCompleted()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel && currentLevel < levels.Length)
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);

        PlayerPrefs.Save();

        if (currentLevel < levels.Length)
            LoadNextLevel();
        else
            Debug.Log("Tebrikler! Tüm seviyeler tamamlandı!");
    }

    void LoadNextLevel()
    {
        int nextLevel = PlayerPrefs.GetInt("CurrentLevel", 1) + 1;
        PlayerPrefs.SetInt("CurrentLevel", nextLevel);
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
