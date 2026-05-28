using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioManagerBridge : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        if (slider == null)
            return;

        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        SyncFromGlobalVolume();
    }

    void Start()
    {
        SyncFromGlobalVolume();
    }

    private void SyncFromGlobalVolume()
    {
        if (slider == null)
            return;

        if (AudioManager.instance != null)
            slider.SetValueWithoutNotify(AudioManager.instance.GetVolume());
    }

    void OnDisable()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(value);
            return;
        }

        AudioListener.volume = Mathf.Clamp01(value);
        Debug.Log("AudioManagerBridge fallback volume: " + AudioListener.volume);
    }
}
