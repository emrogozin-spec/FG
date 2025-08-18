using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIClickHack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<RectTransform> uiElementsToMove = new List<RectTransform>();
    [SerializeField] private RectTransform arrowImage;

    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 100f;
    [SerializeField] private float animationDuration = 1.5f;

    [Header("Rotation Settings")]
    [SerializeField] private int arrowRotations = 2;
    [SerializeField] private float initialArrowRotation = -90f;

    private bool isAnimating = false;
    private bool isExtended = false;

    private Vector2 originalAnchoredPosition;
    private List<Vector2> originalAnchoredPositions = new List<Vector2>();
    private Quaternion arrowOriginalRotation;

    private void Start()
    {
        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        foreach (RectTransform rt in uiElementsToMove)
        {
            originalAnchoredPositions.Add(rt != null ? rt.anchoredPosition : Vector2.zero);
        }

        if (arrowImage != null)
        {
            arrowImage.localRotation = Quaternion.Euler(0, 0, initialArrowRotation);
            arrowOriginalRotation = arrowImage.localRotation;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���������, ��� �� ���� �� ����� �������
            if (IsClickOnThisUIElement())
            {
                if (!isAnimating)
                {
                    StartCoroutine(AnimateUIElements());
                }
            }
        }
    }

    private bool IsClickOnThisUIElement()
    {
        // �������� RectTransform �������� �������
        RectTransform rectTransform = GetComponent<RectTransform>();

        // ������������ ������� ���� � ��������� ���������� RectTransform
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);

        // ���������, �������� �� ����� � ������������� �������
        return rectTransform.rect.Contains(localMousePosition);
    }

    private IEnumerator AnimateUIElements()
    {
        isAnimating = true;

        Vector2 startPos = isExtended ? originalAnchoredPosition + Vector2.right * moveDistance : originalAnchoredPosition;
        Vector2 endPos = isExtended ? originalAnchoredPosition : originalAnchoredPosition + Vector2.right * moveDistance;

        float startRotation = isExtended ? initialArrowRotation + 180f : initialArrowRotation;
        float endRotation = isExtended ? initialArrowRotation : initialArrowRotation + 180f;
        float rotationDirection = isExtended ? -1f : 1f;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);

            for (int i = 0; i < uiElementsToMove.Count; i++)
            {
                if (uiElementsToMove[i] != null)
                {
                    Vector2 objStartPos = isExtended ? originalAnchoredPositions[i] + Vector2.right * moveDistance : originalAnchoredPositions[i];
                    Vector2 objEndPos = isExtended ? originalAnchoredPositions[i] : originalAnchoredPositions[i] + Vector2.right * moveDistance;
                    uiElementsToMove[i].anchoredPosition = Vector2.Lerp(objStartPos, objEndPos, easedT);
                }
            }

            if (arrowImage != null)
            {
                float rotationProgress = 360f * arrowRotations * (easedT * rotationDirection);
                float currentRotation = Mathf.Lerp(startRotation, endRotation, easedT) + rotationProgress;
                arrowImage.localRotation = Quaternion.Euler(0, 0, currentRotation);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GetComponent<RectTransform>().anchoredPosition = endPos;
        for (int i = 0; i < uiElementsToMove.Count; i++)
        {
            if (uiElementsToMove[i] != null)
            {
                uiElementsToMove[i].anchoredPosition = isExtended ? originalAnchoredPositions[i] : originalAnchoredPositions[i] + Vector2.right * moveDistance;
            }
        }

        if (arrowImage != null)
        {
            arrowImage.localRotation = Quaternion.Euler(0, 0, endRotation);
        }

        isExtended = !isExtended;
        isAnimating = false;
    }

    public void ResetAll()
    {
        StopAllCoroutines();
        GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;
        for (int i = 0; i < uiElementsToMove.Count; i++)
        {
            if (uiElementsToMove[i] != null)
                uiElementsToMove[i].anchoredPosition = originalAnchoredPositions[i];
        }
        if (arrowImage != null)
        {
            arrowImage.localRotation = arrowOriginalRotation;
        }
        isExtended = false;
        isAnimating = false;
    }
}