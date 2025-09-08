using System.Collections;
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
        public bool isHorizontal;
        [HideInInspector] public List<Vector2Int> positions = new List<Vector2Int>();
        [HideInInspector] public bool isFound = false;
    }

    [Header("Grid Ayarları")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public Vector2 cellSize = new Vector2(60, 60);
    public float cellSpacing = 5f;

    public List<WordData> wordsToPlace;

    private GameObject[,] gridCells;
    private List<Vector2Int> selectedPositions = new List<Vector2Int>();
    private int gridWidth, gridHeight;

    void Start()
    {
        var levelData = LevelManager.Instance.GetCurrentLevelData();
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

                StartCoroutine(AnimateCell(cell, (x + y * gridWidth) * 0.03f));
            }
        }
    }

    IEnumerator AnimateCell(GameObject cell, float delay)
    {
        cell.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(delay);
        float t = 0f;
        while (t < 0.3f)
        {
            cell.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / 0.3f);
            t += Time.deltaTime;
            yield return null;
        }
        cell.transform.localScale = Vector3.one;
    }

    void PlaceAllWords(List<string> wordList)
    {
        wordsToPlace = new List<WordData>();

        foreach (var word in wordList)
        {
            WordData wd = new WordData();
            wd.word = word.ToUpper();
            wd.isHorizontal = Random.value > 0.5f;

            Vector2Int pos;
            int attempts = 0;
            do
            {
                pos = FindStartPosition(wd.word, wd.isHorizontal, gridWidth, gridHeight);
                attempts++;
                if (attempts > 50) break;
            } while (!CanPlaceWord(wd.word, pos, wd.isHorizontal));

            wd.startPos = pos;
            wordsToPlace.Add(wd);
            PlaceWord(wd);
        }
    }

    Vector2Int FindStartPosition(string word, bool horizontal, int maxX, int maxY)
    {
        int xMax = horizontal ? maxX - word.Length : maxX - 1;
        int yMax = horizontal ? maxY - 1 : maxY - word.Length;
        int x = Random.Range(0, xMax + 1);
        int y = Random.Range(0, yMax + 1);
        return new Vector2Int(x, y);
    }

    bool CanPlaceWord(string word, Vector2Int start, bool horizontal)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int x = start.x + (horizontal ? i : 0);
            int y = start.y + (horizontal ? 0 : i);

            if (gridCells[x, y].GetComponentInChildren<Text>().text != "")
                return false;
        }
        return true;
    }

    void PlaceWord(WordData wordData)
    {
        wordData.positions.Clear();
        for (int i = 0; i < wordData.word.Length; i++)
        {
            int x = wordData.startPos.x + (wordData.isHorizontal ? i : 0);
            int y = wordData.startPos.y + (wordData.isHorizontal ? 0 : i);

            Text t = gridCells[x, y].GetComponentInChildren<Text>();
            t.text = wordData.word[i].ToString();
            wordData.positions.Add(new Vector2Int(x, y));
        }
    }

    void FillEmptyCells(int extraLetters)
    {
        List<char> letters = new List<char>();
        foreach (var word in wordsToPlace)
            letters.AddRange(word.word.ToCharArray());

        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < extraLetters; i++)
            letters.Add(alphabet[Random.Range(0, alphabet.Length)]);

        // Shuffle
        for (int i = 0; i < letters.Count; i++)
        {
            char tmp = letters[i];
            int rnd = Random.Range(i, letters.Count);
            letters[i] = letters[rnd];
            letters[rnd] = tmp;
        }

        var circleManager = Object.FindAnyObjectByType<LetterCircleManager>();
        if (circleManager != null)
            circleManager.CreateCircle(new string(letters.ToArray()));
    }

    public void OnCellClicked(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (selectedPositions.Contains(pos)) return;

        selectedPositions.Add(pos);
        gridCells[x, y].GetComponent<Image>().color = Color.yellow;
        CheckSelectedWord();
    }

    void CheckSelectedWord()
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

    bool AllWordsFound()
    {
        foreach (var word in wordsToPlace)
            if (!word.isFound) return false;
        return true;
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

    public void ClearSelection()
    {
        foreach (var pos in selectedPositions)
            gridCells[pos.x, pos.y].GetComponent<Image>().color = Color.white;

        selectedPositions.Clear();
    }
}
