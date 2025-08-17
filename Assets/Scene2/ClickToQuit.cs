using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ClickToQuit : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float delayBeforeQuit = 0.5f; // Задержка перед выходом
    [SerializeField] private AudioClip clickSound; // Звук при нажатии
    [SerializeField] private bool showConfirmation = true; // Показывать ли подтверждение

    private void OnMouseDown()
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
        }

        if (showConfirmation)
        {
            StartCoroutine(QuitWithConfirmation());
        }
        else
        {
            StartCoroutine(QuitWithDelay());
        }
    }

    private IEnumerator QuitWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeQuit);
        QuitApplication();
    }

    private IEnumerator QuitWithConfirmation()
    {
        // Здесь можно добавить UI подтверждения
        Debug.Log("Вы уверены, что хотите выйти?");
        // В реальном проекте здесь будет вызов UI окна подтверждения
        
        // Временное решение - ждем 1 секунду как будто игрок подтверждает
        yield return new WaitForSeconds(1f);
        
        QuitApplication();
    }

    private void QuitApplication()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnValidate()
    {
        // Автоматически добавляем коллайдер если его нет
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Убедимся что коллайдер является триггером
        GetComponent<Collider2D>().isTrigger = true;
    }
}