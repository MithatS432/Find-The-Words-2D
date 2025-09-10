using System.Collections;
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
    public WordGridManager gridManager;

    private List<GameObject> letterButtons = new List<GameObject>();
    private string currentWord = "";
    private bool isDragging = false;
    private GameObject lastSelectedLetter = null;

    void Start()
    {
        // Örnek harf kümesi - LevelManager'dan alınmalı
        string letters = "PROBLEMRE"; // PROBLEM ve MORE kelimeleri için harfler
        CreateCircle(letters);
    }

    public void CreateCircle(string letters)
    {
        // Öncekileri temizle
        foreach (GameObject btn in letterButtons)
        {
            Destroy(btn);
        }
        letterButtons.Clear();

        // Daireye harfleri yerleştir
        int count = letters.Length;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            GameObject letterBtn = Instantiate(letterButtonPrefab, circleParent);
            letterButtons.Add(letterBtn);

            RectTransform rt = letterBtn.GetComponent<RectTransform>();
            float angle = i * angleStep * Mathf.Deg2Rad;
            rt.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            TextMeshProUGUI text = letterBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = letters[i].ToString();
            }

            // Her harfe tıklanabilirlik özelliği ekle
            Letter letterComponent = letterBtn.GetComponent<Letter>();
            if (letterComponent == null)
            {
                letterComponent = letterBtn.AddComponent<Letter>();
            }
            letterComponent.letter = letters[i];
            letterComponent.circleManager = this;
        }
    }

    public void OnLetterSelected(char letter, GameObject letterObject)
    {
        if (lastSelectedLetter != letterObject)
        {
            currentWord += letter;
            UpdateCurrentWordDisplay();
            lastSelectedLetter = letterObject;
            
            // Seçilen harfi vurgula
            letterObject.GetComponent<Image>().color = Color.yellow;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        currentWord = "";
        UpdateCurrentWordDisplay();
        ResetLetterColors();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Sürükleme sırasında hangi harflerin üzerinden geçtiğimizi kontrol et
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (RaycastResult result in results)
        {
            Letter letter = result.gameObject.GetComponent<Letter>();
            if (letter != null && letter.circleManager == this)
            {
                letter.OnPointerEnter(null);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging && !string.IsNullOrEmpty(currentWord))
        {
            // Kelimeyi kontrol et
            if (gridManager != null)
            {
                gridManager.CheckWord(currentWord);
            }
            
            // Oyun bitti mi kontrol et
            if (gridManager.AreAllWordsFound())
            {
                Debug.Log("Tebrikler! Tüm kelimeleri buldunuz!");
            }
        }
        
        isDragging = false;
        currentWord = "";
        UpdateCurrentWordDisplay();
        ResetLetterColors();
    }

    void UpdateCurrentWordDisplay()
    {
        if (currentWordText != null)
        {
            currentWordText.text = currentWord;
        }
    }

    void ResetLetterColors()
    {
        foreach (GameObject letterBtn in letterButtons)
        {
            letterBtn.GetComponent<Image>().color = Color.white;
        }
    }
}

// Harf nesneleri için yardımcı sınıf
public class Letter : MonoBehaviour, IPointerEnterHandler
{
    public char letter;
    public LetterCircleManager circleManager;
    private bool isSelected = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (circleManager != null && !isSelected)
        {
            circleManager.OnLetterSelected(letter, gameObject);
            isSelected = true;
        }
    }

    void Update()
    {
        // Her frame'de seçili olma durumunu sıfırla (yeni sürükleme için)
        isSelected = false;
    }
}