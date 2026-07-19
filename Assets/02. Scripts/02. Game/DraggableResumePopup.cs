using UnityEngine;
using UnityEngine.EventSystems;

public sealed class DraggableResumePopup : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Vector2 pointerOffset;

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasRect == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;
            canvasRect = canvas.GetComponent<RectTransform>();
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPointer);
        pointerOffset = localPointer;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasRect == null) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                eventData.pressEventCamera,
                out var canvasPointer)) return;

        var target = canvasPointer - pointerOffset;
        var halfPopup = rectTransform.rect.size * .5f;
        var halfCanvas = canvasRect.rect.size * .5f;
        target.x = Mathf.Clamp(target.x, -halfCanvas.x + halfPopup.x, halfCanvas.x - halfPopup.x);
        target.y = Mathf.Clamp(target.y, -halfCanvas.y + halfPopup.y, halfCanvas.y - halfPopup.y);
        rectTransform.anchoredPosition = target;
    }
}
