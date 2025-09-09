using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterCircleManager : MonoBehaviour
{
    public GameObject letterButtonPrefab;
    public Transform circleParent;
    public float radius = 150f;
    public Button submitButton;
    public Button clearButton;
    public TextMeshProUGUI currentWordText; // Seçilen harfleri göstermek için

    private List<Button> letterButtons = new List<Button>();
    private string currentWord = "";
    private WordGridManager gridManager;

    void Start()
    {
        // Null kontrolü ekle
        if (letterButtonPrefab == null)
        {
            Debug.LogError("LetterButtonPrefab atanmamış! Inspector'dan prefab'ı sürükleyip bırakın.");
            return;
        }

        if (circleParent == null)
        {
            Debug.LogError("CircleParent atanmamış! Inspector'dan parent transform'u sürükleyip bırakın.");
            return;
        }

        gridManager = Object.FindAnyObjectByType<WordGridManager>();

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitWord);
        else
            Debug.LogError("SubmitButton atanmamış!");

        if (clearButton != null)
            clearButton.onClick.AddListener(ClearSelection);
        else
            Debug.LogError("ClearButton atanmamış!");
    }

    public void CreateCircle(string letters)
    {
        // Null kontrolü
        if (letterButtonPrefab == null || circleParent == null)
        {
            Debug.LogError("LetterButtonPrefab veya CircleParent atanmamış!");
            return;
        }

        // Öncekileri temizle
        foreach (var btn in letterButtons)
        {
            if (btn != null && btn.gameObject != null)
                Destroy(btn.gameObject);
        }
        letterButtons.Clear();

        // Daireye harfleri yerleştir
        int count = letters.Length;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            GameObject newBtn = Instantiate(letterButtonPrefab, circleParent);
            if (newBtn == null)
            {
                Debug.LogError("Button oluşturulamadı!");
                continue;
            }

            RectTransform rt = newBtn.GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError("Button RectTransform bileşeni yok!");
                continue;
            }

            float angle = i * angleStep * Mathf.Deg2Rad;
            rt.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            TextMeshProUGUI txt = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = letters[i].ToString();
            }
            else
            {
                Debug.LogError("Button içinde TextMeshProUGUI bileşeni yok!");
            }

            Button btn = newBtn.GetComponent<Button>();
            if (btn != null)
            {
                char letter = letters[i];
                btn.onClick.AddListener(() => OnLetterClicked(letter));
                letterButtons.Add(btn);
            }
            else
            {
                Debug.LogError("Button bileşeni yok!");
            }
        }
    }

    void OnLetterClicked(char letter)
    {
        currentWord += letter;
        if (currentWordText != null)
            currentWordText.text = currentWord;
        else
            Debug.LogError("CurrentWordText atanmamış!");

        Debug.Log("Seçilen kelime: " + currentWord);
    }

    void SubmitWord()
    {
        if (string.IsNullOrEmpty(currentWord))
        {
            Debug.Log("Kelime boş!");
            return;
        }

        if (gridManager != null)
        {
            if (gridManager.CheckWord(currentWord))
            {
                Debug.Log("Doğru kelime bulundu: " + currentWord);
            }
            else
            {
                Debug.Log("Yanlış kelime: " + currentWord);
            }
        }
        else
        {
            Debug.LogError("GridManager bulunamadı!");
        }

        ClearSelection();
    }

    public void ClearSelection()
    {
        currentWord = "";
        if (currentWordText != null)
            currentWordText.text = "";
        else
            Debug.LogError("CurrentWordText atanmamış!");

        if (gridManager != null)
            gridManager.ClearSelection();
        else
            Debug.LogError("GridManager bulunamadı!");
    }
}