using UnityEngine;
using UnityEngine.UI;

public class WordGridManager : MonoBehaviour
{
    [Header("Grid Ayarları")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private int numberOfWords = 3;

    [Header("UI Ayarları")]
    [SerializeField] private GameObject cellPrefab;   // Kutucuk prefab
    [SerializeField] private Transform gridParent;    // Kutucukların oluşacağı yer
    [SerializeField] private float cellSpacing = 5f;  // Kutucuklar arası boşluk
    [SerializeField] private Vector2 cellSize = new Vector2(80, 80); // Her kutucuğun boyutu

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        if (cellPrefab == null || gridParent == null)
        {
            Debug.LogError("CellPrefab veya GridParent atanmadı!");
            return;
        }

        // Grid boyutuna göre kutucuk oluştur
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject newCell = Instantiate(cellPrefab, gridParent);
                RectTransform rt = newCell.GetComponent<RectTransform>();

                // Pozisyon hesapla
                rt.sizeDelta = cellSize;
                rt.anchoredPosition = new Vector2(
                    x * (cellSize.x + cellSpacing),
                    -y * (cellSize.y + cellSpacing)
                );

                newCell.name = $"Cell_{x}_{y}";
            }
        }

        Debug.Log($"Grid oluşturuldu: {gridWidth}x{gridHeight}, {numberOfWords} kelime olacak.");
    }
}
