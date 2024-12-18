using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RangementsCroquis : MonoBehaviour
{
    public Image croquisPlaceholder;
    public TMP_InputField inputText;
    public CroquisScript[] croquisArray = new CroquisScript[14];

    private RectTransform thisRectTransform;
    private bool aucunCroquisneSurvoleLaColonne = true;

    private void Start()
    {
        croquisPlaceholder.enabled = false;
        inputText.text = "";

        foreach (CroquisScript croquis in GetComponentsInChildren<CroquisScript>())
        {
            Destroy(croquis.gameObject);
        }

        thisRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        aucunCroquisneSurvoleLaColonne = true;

        // Vérifier si un objet de croquisArray survole l'élément
        foreach (CroquisScript croquis in croquisArray)
        {
            if (croquis !=null && IsHoveringOver(croquis.GetComponent<RectTransform>()))
            {
                aucunCroquisneSurvoleLaColonne = false;
            }
        }

        if (aucunCroquisneSurvoleLaColonne==false && IsMouseHovering())
        {
            croquisPlaceholder.enabled = true;
        }
        else
        {
            croquisPlaceholder.enabled = false;
        }
    }

    private bool IsHoveringOver(RectTransform croquisRectTransform)
    {
        // Convertir les coins du RectTransform en coordonnées d'écran
        Rect thisRect = GetScreenRect(thisRectTransform);
        Rect croquisRect = GetScreenRect(croquisRectTransform);

        // Vérifier s'il y a intersection entre les deux rectangles
        return thisRect.Overlaps(croquisRect);
    }

    private bool IsMouseHovering()
    {
        // Vérifier si la souris est dans le rectangle de l'objet
        Vector2 mousePosition = Input.mousePosition;
        return RectTransformUtility.RectangleContainsScreenPoint(thisRectTransform, mousePosition, Camera.main);
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        // Obtenir les coins du RectTransform dans l'espace écran
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // Calculer les dimensions du rectangle
        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}
