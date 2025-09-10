using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons;
    public LevelData[] levels;
    public static LevelManager Instance;

    [System.Serializable]
    public class LevelData
    {
        public List<string> words;
        public int extraLettersCount = 10;
        public Vector2Int gridSize = new Vector2Int(6, 6);
        public string sceneName; // Level sahne ismi
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!PlayerPrefs.HasKey("CurrentLevel"))
                PlayerPrefs.SetInt("CurrentLevel", 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (levelButtons != null && levelButtons.Length > 0)
            SetupLevelButtons();
    }

    void SetupLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int capturedIndex = i + 1;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() =>
            {
                LoadLevel(capturedIndex);
            });

            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
            levelButtons[i].interactable = capturedIndex <= unlockedLevel;
        }
    }

    public LevelData GetCurrentLevelData()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel > 0 && currentLevel <= levels.Length)
            return levels[currentLevel - 1];
        return levels[0];
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex > 0 && levelIndex <= levels.Length)
        {
            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
            string sceneName = levels[levelIndex - 1].sceneName;
            if (Application.CanStreamedLevelBeLoaded(sceneName))
                SceneManager.LoadScene(sceneName);
            else
                Debug.LogError("Sahne bulunamadı: " + sceneName);
        }
    }

    public void OnLevelCompleted()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("UnlockedLevel", Mathf.Max(PlayerPrefs.GetInt("UnlockedLevel", 1), currentLevel + 1));

        if (Application.CanStreamedLevelBeLoaded("LevelMenu"))
            SceneManager.LoadScene("LevelMenu");
        else
            Debug.LogError("LevelMenu sahnesi Build Settings'e eklenmemiş!");
    }
}
