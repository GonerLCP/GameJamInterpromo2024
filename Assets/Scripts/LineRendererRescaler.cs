using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererRescaler : MonoBehaviour
{
    private RectTransform parentRect;
    private LineRenderer lineRenderer;
    public float initialWidth = 509.34f;
    public float initialHeight = 509.34f;
    void Start()
    {
        parentRect = GetComponentInParent<RectTransform>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        //if (parentRect != null)
        //{
        //    // Met à jour l'échelle en fonction des dimensions du parent
        //    Vector3 scale = parentRect.localScale;
        //    lineRenderer.widthMultiplier = scale.x; // Ou ajuste en fonction de tes besoins
        //}
        UpdateLineRendererPositions();
    }

    void UpdateLineRendererPositions()
    {
        if (parentRect == null) return;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = lineRenderer.GetPosition(i);
            pos.x *= parentRect.rect.width / initialWidth;
            pos.y *= parentRect.rect.height / initialHeight;
            lineRenderer.SetPosition(i, pos);
        }
    }
}