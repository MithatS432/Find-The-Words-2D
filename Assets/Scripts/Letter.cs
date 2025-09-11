using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Letter : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public char letter;
    public LetterCircleManager circleManager;
    private bool isSelected = false;
    private Image img;
    private Button button;

    void Awake()
    {
        img = GetComponent<Image>();
        button = GetComponent<Button>();
        ResetSelection();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (circleManager != null && circleManager.IsDragging && !isSelected)
        {
            SelectLetter();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Tıklamayla da harf seçilebilir
        if (!circleManager.IsDragging)
        {
            circleManager.OnPointerDown(eventData);
            SelectLetter();
        }
    }

    public void SelectLetter()
    {
        isSelected = true;
        img.color = Color.yellow;
        
        if (circleManager != null)
        {
            circleManager.OnLetterSelected(this);
        }
    }

    public void SetCorrect()
    {
        isSelected = true;
        img.color = Color.green;
    }

    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    public void ResetSelection()
    {
        isSelected = false;
        img.color = Color.white;
        
        if (button != null)
        {
            button.interactable = true;
        }
    }
}