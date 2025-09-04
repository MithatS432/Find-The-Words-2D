using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CanvasManager : MonoBehaviour
{
    public Button level1;
    public Button level2;
    public Button level3;
    public Button level4;
    public Button level5;
    public Button level6;
    public Button level7;
    public Button level8;
    public Button level9;
    public Button level10;
    public Button level11;
    public Button level12;
    public Button level13;
    public Button level14;
    public Button level15;
    void Start()
    {
        level1.onClick.AddListener(() => LoadLevel("1"));
        level2.onClick.AddListener(() => LoadLevel("2"));
        level3.onClick.AddListener(() => LoadLevel("3"));
        level4.onClick.AddListener(() => LoadLevel("4"));
        level5.onClick.AddListener(() => LoadLevel("5"));
        level6.onClick.AddListener(() => LoadLevel("6"));
        level7.onClick.AddListener(() => LoadLevel("7"));
        level8.onClick.AddListener(() => LoadLevel("8"));
        level9.onClick.AddListener(() => LoadLevel("9"));
        level10.onClick.AddListener(() => LoadLevel("10"));
        level11.onClick.AddListener(() => LoadLevel("11"));
        level12.onClick.AddListener(() => LoadLevel("12"));
        level13.onClick.AddListener(() => LoadLevel("13"));
        level14.onClick.AddListener(() => LoadLevel("14"));
        level15.onClick.AddListener(() => LoadLevel("15"));
    }
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
