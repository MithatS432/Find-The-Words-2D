using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LevelData = LevelManager.LevelData;


public class LetterCircleManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI Referansları")]
    public GameObject letterButtonPrefab;
    public Transform circleParent;
    public TextMeshProUGUI currentWordText;

    [Header("Ayarlar")]
    public float radius = 150f;
    public LayerMask letterLayerMask = -1; // Tüm layer'lar

    private List<Letter> lettersList = new List<Letter>();
    private List<Letter> selectedLetters = new List<Letter>();
    private string currentWord = "";
    private bool isDragging = false;
    private GraphicRaycaster graphicRaycaster;

    public bool IsDragging => isDragging;

    void Start()
    {
        // GraphicRaycaster'ı al (UI elementleri için gerekli)
        graphicRaycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();

        var levelData = LevelManager.Instance.GetCurrentLevelData();
        SetupLetters(levelData);
    }

    void Update()
    {
        // Debug için
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log($"Current word: '{currentWord}' | Selected: {selectedLetters.Count} | Dragging: {isDragging}");
        }
    }

    void SetupLetters(LevelData levelData)
    {
        // Kelimelerdeki harfleri topla
        HashSet<char> uniqueLetters = new HashSet<char>();
        foreach (var word in levelData.words)
        {
            foreach (char c in word.ToUpper())
            {
                if (char.IsLetter(c))
                    uniqueLetters.Add(c);
            }
        }

        // Ana harfleri string'e çevir
        string letters = new string(new List<char>(uniqueLetters).ToArray());

        // Extra harfleri ekle
        string extraLetters = GenerateExtraLetters(levelData.extraLettersCount);
        string allLetters = letters + extraLetters;

        // Harfleri karıştır
        allLetters = ShuffleString(allLetters);

        CreateCircle(allLetters);
    }

    public void CreateCircle(string letters)
    {
        // Eski harfleri temizle
        foreach (Transform child in circleParent)
            Destroy(child.gameObject);
        lettersList.Clear();

        int count = letters.Length;
        if (count == 0) return;

        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            GameObject btn = Instantiate(letterButtonPrefab, circleParent);
            btn.name = "Letter_" + letters[i];

            // Pozisyon ayarla
            RectTransform rt = btn.GetComponent<RectTransform>();
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            rt.anchoredPosition = pos;

            // Harf metnini ayarla
            TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = letters[i].ToString();

            // Letter component'ini ayarla
            Letter letterComp = btn.GetComponent<Letter>();
            if (letterComp == null)
                letterComp = btn.AddComponent<Letter>();

            letterComp.SetupLetter(letters[i], this);
            lettersList.Add(letterComp);
        }
    }

    string GenerateExtraLetters(int count)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.Random rnd = new System.Random();

        for (int i = 0; i < count; i++)
            sb.Append(alphabet[rnd.Next(alphabet.Length)]);

        return sb.ToString();
    }

    string ShuffleString(string input)
    {
        System.Random rnd = new System.Random();
        char[] array = input.ToCharArray();

        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            char temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        return new string(array);
    }

    public void OnLetterSelected(Letter letter)
    {
        if (selectedLetters.Contains(letter) || !letter.IsInteractable())
            return;

        selectedLetters.Add(letter);
        currentWord += letter.GetLetter();

        UpdateCurrentWordDisplay();

        Debug.Log($"Harf seçildi: {letter.GetLetter()}, Kelime: {currentWord}");
    }

    void UpdateCurrentWordDisplay()
    {
        if (currentWordText != null)
            currentWordText.text = currentWord;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        ClearSelection();

        // İlk harfi seç
        Letter firstLetter = GetLetterAtPosition(eventData.position);
        if (firstLetter != null && firstLetter.IsInteractable())
        {
            firstLetter.SelectLetter();
        }

        Debug.Log("Drag başladı");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Letter currentLetter = GetLetterAtPosition(eventData.position);
        if (currentLetter != null && currentLetter.IsInteractable() && !selectedLetters.Contains(currentLetter))
        {
            currentLetter.SelectLetter();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging && !string.IsNullOrEmpty(currentWord))
        {
            Debug.Log($"Kelime kontrol ediliyor: {currentWord}");
            CheckCurrentWord();
        }

        isDragging = false;
        Debug.Log("Drag bitti");
    }

    Letter GetLetterAtPosition(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Letter letter = result.gameObject.GetComponent<Letter>();
            if (letter != null)
                return letter;
        }

        return null;
    }

    void CheckCurrentWord()
    {
        WordGridManager gridManager = Object.FindAnyObjectByType<WordGridManager>();
        if (gridManager == null)
        {
            Debug.LogError("WordGridManager bulunamadı!");
            ClearSelection();
            return;
        }

        bool isCorrect = gridManager.CheckWord(currentWord);
        Debug.Log($"Kelime kontrol sonucu: {currentWord} = {isCorrect}");

        if (isCorrect)
        {
            // Doğru kelime - harfleri yeşil yap ve devre dışı bırak
            foreach (Letter letter in selectedLetters)
            {
                letter.SetCorrect();
            }

            // Seçimi temizleme - sadece yanlış harfleri sıfırla
            selectedLetters.Clear();
            currentWord = "";
            UpdateCurrentWordDisplay();
        }
        else
        {
            // Yanlış kelime - tümünü sıfırla
            ClearSelection();
        }
    }

    public void ClearSelection()
    {
        // Sadece seçili olan harfleri sıfırla
        foreach (Letter letter in selectedLetters)
        {
            if (letter.IsInteractable())
                letter.ResetSelection();
        }

        selectedLetters.Clear();
        currentWord = "";
        UpdateCurrentWordDisplay();
    }
}