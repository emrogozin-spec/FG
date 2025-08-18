using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageSceneTrigger : MonoBehaviour, IPointerClickHandler
{
    public string targetSceneName;

    void Start()
    {
        // ������������� �������� Raycast Target
        Image img = GetComponent<Image>();
        if (img != null) img.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // ���� �������� �� ����������, ������ ���
            if (SceneFadeManager.Instance == null)
            {
                GameObject managerObj = new GameObject("SceneFadeManager");
                managerObj.AddComponent<SceneFadeManager>();
            }

            SceneFadeManager.Instance.LoadSceneWithFade(targetSceneName);
        }
    }
}