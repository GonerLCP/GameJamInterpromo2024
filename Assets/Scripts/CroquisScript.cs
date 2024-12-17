using NUnit.Framework;
using UnityEditor.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CroquisScript : MonoBehaviour
{
    public bool estSurCadre;
    public bool estDansCroquis;
    private RectTransform rectTransform;
    private Canvas canvas;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    void Update()
    {
        DetectUIElement();
        MoveObject();
    }

    void DetectUIElement()
    {
        // R�cup�ration du RectTransform de cet objet
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Cr�ation des donn�es d'�v�nement
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position)
        };

        // Stocker les r�sultats du Raycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Filtrer les r�sultats pour le calque d'UI (v�rifie si layer 5 est correct)
        var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();

        // Afficher les noms des �l�ments trouv�s
        if (uiResults.Count > 0)
        {
            foreach (var result in uiResults)
            {
                //Chercher dans les results si le cadre y est
                Debug.Log($"Element trouv� : {result.gameObject.name}");
                if (result.gameObject.name == "Cadre")
                {
                    estSurCadre = true;
                    break;
                }
                else
                {
                    estSurCadre = false;
                }
            }
        }
        else
        {
            Debug.Log("Aucun �l�ment UI d�tect� au-dessus.");
        }
    }

    void MoveObject()
    {
        if (!estDansCroquis) { return; }
        if(!Input.GetMouseButton(1)) { return; }
        Vector2 mousePosition;

        // Convertit la position de la souris dans l'espace du RectTransform parent
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out mousePosition
        );

        // Met � jour la position de l'image
        rectTransform.localPosition = mousePosition;
    }

    public void InCroquis()
    {
        estDansCroquis = true;
    }

    public void OutCroquis()
    {
        estDansCroquis = false;
    }
}
