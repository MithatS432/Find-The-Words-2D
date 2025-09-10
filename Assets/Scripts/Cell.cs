using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int x, y;
    public WordGridManager gridManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        gridManager.OnCellClicked(x, y);
    }
}
