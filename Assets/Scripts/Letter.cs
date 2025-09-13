using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Letter : MonoBehaviour, IPointerClickHandler
{
    private char letter; // 🔹 private yaptık, dışarıdan SetupLetter ile atanacak
    private bool isSelected = false;
    private bool isInteractable = true;
    private Image img;
    private LetterCircleManager circleManager;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = Color.white; // daima beyaz
    }

    // 🔹 Harfi ve manager'ı dışarıdan ayarlamak için
    public void SetupLetter(char l, LetterCircleManager manager)
    {
        letter = l;
        circleManager = manager;
    }

    // 🔹 Harfi dışarıya döndüren getter
    public char GetLetter()
    {
        return letter;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelected && isInteractable)
        {
            SelectLetter();
        }
    }

    public void SelectLetter()
    {
        isSelected = true;
        img.color = Color.yellow; 
        circleManager.OnLetterSelected(this);
    }

    public void SetCorrect()
    {
        isSelected = false;
        img.color = Color.white;
    }

    public void ResetSelection()
    {
        isSelected = false;
        img.color = Color.white;
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;
        img.color = value ? Color.white : new Color(1,1,1,0.3f); 
    }
}
