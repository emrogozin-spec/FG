using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarVolumeController : MonoBehaviour
{
    [Header("Настройки звука")]
    public string volumeParameter = "MasterVolume";
    
    [Header("Сохранение настроек")]
    public string saveKey = "MasterVolume";
    public float defaultValue = 0.75f;

    private Scrollbar volumeScrollbar;
    private bool isInitialized = false;

    void Start()
    {
        volumeScrollbar = GetComponent<Scrollbar>();
        
        // Добавляем обработчик изменения значения
        volumeScrollbar.onValueChanged.AddListener(OnVolumeChanged);
        
        // Загружаем сохраненное значение
        LoadVolume();
        
        isInitialized = true;
    }

    // Обработчик изменения значения Scrollbar
    private void OnVolumeChanged(float value)
    {
        if (!isInitialized) return;
        
        SetVolume(value);
        SaveVolume(value);
    }

    // Установка громкости
    public void SetVolume(float volume)
    {
        // Устанавливаем громкость для всех AudioSource
        AudioListener.volume = volume;
        
        Debug.Log($"Громкость установлена: {volume}");
    }

    // Сохранение громкости
    private void SaveVolume(float volume)
    {
        PlayerPrefs.SetFloat(saveKey, volume);
        PlayerPrefs.Save();
    }

    // Загрузка громкости
    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(saveKey, defaultValue);
            volumeScrollbar.value = savedVolume;
            SetVolume(savedVolume);
        }
        else
        {
            volumeScrollbar.value = defaultValue;
            SetVolume(defaultValue);
        }
    }

    // Сброс к значениям по умолчанию
    public void ResetToDefault()
    {
        volumeScrollbar.value = defaultValue;
        SetVolume(defaultValue);
        SaveVolume(defaultValue);
    }

    // Метод для получения текущей громкости
    public float GetCurrentVolume()
    {
        return volumeScrollbar.value;
    }

    // Увеличение громкости
    public void IncreaseVolume(float amount = 0.1f)
    {
        float newVolume = Mathf.Clamp01(volumeScrollbar.value + amount);
        volumeScrollbar.value = newVolume;
    }

    // Уменьшение громкости
    public void DecreaseVolume(float amount = 0.1f)
    {
        float newVolume = Mathf.Clamp01(volumeScrollbar.value - amount);
        volumeScrollbar.value = newVolume;
    }
}