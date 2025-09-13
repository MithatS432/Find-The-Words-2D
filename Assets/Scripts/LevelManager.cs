using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [System.Serializable]
    public class LevelData
    {
        public string sceneName;                  // Level sahnesi adı
        public List<string> words;                // Oyun için kelimeler
        public int extraLettersCount = 10;        // Ekstra harf sayısı
        public Vector2Int gridSize = new Vector2Int(6, 6); // Grid boyutu
    }

    public LevelData[] levels;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!PlayerPrefs.HasKey("CurrentLevel"))
                PlayerPrefs.SetInt("CurrentLevel", 1);

            if (!PlayerPrefs.HasKey("UnlockedLevel"))
                PlayerPrefs.SetInt("UnlockedLevel", 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Mevcut level verilerini almak için
    public LevelData GetCurrentLevelData()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel > 0 && currentLevel <= levels.Length)
            return levels[currentLevel - 1];
        return levels[0];
    }

    // Level yükleme
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

    // Level tamamlandıktan sonra çağrılır
    public void OnLevelCompleted()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel + 1 > unlockedLevel)
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);

        SceneManager.LoadScene("LevelMenu"); // Menü sahnesine dön
    }
}
