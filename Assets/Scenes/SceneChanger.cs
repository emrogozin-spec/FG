using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneChanger : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Имя сцены для загрузки")]
    public string sceneName;
    
    [Tooltip("Должна ли быть задержка перед загрузкой сцены")]
    public bool useDelay = false;
    
    [Tooltip("Задержка в секундах"), Min(0)]
    public float delayTime = 1f;
    
    [Tooltip("Эффект затемнения при переходе (опционально)")]
    public Animator fadeAnimator;
    
    private static bool isLoading = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isLoading)
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        if (isLoading) return;
        
        isLoading = true;
        
        // Если есть аниматор, запускаем анимацию затемнения
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
        }
        
        if (useDelay)
        {
            Invoke("LoadScene", delayTime);
        }
        else
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not specified!");
            isLoading = false;
        }
    }

    // Альтернативный метод для вызова через UnityEvent
    public void ChangeScene(string newSceneName)
    {
        sceneName = newSceneName;
        ChangeScene();
    }
}