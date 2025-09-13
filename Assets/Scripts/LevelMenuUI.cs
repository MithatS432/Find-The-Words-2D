using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelMenuUI : MonoBehaviour
{
    public Button[] levelButtons;

    void OnEnable()
    {
        // Coroutine ile sahne tamamen yüklendikten sonra butonları ayarlıyoruz
        StartCoroutine(UpdateButtonsNextFrame());
    }

    IEnumerator UpdateButtonsNextFrame()
    {
        yield return null; // Bir frame bekle, tüm UI nesneleri aktif olsun

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log("UnlockedLevel: " + unlockedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int capturedIndex = i + 1;

            // GameObject aktif değilse aktif et
            if (!levelButtons[i].gameObject.activeInHierarchy)
            {
                levelButtons[i].gameObject.SetActive(true);
                Debug.Log($"Button {capturedIndex} GameObject aktif edildi.");
            }

            // Level açılmış mı kontrol et
            levelButtons[i].interactable = capturedIndex <= unlockedLevel;
            Debug.Log($"Button {capturedIndex} interactable: {levelButtons[i].interactable}");

            // Button tıklama olayını ayarla
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() =>
            {
                LevelManager.Instance.LoadLevel(capturedIndex);
            });
        }
    }
}
