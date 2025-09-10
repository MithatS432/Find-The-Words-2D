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
    private bool isDragging = false;

    void Start()
    {
        var levelData = LevelManager.Instance.GetCurrentLevelData();

        string letters = string.Join("", levelData.words.ToArray());
        letters += GenerateExtraLetters(levelData.extraLettersCount);

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
        isDragging = true;
        currentWord = "";
        selectedLetters.Clear();
        if (currentWordText != null)
            currentWordText.text = "";
        ResetLetters(); // Önceki sarılar kalmasın, sadece seçilmeye başlanan sarı olacak
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            Letter letter = result.gameObject.GetComponent<Letter>();
            if (letter != null) letter.OnPointerEnter(null);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging && !string.IsNullOrEmpty(currentWord))
        {
            WordGridManager gridManager = Object.FindAnyObjectByType<WordGridManager>();
            if (gridManager != null)
            {
                bool isCorrect = gridManager.CheckWord(currentWord);
                if (isCorrect)
                {
                    foreach (var l in selectedLetters)
                        l.SetCorrect(); // Doğru kelimeyi bulunca yeşil yap
                }
            }
        }

        isDragging = false;
        currentWord = "";
        selectedLetters.Clear();
        if (currentWordText != null)
            currentWordText.text = "";
    }

    public void ResetLetters()
    {
        foreach (var l in lettersList)
            l.ResetSelection();
    }
}
