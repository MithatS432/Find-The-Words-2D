using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Letter : MonoBehaviour, IPointerEnterHandler
{
    public char letter;
    public LetterCircleManager circleManager;
    private bool isSelected = false;
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            isSelected = true;
            img.color = Color.yellow; // Sürükle-bırak sırasında sarı olacak
            circleManager.OnLetterSelected(this);
        }
    }

    public void SetCorrect()  // Kelime doğruysa yeşil
    {
        if (img != null)
            img.color = Color.green;
    }

    public void ResetSelection()  // Yeni kelime için sıfırlama
    {
        isSelected = false;
        if (img != null)
            img.color = Color.white;
    }
}
