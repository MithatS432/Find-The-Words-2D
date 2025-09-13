using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordGridManager : MonoBehaviour
{
    [Header("Grid UI")]
    public Transform wordsParent; // Kelimelerin gösterileceği parent
    public GameObject wordUIPrefab; // Kelime UI prefab'ı

    [Header("Grid Ayarları")]
    public GridLayoutGroup gridLayout;

    private List<string> targetWords = new List<string>();
    private List<string> foundWords = new List<string>();
    private Dictionary<string, GameObject> wordUIObjects = new Dictionary<string, GameObject>();

    [Header("Debug")]
    public bool showDebugLogs = true;

    void Start()
    {
        SetupGrid();
    }

    void SetupGrid()
    {
        Debug.Log("SetupGrid başladı...");

        var levelData = LevelManager.Instance?.GetCurrentLevelData();

        if (levelData == null)
        {
            Debug.LogError("Level data bulunamadı! Test kelimeler kullanılacak.");
            CreateTestLevel();
            return;
        }

        // Target kelimeleri ayarla
        targetWords.Clear();
        foundWords.Clear();

        if (levelData.words != null)
        {
            foreach (string word in levelData.words)
            {
                string upperWord = word?.ToUpper().Trim();
                if (!string.IsNullOrEmpty(upperWord))
                {
                    targetWords.Add(upperWord);
                }
            }
        }

        if (targetWords.Count == 0)
        {
            Debug.LogWarning("levelData.words boş! Test kelimeleri ekleniyor...");
            CreateTestLevel();
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log($"Target words: {string.Join(", ", targetWords)}");
        }

        CreateWordUI();
    }

    void CreateWordUI()
    {
        // Önceki UI'ları temizle
        foreach (Transform child in wordsParent)
        {
            Destroy(child.gameObject);
        }
        wordUIObjects.Clear();

        // Her kelime için UI oluştur
        foreach (string word in targetWords)
        {
            GameObject wordObj = Instantiate(wordUIPrefab, wordsParent);
            wordObj.name = "Word_" + word;

            // Word UI'ı ayarla
            SetupWordUI(wordObj, word, false);

            wordUIObjects[word] = wordObj;
        }

        // Grid layout'u güncelle
        if (gridLayout != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayout.GetComponent<RectTransform>());
        }
    }

    void SetupWordUI(GameObject wordObj, string word, bool isFound)
    {
        if (wordObj == null)
        {
            Debug.LogError("SetupWordUI: wordObj null!");
            return;
        }

        // Text component'ini bul ve ayarla
        TextMeshProUGUI[] texts = wordObj.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length > 0)
        {
            texts[0].text = isFound ? word : new string('_', word.Length);
            texts[0].color = isFound ? Color.black : Color.gray;
        }
        else
        {
            Debug.LogWarning($"SetupWordUI: {word} için TextMeshProUGUI bulunamadı!");
        }

        // Background color'ı ayarla
        Image bg = wordObj.GetComponent<Image>();
        if (bg != null)
        {
            bg.color = isFound ? new Color(0.8f, 1f, 0.8f, 0.5f) : new Color(1f, 1f, 1f, 0.3f);
        }
    }
    void CreateTestLevel()
    {
        // Test kelimeler
        targetWords = new List<string> { "CAR", "CAN", "CAT", "COW", "CUP" };
        foundWords.Clear();

        if (showDebugLogs)
            Debug.Log("Test level oluşturuldu: " + string.Join(", ", targetWords));

        // UI'ı oluştur
        CreateWordUI();
    }


    public bool CheckWord(string inputWord)
    {
        if (string.IsNullOrEmpty(inputWord))
            return false;

        string upperInput = inputWord.ToUpper().Trim();

        if (showDebugLogs)
        {
            Debug.Log($"Checking word: '{upperInput}'");
            Debug.Log($"Target words: {string.Join(", ", targetWords)}");
            Debug.Log($"Found words: {string.Join(", ", foundWords)}");
        }

        // Kelime target words'de var mı ve daha önce bulunmamış mı?
        if (targetWords.Contains(upperInput) && !foundWords.Contains(upperInput))
        {
            // Kelimeyi bulunmuş olarak işaretle
            foundWords.Add(upperInput);

            // UI'ı güncelle
            if (wordUIObjects.ContainsKey(upperInput))
            {
                SetupWordUI(wordUIObjects[upperInput], upperInput, true);
            }

            if (showDebugLogs)
            {
                Debug.Log($"✅ Correct word found: {upperInput}");
            }

            // Level tamamlandı mı kontrol et
            CheckLevelCompletion();

            return true;
        }

        if (showDebugLogs)
        {
            if (foundWords.Contains(upperInput))
                Debug.Log($"❌ Word already found: {upperInput}");
            else
                Debug.Log($"❌ Word not in target list: {upperInput}");
        }

        return false;
    }

    void CheckLevelCompletion()
    {
        if (foundWords.Count >= targetWords.Count)
        {
            if (showDebugLogs)
            {
                Debug.Log("🎉 Level Completed!");
            }

            // Level tamamlandı
            OnLevelCompleted();
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log($"Progress: {foundWords.Count}/{targetWords.Count}");
            }
        }
    }

    void OnLevelCompleted()
    {
        // Level tamamlandığında yapılacaklar
        Debug.Log("Level Tamamlandı!");

        // Biraz bekle sonra level menu'ye dön
        Invoke(nameof(ReturnToLevelMenu), 2f);
    }

    void ReturnToLevelMenu()
    {
        LevelManager.Instance.OnLevelCompleted();
    }

    // Public metodlar - dış sınıflar için
    public List<string> GetTargetWords()
    {
        return new List<string>(targetWords);
    }

    public List<string> GetFoundWords()
    {
        return new List<string>(foundWords);
    }

    public int GetProgress()
    {
        return foundWords.Count;
    }

    public int GetTotalWords()
    {
        return targetWords.Count;
    }

    public bool IsLevelCompleted()
    {
        return foundWords.Count >= targetWords.Count;
    }

    // Test metodu
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void TestCheckWord(string word)
    {
        bool result = CheckWord(word);
        Debug.Log($"Test Result: '{word}' = {result}");
    }
}