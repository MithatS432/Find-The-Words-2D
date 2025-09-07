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
    }

    [Header("Grid Ayarları")]
    public int gridWidth = 5;
    public int gridHeight = 5;

    [Header("UI Ayarları")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public Vector2 cellSize = new Vector2(30, 30);
    public float cellSpacing = 5f;

    [Header("Kelime Listesi")]
    public List<WordData> wordsToPlace;

    private GameObject[,] gridCells;
    private string selectedWord = "";
    private List<GameObject> selectedCells = new List<GameObject>();

    void Start()
    {
        CreateGrid();
        PlaceWords();
    }

    void CreateGrid()
    {
        gridCells = new GameObject[gridWidth, gridHeight];

        // Grid toplam boyutu
        float totalWidth = gridWidth * cellSize.x + (gridWidth - 1) * cellSpacing;
        float totalHeight = gridHeight * cellSize.y + (gridHeight - 1) * cellSpacing;

        // Grid’in başlangıç pozisyonu (merkezden ortalanmış, biraz yukarı kaydırılmış)
        Vector2 startPos = new Vector2(
            -totalWidth / 2f + cellSize.x / 2f,   // X pivot düzeltmesi
            totalHeight / 2f - cellSize.y / 2f - 50f // Y pivot ve yukarı kaydırma
        );

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

                // Cell scripti ayarla
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
            for (int i = 0; i < wordData.word.Length; i++)
            {
                int x = wordData.startPos.x + (wordData.isHorizontal ? i : 0);
                int y = wordData.startPos.y + (wordData.isHorizontal ? 0 : i);

                if (x < gridWidth && y < gridHeight)
                {
                    Text cellText = gridCells[x, y].GetComponentInChildren<Text>();
                    if (cellText != null)
                        cellText.text = wordData.word[i].ToString();
                }
            }
        }
    }

    public void OnCellClicked(int x, int y)
    {
        GameObject cell = gridCells[x, y];
        Text cellText = cell.GetComponentInChildren<Text>();
        if (cellText == null || string.IsNullOrEmpty(cellText.text))
            return;

        selectedWord += cellText.text;
        selectedCells.Add(cell);

        Image img = cell.GetComponent<Image>();
        if (img != null)
            img.color = Color.yellow;

        CheckWord();
    }

    void CheckWord()
    {
        foreach (var wordData in wordsToPlace)
        {
            if (selectedWord.Equals(wordData.word, System.StringComparison.OrdinalIgnoreCase))
            {
                foreach (var cell in selectedCells)
                {
                    Image img = cell.GetComponent<Image>();
                    if (img != null)
                        img.color = Color.green;
                }

                selectedCells.Clear();
                selectedWord = "";

                CheckLevelComplete();
                return;
            }
        }
    }

    void CheckLevelComplete()
    {
        bool allFound = true;

        foreach (var wordData in wordsToPlace)
        {
            bool found = false;
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Text t = gridCells[x, y].GetComponentInChildren<Text>();
                    if (t != null && t.text.Equals(wordData.word[0].ToString(), System.StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (!found)
            {
                allFound = false;
                break;
            }
        }

        if (allFound)
        {
            Debug.Log("Level tamamlandı!");
            LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
            if (levelManager != null)
                levelManager.OnLevelCompleted();
        }
    }
}
