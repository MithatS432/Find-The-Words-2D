using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordGridManager : MonoBehaviour
{
    [System.Serializable]
    public class WordData
    {
        public string word;
        public Vector2Int startPos;
        public bool isHorizontal = true;

        [HideInInspector] public List<Vector2Int> positions = new List<Vector2Int>();
        [HideInInspector] public bool isFound = false;
    }

    [Header("Grid Ayarları")]
    public int gridWidth = 6;
    public int gridHeight = 6;

    [Header("UI Ayarları")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public Vector2 cellSize = new Vector2(60, 60);
    public float cellSpacing = 5f;

    [Header("Kelime Listesi")]
    public List<WordData> wordsToPlace;

    private GameObject[,] gridCells;
    private List<Vector2Int> selectedPositions = new List<Vector2Int>();

    void Start()
    {
        CreateGrid();
        PlaceWords();
    }

    void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];

        float totalWidth = gridWidth * cellSize.x + (gridWidth - 1) * cellSpacing;
        float totalHeight = gridHeight * cellSize.y + (gridHeight - 1) * cellSpacing;

        Vector2 startPos = new Vector2(-totalWidth / 2f + cellSize.x / 2f,
                                       totalHeight / 2f - cellSize.y / 2f);

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject newCell = Instantiate(cellPrefab, gridParent);
                RectTransform rt = newCell.GetComponent<RectTransform>();
                rt.sizeDelta = cellSize;
                rt.anchoredPosition = new Vector2(
                    startPos.x + x * (cellSize.x + cellSpacing),
                    startPos.y - y * (cellSize.y + cellSpacing)
                );

                newCell.name = $"Cell_{x}_{y}";
                gridCells[x, y] = newCell;

                // Cell scripti bağla
                Cell cellScript = newCell.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.x = x;
                    cellScript.y = y;
                    cellScript.gridManager = this;
                }
            }
        }
    }

    void PlaceWords()
    {
        foreach (var wordData in wordsToPlace)
        {
            wordData.positions.Clear();

            for (int i = 0; i < wordData.word.Length; i++)
            {
                int x = wordData.startPos.x + (wordData.isHorizontal ? i : 0);
                int y = wordData.startPos.y + (wordData.isHorizontal ? 0 : i);

                if (x < gridWidth && y < gridHeight)
                {
                    Text cellText = gridCells[x, y].GetComponentInChildren<Text>();
                    if (cellText != null)
                        cellText.text = wordData.word[i].ToString();

                    wordData.positions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    public void OnCellClicked(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);

        // Tekrar seçmesin
        if (selectedPositions.Contains(pos)) return;

        selectedPositions.Add(pos);
        gridCells[x, y].GetComponent<Image>().color = Color.yellow;

        CheckSelectedWord();
    }

    void CheckSelectedWord()
    {
        foreach (var wordData in wordsToPlace)
        {
            if (!wordData.isFound && MatchWord(wordData))
            {
                wordData.isFound = true;

                // yeşile boya
                foreach (var pos in wordData.positions)
                {
                    gridCells[pos.x, pos.y].GetComponent<Image>().color = Color.green;
                }

                selectedPositions.Clear();

                // Level bitti mi kontrol
                if (AllWordsFound())
                {
                    Debug.Log("Level tamamlandı!");
                    LevelManager levelManager = FindAnyObjectByType<LevelManager>();
                    if (levelManager != null)
                        levelManager.OnLevelCompleted();
                }

                return;
            }
        }
    }

    bool MatchWord(WordData wordData)
    {
        if (wordData.positions.Count != selectedPositions.Count) return false;

        foreach (var pos in wordData.positions)
        {
            if (!selectedPositions.Contains(pos))
                return false;
        }
        return true;
    }

    bool AllWordsFound()
    {
        foreach (var wordData in wordsToPlace)
        {
            if (!wordData.isFound) return false;
        }
        return true;
    }
    public void CheckSubmittedWord(string word)
    {
        foreach (var wordData in wordsToPlace)
        {
            if (!wordData.isFound && wordData.word.Equals(word, System.StringComparison.OrdinalIgnoreCase))
            {
                wordData.isFound = true;

                // gridde kelimeyi yeşile boya
                foreach (var pos in wordData.positions)
                {
                    gridCells[pos.x, pos.y].GetComponent<Image>().color = Color.green;
                }

                Debug.Log("Kelime bulundu: " + word);

                if (AllWordsFound())
                {
                    Debug.Log("Level tamamlandı!");
                    LevelManager levelManager = FindAnyObjectByType<LevelManager>();
                    if (levelManager != null)
                        levelManager.OnLevelCompleted();
                }

                return;
            }
        }

        Debug.Log("Yanlış kelime: " + word);
    }

}
