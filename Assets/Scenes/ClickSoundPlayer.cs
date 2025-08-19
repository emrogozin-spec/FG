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

    [Header("Anti-Spam Settings")]
    [Tooltip("Минимальное время между звуками (секунды)")]
    public float minTimeBetweenClicks = 0.1f;
    [Tooltip("Блокировать ли повторные клики во время воспроизведения")]
    public bool blockDuringPlayback = true;

    private AudioSource audioSource;
    private float lastClickTime = -1f;
    private bool isPlaying = false;

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

    // Воспроизведение звука с защитой от спама
    public void PlayClickSound()
    {
        if (CanPlaySound())
        {
            PlaySoundInternal();
        }
    }

    // Проверка возможности воспроизведения
    private bool CanPlaySound()
    {
        if (clickSound == null || audioSource == null)
            return false;

        // Проверка времени между кликами
        if (Time.unscaledTime - lastClickTime < minTimeBetweenClicks)
            return false;

        // Проверка, не воспроизводится ли уже звук (если включена блокировка)
        if (blockDuringPlayback && isPlaying)
            return false;

        return true;
    }

    // Внутреннее воспроизведение звука
    private void PlaySoundInternal()
    {
        isPlaying = true;
        lastClickTime = Time.unscaledTime;

        audioSource.PlayOneShot(clickSound, volume);

        // Запускаем корутину для сброса флага после завершения звука
        StartCoroutine(ResetPlayingFlag(clickSound.length));
    }

    // Корутина для сброса флага воспроизведения
    private System.Collections.IEnumerator ResetPlayingFlag(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        isPlaying = false;
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

    // Метод для принудительного воспроизведения без проверки ограничений
    public void PlaySoundForce()
    {
        if (clickSound != null && audioSource != null)
        {
            PlaySoundInternal();
        }
    }

    // Останавливаем все корутины при уничтожении объекта
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // Останавливаем все корутины при отключении объекта
    private void OnDisable()
    {
        StopAllCoroutines();
        isPlaying = false;
    }
}