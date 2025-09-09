using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int x, y;
    public WordGridManager gridManager;

    void Start()
    {
        // GridManager bulunamazsa bulmaya çalış
        if (gridManager == null)
            gridManager = Object.FindAnyObjectByType<WordGridManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gridManager != null)
            gridManager.OnCellClicked(x, y);
        else
            Debug.LogError("GridManager atanmamış!");
    }
}