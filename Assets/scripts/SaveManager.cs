using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float px, py; // 玩家位置
    public int hp, mp;
    public int dialogIndex, candleNum, killNum;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public string saveFileName = "save1.json";
    private SaveData loadedData = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        new GameObject(nameof(SaveManager)).AddComponent<SaveManager>();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveCurrent()
    {
        var data = new SaveData();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.px = player.transform.position.x;
            data.py = player.transform.position.y;
        }
        if (gameManager.instance != null)
        {
            data.hp = gameManager.instance.playerCurrentHP;
            data.mp = gameManager.instance.playerCurrentMP;
            data.dialogIndex = gameManager.instance.dialogInfoIndex;
            data.candleNum = gameManager.instance.candleNum;
            data.killNum = gameManager.instance.killNum;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);
        Debug.Log("已保存: " + Application.persistentDataPath);
    }

    public bool Load()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path)) return false;
        string json = File.ReadAllText(path);
        loadedData = JsonUtility.FromJson<SaveData>(json);
        ApplyLoadedDataIfAny();
        return true;
    }

    public static bool HasSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "save1.json");
        return File.Exists(path);
    }

    public void ApplyLoadedDataIfAny()
    {
        if (loadedData == null) return;
        if (gameManager.instance != null)
        {
            gameManager.instance.playerCurrentHP = loadedData.hp;
            gameManager.instance.playerCurrentMP = loadedData.mp;
            gameManager.instance.dialogInfoIndex = loadedData.dialogIndex;
            gameManager.instance.candleNum = loadedData.candleNum;
            gameManager.instance.killNum = loadedData.killNum;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(loadedData.px, loadedData.py, player.transform.position.z);
        }

        loadedData = null; // 只应用一次
    }

    public static void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "save1.json");
        if (File.Exists(path)) File.Delete(path);
    }
}
