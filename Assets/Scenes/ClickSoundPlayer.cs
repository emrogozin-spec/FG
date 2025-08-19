using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ClickSoundPlayer : MonoBehaviour, IPointerClickHandler
{
    [Header("Sound Settings")]
    public AudioClip clickSound;
    
    [Range(0f, 1f)]
    public float volume = 1f;
    
    private AudioSource audioSource;

    void Start()
    {
        // Гарантируем, что Image будет принимать клики
        GetComponent<Image>().raycastTarget = true;
        
        // Создаем или получаем AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    // Вызывается при клике на объект
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickSound();
    }

    // Воспроизведение звука
    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
    }

    // Метод для изменения звука программно
    public void SetClickSound(AudioClip newSound, float newVolume = 1f)
    {
        clickSound = newSound;
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    // Метод для принудительного воспроизведения звука из других скриптов
    public void PlaySound()
    {
        PlayClickSound();
    }
}