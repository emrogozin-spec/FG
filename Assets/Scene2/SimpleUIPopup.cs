using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomImagePopup : MonoBehaviour
{
    [Header("Основное изображение (кликабельное)")]
    public Image clickableImage;  // Сюда перетащите Image, на которое нужно кликать

    [Header("Ваши кастомные элементы")]
    public Image popupPanel;      // Ваша панель (перетащите сюда)
    public Image greenButton;     // Ваша зелёная кнопка (Image)
    public Image redButton;       // Ваша красная кнопка (Image)

    void Start()
    {
        // Скрываем панель и кнопки при старте
        if (popupPanel != null) popupPanel.gameObject.SetActive(false);
        if (greenButton != null) greenButton.gameObject.SetActive(false);
        if (redButton != null) redButton.gameObject.SetActive(false);

        // Делаем основное изображение кликабельным
        AddClickToImage();

        // Настраиваем кнопки
        SetupButtons();
    }

    void AddClickToImage()
    {
        // Если нет Image — ошибка
        if (clickableImage == null)
        {
            Debug.LogError("Не назначено изображение для клика!");
            return;
        }

        // Добавляем EventTrigger, если его нет
        var eventTrigger = clickableImage.gameObject.GetComponent<EventTrigger>() ??
                          clickableImage.gameObject.AddComponent<EventTrigger>();

        // Создаём событие клика
        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { TogglePopup(); });

        // Добавляем событие
        eventTrigger.triggers.Add(entry);
    }

    void SetupButtons()
    {
        // Делаем зелёную кнопку кликабельной
        if (greenButton != null)
        {
            var greenBtn = greenButton.gameObject.GetComponent<Button>() ??
                          greenButton.gameObject.AddComponent<Button>();
            greenBtn.onClick.AddListener(() => popupPanel.gameObject.SetActive(false));
        }

        // Делаем красную кнопку кликабельной
        if (redButton != null)
        {
            var redBtn = redButton.gameObject.GetComponent<Button>() ??
                        redButton.gameObject.AddComponent<Button>();
            redBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }

    void TogglePopup()
    {
        if (popupPanel == null) return;

        bool shouldShow = !popupPanel.gameObject.activeSelf;
        popupPanel.gameObject.SetActive(shouldShow);

        if (greenButton != null) greenButton.gameObject.SetActive(shouldShow);
        if (redButton != null) redButton.gameObject.SetActive(shouldShow);
    }
}