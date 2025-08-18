using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFadeManager : MonoBehaviour
{
    public static SceneFadeManager Instance;

    [Header("Настройки")]
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;

    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        // Если Instance уже существует, уничтожаем дубликат
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Инициализируем затемнение, если ещё не сделано
        if (fadeImage == null)
        {
            CreateFadeOverlay();
        }

        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Отписываемся при уничтожении
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // При загрузке новой сцены запускаем плавное появление
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
            StartCoroutine(Fade(1f, 0f));
        }
    }

    void CreateFadeOverlay()
    {
        // Создаём Canvas для затемнения
        fadeCanvas = gameObject.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;

        // Создаём Image (чёрный экран)
        GameObject imageObj = new GameObject("FadeImage");
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);

        // Растягиваем на весь экран
        RectTransform rt = imageObj.GetComponent<RectTransform>();
        rt.SetParent(fadeCanvas.transform);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        // 1️⃣ Затемнение (0 → 1)
        yield return Fade(0f, 1f);

        // 2️⃣ Асинхронная загрузка сцены
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, startAlpha);

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, endAlpha);
    }
}