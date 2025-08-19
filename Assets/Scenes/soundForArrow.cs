using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class soundForArrow : MonoBehaviour, IPointerClickHandler
{
    [Header("Sound Settings")]
    public AudioClip[] clickSounds; // Массив звуков для чередования
    
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Header("Cooldown Settings")]
    public float cooldownTime = 1f; // Время задержки между кликами
    public bool waitForSoundFinish = true; // Ждать окончания звука
    
    [Header("Play Order")]
    public bool resetIndexOnStart = true; // Сбрасывать индекс при старте
    public int startIndex = 0; // Начальный индекс
    
    private AudioSource audioSource;
    private int currentSoundIndex = 0;
    private bool canPlaySound = true;
    private float lastPlayTime;

    void Start()
    {
        // Проверяем есть ли Image компонент и настраиваем его
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }
        else
        {
            // Если нет Image, добавляем его
            image = gameObject.AddComponent<Image>();
            image.raycastTarget = true;
            image.color = new Color(1, 1, 1, 0.01f); // Почти прозрачный
            Debug.Log("Добавлен Image компонент на объект " + gameObject.name);
        }
        
        // Настраиваем AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        
        // Сбрасываем индекс звука при старте
        if (resetIndexOnStart)
        {
            currentSoundIndex = Mathf.Clamp(startIndex, 0, clickSounds != null ? clickSounds.Length - 1 : 0);
            Debug.Log("Индекс звука сброшен на: " + currentSoundIndex);
        }
    }

    void Update()
    {
        // Проверяем, можно ли снова воспроизводить звук
        if (!canPlaySound)
        {
            if (waitForSoundFinish)
            {
                // Ждем окончания звука И прошло время кд
                if (!audioSource.isPlaying && Time.time - lastPlayTime >= cooldownTime)
                {
                    canPlaySound = true;
                }
            }
            else
            {
                // Ждем только время кд
                if (Time.time - lastPlayTime >= cooldownTime)
                {
                    canPlaySound = true;
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canPlaySound)
        {
            PlayNextSound();
        }
    }

    public void PlayNextSound()
    {
        // Проверяем все возможные причины ошибки
        if (clickSounds == null || clickSounds.Length == 0)
        {
            Debug.LogWarning("Массив clickSounds пустой или не назначен!");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource не найден!");
            return;
        }

        if (!canPlaySound)
        {
            Debug.Log("Звук еще на кулдауне");
            return;
        }

        // Проверяем текущий звук
        if (clickSounds[currentSoundIndex] == null)
        {
            Debug.LogWarning("Звук с индексом " + currentSoundIndex + " не назначен!");
            // Переходим к следующему звуку
            currentSoundIndex = (currentSoundIndex + 1) % clickSounds.Length;
            return;
        }

        Debug.Log("Воспроизводим звук с индексом: " + currentSoundIndex);
        
        // Воспроизводим текущий звук
        audioSource.PlayOneShot(clickSounds[currentSoundIndex], volume);
        
        // Обновляем индекс
        currentSoundIndex = (currentSoundIndex + 1) % clickSounds.Length;
        Debug.Log("Следующий индекс: " + currentSoundIndex);
        
        // Устанавливаем задержку
        canPlaySound = false;
        lastPlayTime = Time.time;
        
        // Если не ждем окончания звука, но звук очень длинный - ограничиваем кд
        if (!waitForSoundFinish && cooldownTime < clickSounds[currentSoundIndex].length)
        {
            cooldownTime = clickSounds[currentSoundIndex].length + 0.1f;
        }
    }

    // Сбросить индекс к начальному
    public void ResetSoundIndex()
    {
        currentSoundIndex = Mathf.Clamp(startIndex, 0, clickSounds != null ? clickSounds.Length - 1 : 0);
        Debug.Log("Индекс звука сброшен на: " + currentSoundIndex);
    }

    // Установить конкретный индекс
    public void SetSoundIndex(int index)
    {
        if (clickSounds != null && clickSounds.Length > 0)
        {
            currentSoundIndex = Mathf.Clamp(index, 0, clickSounds.Length - 1);
            Debug.Log("Индекс звука установлен на: " + currentSoundIndex);
        }
    }

    public void PlaySoundAtIndex(int index)
    {
        if (clickSounds != null && clickSounds.Length > 0 && audioSource != null && canPlaySound)
        {
            int safeIndex = Mathf.Clamp(index, 0, clickSounds.Length - 1);
            
            if (clickSounds[safeIndex] != null)
            {
                audioSource.PlayOneShot(clickSounds[safeIndex], volume);
                currentSoundIndex = (safeIndex + 1) % clickSounds.Length;
                
                canPlaySound = false;
                lastPlayTime = Time.time;
            }
        }
    }

    // Принудительно разрешить воспроизведение звука
    public void EnableSoundPlay()
    {
        canPlaySound = true;
    }

    // Принудительно запретить воспроизведение звука
    public void DisableSoundPlay()
    {
        canPlaySound = false;
    }

    // Проверить, можно ли воспроизводить звук
    public bool CanPlaySound()
    {
        return canPlaySound;
    }

    // Получить текущий индекс
    public int GetCurrentSoundIndex()
    {
        return currentSoundIndex;
    }

    // Установить время задержки
    public void SetCooldownTime(float newCooldown)
    {
        cooldownTime = Mathf.Max(0.1f, newCooldown);
    }

    public void SetClickSounds(AudioClip[] newSounds, float newVolume = 1f)
    {
        clickSounds = newSounds;
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        currentSoundIndex = Mathf.Clamp(startIndex, 0, clickSounds != null ? clickSounds.Length - 1 : 0);
    }
}