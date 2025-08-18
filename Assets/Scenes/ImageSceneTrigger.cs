using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Добавлено это пространство имен

[RequireComponent(typeof(Image))]
public class ImageSceneTrigger : MonoBehaviour, IPointerClickHandler
{
    public string targetSceneName;

    void Start()
    {
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneFadeManager fadeManager = FindObjectOfType<SceneFadeManager>();

            if (fadeManager != null)
            {
                fadeManager.LoadScene(targetSceneName);
            }
            else
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}