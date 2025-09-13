using UnityEngine;
using UnityEngine.UI;

public class LevelMenuUI : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int capturedIndex = i + 1;

            // Sadece açılmış seviyeleri aktif yap
            levelButtons[i].interactable = capturedIndex <= unlockedLevel;

            // Button tıklandığında LevelManager aracılığıyla level yükle
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() =>
            {
                LevelManager.Instance.LoadLevel(capturedIndex);
            });
        }
    }
}
