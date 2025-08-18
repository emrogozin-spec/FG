using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomImagePopup : MonoBehaviour
{
    [Header("�������� ����������� (������������)")]
    public Image clickableImage;  // ���� ���������� Image, �� ������� ����� �������

    [Header("���� ��������� ��������")]
    public Image popupPanel;      // ���� ������ (���������� ����)
    public Image greenButton;     // ���� ������ ������ (Image)
    public Image redButton;       // ���� ������� ������ (Image)

    void Start()
    {
        // �������� ������ � ������ ��� ������
        if (popupPanel != null) popupPanel.gameObject.SetActive(false);
        if (greenButton != null) greenButton.gameObject.SetActive(false);
        if (redButton != null) redButton.gameObject.SetActive(false);

        // ������ �������� ����������� ������������
        AddClickToImage();

        // ����������� ������
        SetupButtons();
    }

    void AddClickToImage()
    {
        // ���� ��� Image � ������
        if (clickableImage == null)
        {
            Debug.LogError("�� ��������� ����������� ��� �����!");
            return;
        }

        // ��������� EventTrigger, ���� ��� ���
        var eventTrigger = clickableImage.gameObject.GetComponent<EventTrigger>() ??
                          clickableImage.gameObject.AddComponent<EventTrigger>();

        // ������ ������� �����
        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { TogglePopup(); });

        // ��������� �������
        eventTrigger.triggers.Add(entry);
    }

    void SetupButtons()
    {
        // ������ ������ ������ ������������
        if (greenButton != null)
        {
            var greenBtn = greenButton.gameObject.GetComponent<Button>() ??
                          greenButton.gameObject.AddComponent<Button>();
            greenBtn.onClick.AddListener(() => popupPanel.gameObject.SetActive(false));
        }

        // ������ ������� ������ ������������
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