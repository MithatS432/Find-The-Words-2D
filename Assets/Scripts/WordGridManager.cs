using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordGridManager : MonoBehaviour
{
    [System.Serializable]
    public class WordData
    {
        public string word;
        [HideInInspector] public bool isFound = false;
    }

    [Header("Grid Ayarları")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public Vector2 cellSize = new Vector2(80, 80);
    public float cellSpacing = 5f;

    [Header("Oyun Ayarları")]
    public List<WordData> wordsToFind;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI foundWordsText;

    private GameObject[,] gridCells;
    private int gridWidth, gridHeight;
    private int score = 0;
    private int foundWordsCount = 0;

    void Start()
    {
        // Grid boyutunu kelimelere göre ayarla
        gridWidth = 8;
        gridHeight = 8;

        CreateGrid();
        PlaceWords();
        UpdateUI();
    }

    void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];

        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.cellSize = cellSize;
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridWidth;
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                cell.name = $"Cell_{x}_{y}";
                gridCells[x, y] = cell;

                // Hücreyi başlangıçta boş yap
                TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = "";
                }
            }
        }
    }

    void PlaceWords()
    {
        // Örnek kelimeleri yerleştir
        // Bu kısmı LevelManager'dan alacağınız verilerle değiştirebilirsiniz
        wordsToFind = new List<WordData>
        {
            new WordData { word = "PROBLEM" },
            new WordData { word = "MORE" },
            new WordData { word = "ROPE" }
        };
    }

    public void CheckWord(string attemptedWord)
    {
        foreach (WordData wordData in wordsToFind)
        {
            if (!wordData.isFound && attemptedWord.Equals(wordData.word, System.StringComparison.OrdinalIgnoreCase))
            {
                wordData.isFound = true;
                foundWordsCount++;
                score += attemptedWord.Length * 100;

                // Bulunan kelimeyi grid'de göster
                StartCoroutine(RevealWord(wordData.word));

                UpdateUI();
                return;
            }
        }

        // Yanlış kelime için feedback
        Debug.Log("Yanlış kelime: " + attemptedWord);
    }

    IEnumerator RevealWord(string word)
    {
        // Kelimenin grid'de gösterilmesi (animasyonlu)
        // Bu kısmı ihtiyacınıza göre özelleştirebilirsiniz
        yield return new WaitForSeconds(0.5f);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Puan: {score}";

        if (foundWordsText != null)
            foundWordsText.text = $"Bulunan Kelimeler: {foundWordsCount}/{wordsToFind.Count}";
    }

    public bool AreAllWordsFound()
    {
        return foundWordsCount >= wordsToFind.Count;
    }
}