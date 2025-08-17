using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))] // Требуем наличие коллайдера
public class ClickToChangeScene : MonoBehaviour
{
    [SerializeField] private string targetSceneName; // Имя сцены для загрузки
    [SerializeField] private bool usePlayerPrefs = false; // Сохранять ли данные через PlayerPrefs
    [SerializeField] private string loadingScreenScene = ""; // Сцена загрузки (опционально)

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            if (usePlayerPrefs)
            {
                PlayerPrefs.Save(); // Сохраняем данные перед сменой сцены
            }

            if (!string.IsNullOrEmpty(loadingScreenScene))
            {
                // Загружаем сцену загрузки и передаем имя целевой сцены
                PlayerPrefs.SetString("NextScene", targetSceneName);
                SceneManager.LoadScene(loadingScreenScene);
            }
            else
            {
                // Загружаем целевую сцену напрямую
                SceneManager.LoadScene(targetSceneName);
            }
        }
        else
        {
            Debug.LogWarning("Target scene name is not set!", this);
        }
    }

    // Проверяем настройки в редакторе
    private void OnValidate()
    {
        // Убедимся, что коллайдер включен и является триггером
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("No Collider2D found on this GameObject!", this);
        }
    }
}