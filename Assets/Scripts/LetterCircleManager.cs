using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterCircleManager : MonoBehaviour
{
    public GameObject letterButtonPrefab;
    public Transform circleParent;
    public float radius = 150f;
    public Button submitButton;
    public Button clearButton;

    private List<Button> letterButtons = new List<Button>();
    private string currentWord = "";
    private WordGridManager gridManager;

    void Start()
    {
        gridManager = Object.FindAnyObjectByType<WordGridManager>();

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitWord);

        if (clearButton != null)
            clearButton.onClick.AddListener(ClearSelection);
    }

    public void CreateCircle(string letters)
    {
        foreach (var btn in letterButtons) Destroy(btn.gameObject);
        letterButtons.Clear();

        int count = letters.Length;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            GameObject btnObj = Instantiate(letterButtonPrefab, circleParent);
            RectTransform rt = btnObj.GetComponent<RectTransform>();
            float angle = i * angleStep * Mathf.Deg2Rad;
            rt.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            btnObj.GetComponentInChildren<Text>().text = letters[i].ToString();
            char letter = letters[i];
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnLetterClicked(letter));
            letterButtons.Add(btn);
        }
    }

    void OnLetterClicked(char letter)
    {
        currentWord += letter;
        Debug.Log("Seçilen kelime: " + currentWord);
    }

    void SubmitWord()
    {
        if (!string.IsNullOrEmpty(currentWord) && gridManager != null)
        {
            if (gridManager.CheckWord(currentWord))
                Debug.Log("Doğru kelime bulundu: " + currentWord);
            else
                Debug.Log("Yanlış kelime: " + currentWord);
        }
        currentWord = "";
    }

    public void ClearSelection()
    {
        currentWord = "";
        gridManager?.ClearSelection();
    }
}
