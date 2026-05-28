using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject a1;
    public GameObject a2;

    [SerializeField] private string pausePanelName = "pause";
    [SerializeField] private string settingsPanelName = "settingMenu";

    private bool isPaused = false;

    void Awake()
    {
        BindPanelsIfNeeded();
        SetPauseState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Resume()
    {
        SetPauseState(false);
    }

    public void Save()
    {
        SaveManager.instance.SaveCurrent();
    }

    public void Load()
    {
        SaveManager.instance.Load();
    }

    public void OpenSettings()
    {
        SetPauseState(true);
        if (a1 != null) a1.SetActive(false);
        if (a2 != null) a2.SetActive(true);
    }

    public void CloseSettings()
    {
        if (a1 != null) a1.SetActive(true);
        if (a2 != null) a2.SetActive(false);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("mainMEnu");
    }

    private void Pause()
    {
        SetPauseState(true);
        if (mainPanel != null) mainPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void SetPauseState(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
        if (mainPanel != null) mainPanel.SetActive(!pause);
        if (pausePanel != null && !pause) pausePanel.SetActive(false);
        if (settingsPanel != null && !pause) settingsPanel.SetActive(false);
    }

    private void BindPanelsIfNeeded()
    {
        if (pausePanel == null && !string.IsNullOrWhiteSpace(pausePanelName))
        {
            pausePanel = FindSceneObjectByName(pausePanelName);
        }

        if (settingsPanel == null && !string.IsNullOrWhiteSpace(settingsPanelName))
        {
            settingsPanel = FindSceneObjectByName(settingsPanelName);
        }
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        Transform[] transforms = FindObjectsOfType<Transform>(true);
        foreach (Transform candidate in transforms)
        {
            if (candidate.name == objectName)
            {
                return candidate.gameObject;
            }
        }

        return null;
    }
}
