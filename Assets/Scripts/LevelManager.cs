using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string word;
        public int extraLettersCount;
        public Vector2Int gridSize = new Vector2Int(6, 6);
        public List<string> words; // Birden fazla kelime desteği
    }

    public Button[] levelButtons;
    public LevelData[] levels;
    public static LevelManager Instance;

    void Awake()
    {
        // Singleton pattern - sadece bir Instance olmalı
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // PlayerPrefs yoksa başlangıç değerlerini ayarla
            if (!PlayerPrefs.HasKey("UnlockedLevel"))
            {
                PlayerPrefs.SetInt("UnlockedLevel", 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        if (levelButtons == null)
        {
            Debug.LogError("levelButtons dizisi atanmadı!");
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null)
            {
                Debug.LogError($"levelButtons[{i}] null!");
                continue;
            }

            var textComponent = levelButtons[i].GetComponentInChildren<Text>();
            if (i + 1 <= unlockedLevel)
            {
                levelButtons[i].interactable = true;
                if (textComponent != null)
                    textComponent.text = (i + 1).ToString();
            }
            else
            {
                levelButtons[i].interactable = false;
                if (textComponent != null)
                    textComponent.text = "🔒";
            }

            int levelIndex = i + 1;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex <= levels.Length)
        {
            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
            SceneManager.LoadScene(levelIndex.ToString());
        }
        else
        {
            Debug.Log("Tüm leveller tamamlandı!");
        }
    }

    public LevelData GetCurrentLevelData()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Dizi sınırlarını kontrol et
        if (currentLevel > 0 && currentLevel <= levels.Length)
        {
            return levels[currentLevel - 1];
        }

        Debug.LogError("Geçersiz level indeksi: " + currentLevel);
        return levels[0]; // Varsayılan olarak ilk level
    }

    public void OnLevelCompleted()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("LevelMenu");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}