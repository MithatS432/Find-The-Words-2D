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
        public Vector2Int startPos;
        public bool isHorizontal;
        [HideInInspector] public List<Vector2Int> positions = new List<Vector2Int>();
        [HideInInspector] public bool isFound = false;
    }

    [Header("Grid Ayarları")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public Vector2 cellSize = new Vector2(80, 80);
    public float cellSpacing = 10f;

    public LetterCircleManager letterCircleManager;

    private GameObject[,] gridCells;
    private List<Vector2Int> selectedPositions = new List<Vector2Int>();
    private int gridWidth, gridHeight;
    private List<WordData> wordsToPlace;

    void Start()
    {
        if (LevelManager.Instance == null) return;
        var levelData = LevelManager.Instance.GetCurrentLevelData();
        if (levelData == null) return;

        gridWidth = levelData.gridSize.x;
        gridHeight = levelData.gridSize.y;

        CreateGrid();
        PlaceAllWords(levelData.words);
        FillEmptyCells(levelData.extraLettersCount);
    }

    void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];
        float totalWidth = gridWidth * cellSize.x + (gridWidth - 1) * cellSpacing;
        float totalHeight = gridHeight * cellSize.y + (gridHeight - 1) * cellSpacing;
        Vector2 startPos = new Vector2(-totalWidth / 2 + cellSize.x / 2, totalHeight / 2 - cellSize.y / 2);

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.sizeDelta = cellSize;
                rt.anchoredPosition = new Vector2(startPos.x + x * (cellSize.x + cellSpacing),
                                                  startPos.y - y * (cellSize.y + cellSpacing));
                gridCells[x, y] = cell;

                var cellScript = cell.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.x = x;
                    cellScript.y = y;
                    cellScript.gridManager = this;
                }
            }
        }
    }

    void PlaceAllWords(List<string> wordList)
    {
        wordsToPlace = new List<WordData>();
        int row = 0;

        foreach (var word in wordList)
        {
            WordData wd = new WordData();
            wd.word = word.ToUpper();
            wd.isHorizontal = true;
            wd.startPos = new Vector2Int(0, row);
            PlaceWord(wd);
            wordsToPlace.Add(wd);

            row++;
            if (row >= gridHeight) row = 0;
        }
    }

    void PlaceWord(WordData wordData)
    {
        wordData.positions.Clear();
        for (int i = 0; i < wordData.word.Length; i++)
        {
            int x = wordData.startPos.x + i;
            int y = wordData.startPos.y;
            if (x >= gridWidth) break;

            var cellObj = gridCells[x, y];
            if (cellObj == null) continue;

            TMP_Text t = cellObj.GetComponentInChildren<TMP_Text>();
            if (t == null)
            {
                Debug.LogError($"gridCells[{x},{y}] içinde TMP_Text bileşeni bulunamadı!");
                continue;
            }

            t.text = wordData.word[i].ToString();
            wordData.positions.Add(new Vector2Int(x, y));
        }
    }

    void FillEmptyCells(int extraLetters)
    {
        List<char> letters = new List<char>();
        foreach (var word in wordsToPlace)
            letters.AddRange(word.word.ToCharArray());

        string alphabet = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ";
        for (int i = 0; i < extraLetters; i++)
            letters.Add(alphabet[Random.Range(0, alphabet.Length)]);

        // Karıştır
        for (int i = 0; i < letters.Count; i++)
        {
            char tmp = letters[i];
            int rnd = Random.Range(i, letters.Count);
            letters[i] = letters[rnd];
            letters[rnd] = tmp;
        }

        var circleManager = Object.FindAnyObjectByType<LetterCircleManager>();
        if (circleManager != null)
        {
            circleManager.CreateCircle(new string(letters.ToArray()));
        }
        else
        {
            Debug.LogError("LetterCircleManager bulunamadı!");
        }
    }

    public bool CheckWord(string attempt)
    {
        foreach (var word in wordsToPlace)
        {
            if (attempt.Equals(word.word, System.StringComparison.OrdinalIgnoreCase) && !word.isFound)
            {
                word.isFound = true;
                foreach (var pos in word.positions)
                    gridCells[pos.x, pos.y].GetComponent<Image>().color = Color.green;

                if (AllWordsFound())
                    LevelManager.Instance.OnLevelCompleted();
                return true;
            }
        }
        return false;
    }

    bool AllWordsFound()
    {
        foreach (var word in wordsToPlace)
            if (!word.isFound) return false;
        return true;
    }

    public void ClearSelection()
    {
        foreach (var pos in selectedPositions)
            gridCells[pos.x, pos.y].GetComponent<Image>().color = Color.white;
        selectedPositions.Clear();
    }

    public void OnCellClicked(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (selectedPositions.Contains(pos)) return;

        selectedPositions.Add(pos);
        gridCells[x, y].GetComponent<Image>().color = Color.yellow;
        CheckSelectedWords();
    }

    void CheckSelectedWords()
    {
        foreach (var word in wordsToPlace)
        {
            if (!word.isFound && MatchWord(word))
            {
                word.isFound = true;
                foreach (var pos in word.positions)
                    gridCells[pos.x, pos.y].GetComponent<Image>().color = Color.green;

                selectedPositions.Clear();

                if (AllWordsFound())
                    LevelManager.Instance.OnLevelCompleted();
                return;
            }
        }
    }

    bool MatchWord(WordData wordData)
    {
        if (wordData.positions.Count != selectedPositions.Count) return false;
        foreach (var pos in wordData.positions)
            if (!selectedPositions.Contains(pos)) return false;
        return true;
    }
}
