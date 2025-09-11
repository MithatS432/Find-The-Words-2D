using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    [Header("Renkler")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color correctColor = Color.green;
    
    private char letter;
    private LetterCircleManager circleManager;
    private bool isSelected = false;
    private bool isInteractable = true;
    
    private Image img;
    private Button button;

    void Awake()
    {
        img = GetComponent<Image>();
        button = GetComponent<Button>();
        
        // Button click event'ini kaldır (drag ile kontrol edeceğiz)
        if (button != null)
            button.onClick.RemoveAllListeners();
    }

    public void SetupLetter(char letterChar, LetterCircleManager manager)
    {
        letter = letterChar;
        circleManager = manager;
        ResetSelection();
    }

    public char GetLetter()
    {
        return letter;
    }

    public void SelectLetter()
    {
        if (!isInteractable || isSelected) 
            return;

        isSelected = true;
        
        if (img != null)
            img.color = selectedColor;

        // Circle manager'a bildir
        if (circleManager != null)
            circleManager.OnLetterSelected(this);
    }

    public void SetCorrect()
    {
        isSelected = true;
        isInteractable = false;
        
        if (img != null)
            img.color = correctColor;
        
        if (button != null)
            button.interactable = false;
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        
        if (button != null)
            button.interactable = interactable;
            
        if (!interactable && img != null)
            img.color = correctColor;
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void ResetSelection()
    {
        if (!isInteractable) 
            return;
            
        isSelected = false;
        
        if (img != null)
            img.color = normalColor;
    }

    // Debug için
    void OnValidate()
    {
        if (img == null)
            img = GetComponent<Image>();
    }
}