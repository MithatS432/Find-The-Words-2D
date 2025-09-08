using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterCircleManager : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject letterButtonPrefab;
    public Transform circleParent;
    public float radius = 150f;

    [HideInInspector] public string currentWord = "";
    private List<Button> letterButtons = new List<Button>();
    private WordGridManager gridManager;

    void Start()
    {
        gridManager = FindAnyObjectByType<WordGridManager>();
    }

    public void SetupLetters(string letters)
    {
        // önceki harfleri temizle
        foreach (var btn in letterButtons)
        {
            Destroy(btn.gameObject);
        }
        letterButtons.Clear();
        currentWord = "";

        // daireye harfleri yerleştir
        float angleStep = 360f / letters.Length;
        for (int i = 0; i < letters.Length; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            GameObject newBtnObj = Instantiate(letterButtonPrefab, circleParent);
            RectTransform rt = newBtnObj.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;

            Button btn = newBtnObj.GetComponent<Button>();
            Text txt = newBtnObj.GetComponentInChildren<Text>();
            txt.text = letters[i].ToString();

            int index = i;
            btn.onClick.AddListener(() => OnLetterClicked(letters[index]));

            letterButtons.Add(btn);
        }
    }

    void OnLetterClicked(char c)
    {
        currentWord += c;
        Debug.Log("Seçilen kelime: " + currentWord);
    }

    public void SubmitWord()
    {
        if (gridManager != null)
        {
            gridManager.CheckSubmittedWord(currentWord);
        }
        currentWord = "";
    }
}
