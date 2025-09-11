using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LetterCircleManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject letterButtonPrefab;
    public Transform circleParent;
    public float radius = 150f;
    public TextMeshProUGUI currentWordText;

    private List<Letter> lettersList = new List<Letter>();
    private List<Letter> selectedLetters = new List<Letter>();
    private string currentWord = "";
    public bool IsDragging { get; private set; } = false;

    void Start()
    {
        var levelData = LevelManager.Instance.GetCurrentLevelData();

        // Kelimelerdeki harfleri tek seferlik al
        HashSet<char> uniqueLetters = new HashSet<char>();
        foreach (var w in levelData.words)
            foreach (var c in w)
                uniqueLetters.Add(c);

        string letters = new string(new List<char>(uniqueLetters).ToArray());

        CreateCircle(letters);
    }


    public void CreateCircle(string letters)
    {
        foreach (Transform child in circleParent)
            Destroy(child.gameObject);
        lettersList.Clear();

        int count = letters.Length;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            GameObject btn = Instantiate(letterButtonPrefab, circleParent);
            RectTransform rt = btn.GetComponent<RectTransform>();
            float angle = i * angleStep * Mathf.Deg2Rad;
            rt.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = letters[i].ToString();

            Letter letterComp = btn.AddComponent<Letter>();
            letterComp.letter = letters[i];
            letterComp.circleManager = this;

            lettersList.Add(letterComp);
        }
    }

    string GenerateExtraLetters(int count)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < count; i++)
            sb.Append(alphabet[Random.Range(0, alphabet.Length)]);
        return sb.ToString();
    }

    public void OnLetterSelected(Letter letter)
    {
        if (!selectedLetters.Contains(letter))
        {
            selectedLetters.Add(letter);
            currentWord += letter.letter;

            if (currentWordText != null)
                currentWordText.text = currentWord;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDragging = true;
        ClearSelection();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;

        // Sürükleme sırasında fare pozisyonundaki harfleri bul
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            Letter letter = result.gameObject.GetComponent<Letter>();
            if (letter != null && !selectedLetters.Contains(letter))
            {
                OnLetterSelected(letter);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDragging && !string.IsNullOrEmpty(currentWord))
        {
            WordGridManager gridManager = Object.FindAnyObjectByType<WordGridManager>();
            if (gridManager != null)
            {
                bool isCorrect = gridManager.CheckWord(currentWord);
                if (isCorrect)
                {
                    // Doğru kelime - harfleri yeşil yap ve devre dışı bırak
                    foreach (var l in selectedLetters)
                    {
                        l.SetCorrect();
                        l.SetInteractable(false);
                    }
                }
                else
                {
                    // Yanlış kelime - sıfırla
                    ClearSelection();
                }
            }
        }

        IsDragging = false;
    }

    public void ClearSelection()
    {
        foreach (var l in lettersList)
            l.ResetSelection();

        selectedLetters.Clear();
        currentWord = "";

        if (currentWordText != null)
            currentWordText.text = "";
    }
}