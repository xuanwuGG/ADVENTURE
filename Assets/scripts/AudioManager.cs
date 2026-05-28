using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    const string PREF_VOLUME = "masterVolume";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        new GameObject(nameof(AudioManager)).AddComponent<AudioManager>();
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

        float v = PlayerPrefs.GetFloat(PREF_VOLUME, 1f);
        AudioListener.volume = v;
    }

    public void SetVolume(float v)
    {
        v = Mathf.Clamp01(v);
        AudioListener.volume = v;
        PlayerPrefs.SetFloat(PREF_VOLUME, v);
        PlayerPrefs.Save();
        Debug.Log("AudioManager.SetVolume: " + v);
    }

    public float GetVolume() => AudioListener.volume;
}
