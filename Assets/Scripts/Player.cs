using NUnit.Framework;
using UnityEditor.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    private GameObject croquis;
    private CroquisScript croquisScript;
    public bool estAttrape;
    public Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!estAttrape)
        {
            GetCroquisScript();
        }
        MoveObject();
    }

    void MoveObject()
    {
        if (croquis == null || croquisScript == null) return;
        if (!Input.GetMouseButton(1)) { estAttrape = false; return; }
        croquis.transform.SetAsLastSibling();
        Vector2 mousePosition;
        estAttrape = true;

        // Convertit la position de la souris dans l'espace du RectTransform parent
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out mousePosition
        );

        // Met à jour la position de l'image
        croquis.GetComponent<RectTransform>().anchoredPosition = mousePosition;
    }

    private void GetCroquisScript()
    {
        // Création des données d'événement
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // Stocker les résultats du Raycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Filtrer les résultats pour le calque d'UI (vérifie si layer 5 est correct)
        var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();

        foreach (var result in uiResults)
        {
            if (result.gameObject.CompareTag("Croquis")) // Vérifie si l'objet a le tag "Croquis"
            {
                croquis = result.gameObject;
                croquisScript = result.gameObject.GetComponent<CroquisScript>();
                return;
            }
        }

        // Réinitialise si aucun objet correspondant n'est trouvé
        croquis = null;
        croquisScript = null;
    }
}
