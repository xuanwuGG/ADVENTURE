using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string gameSceneName = "SampleScene"; // 可在 Inspector 配置
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    void Start()
    {
        // 初始化面板状态
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        // 如果有存档则载入，否则新游戏
        if (SaveManager.HasSave())
        {
            LoadGame();
        }
        else
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        SaveManager.DeleteSave();
        // 可在这里做一些默认重置（gameManager 会在场景加载后初始化）
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadGame()
    {
        // 载入存档数据后打开场景
        if (!SaveManager.HasSave())
        {
            Debug.Log("没有存档，启动新游戏");
            NewGame();
            return;
        }

        // 先加载游戏场景，再在场景加载完成后应用存档
        SceneManager.LoadScene(gameSceneName);
        // 在游戏场景的 Start 或某处应调用 SaveManager.ApplyLoadedDataIfAny();
    }

    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
