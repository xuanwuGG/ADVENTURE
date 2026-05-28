using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManagerBridge : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "mainMEnu";

    public void SaveCurrent()
    {
        if (SaveManager.instance != null)
            SaveManager.instance.SaveCurrent();
    }

    public void Load()
    {
        if (SaveManager.instance != null)
            SaveManager.instance.Load();
    }

    public void DeleteSave()
    {
        SaveManager.DeleteSave();
    }

    public bool HasSave()
    {
        return SaveManager.HasSave();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
