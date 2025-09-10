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

    public GameObject cellPrefab;
    public Transform gridParent;

    [HideInInspector] public List<WordData> wordsToFind;
    [HideInInspector] public int gridWidth = 8;
    [HideInInspector] public int gridHeight = 8;

    private GameObject[,] gridCells;

    void Start()
    {
        // Level verilerini al
        var levelData = LevelManager.Instance.GetCurrentLevelData();

        // Kelimeleri hazırla
        wordsToFind = new List<WordData>();
        foreach (var w in levelData.words)
        {
            wordsToFind.Add(new WordData { word = w });
        }

        gridWidth = levelData.gridSize.x;
        gridHeight = levelData.gridSize.y;

        CreateGrid();
    }

    void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];

        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
            gridLayout.constraintCount = gridWidth;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                cell.name = $"Cell_{x}_{y}";

                gridCells[x, y] = cell;

                TextMeshProUGUI text = cell.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = "";
            }
        }
    }
    void RevealWordInGrid(string word)
    {
        int maxX = gridWidth - word.Length;
        int xStart = Random.Range(0, maxX + 1);
        int yStart = Random.Range(0, gridHeight);

        for (int i = 0; i < word.Length; i++)
        {
            TextMeshProUGUI text = gridCells[xStart + i, yStart].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = word[i].ToString();
        }

        Debug.Log("Doğru kelime bulundu: " + word);
    }

    public bool AreAllWordsFound()
    {
        foreach (WordData wordData in wordsToFind)
            if (!wordData.isFound) return false;
        return true;
    }
    public bool CheckWord(string attemptedWord)
    {
        foreach (WordData wordData in wordsToFind)
        {
            if (!wordData.isFound && attemptedWord.Equals(wordData.word, System.StringComparison.OrdinalIgnoreCase))
            {
                wordData.isFound = true;
                RevealWordInGrid(wordData.word);

                if (AreAllWordsFound())
                    LevelManager.Instance.OnLevelCompleted();

                return true; // doğru kelime bulundu
            }
        }
        return false; // yanlış kelime
    }

}
