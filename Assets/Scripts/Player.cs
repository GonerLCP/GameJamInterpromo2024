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
    public bool estAttrape, estAttrapeTampon;
    private bool surLeCadreDrop;
    public Canvas canvas;
    private Vector2 offset;
    public Vector3 coordTP;

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
        DropObject();
    }

    void DropObject()
    {
        if (estAttrapeTampon == true && estAttrape == false)
        {
            //////FAUT PAS REGARDER CA CEST CACA JE SUIS UN BRANQUIGNOL 
            // Création des données d'événement
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            // Stocker les résultats du Raycast
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Filtrer les résultats pour le calque d'UI (vérifie si layer 5 est correct)
            var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();
            surLeCadreDrop = false;
            foreach (var result in uiResults)
            {
                if (result.gameObject.name == "Cadre") //SI on est sur un cadre on dit qu'il faudra se tp dessus
                {
                    surLeCadreDrop = true;
                }
                if (result.gameObject.CompareTag("TableColumn")) //Si on trouve une table column on lui met en enfant
                {
                    croquis.transform.parent = result.gameObject.transform.Find("Rangements").transform;
                    estAttrapeTampon = estAttrape;
                    return;
                }
            }
            if (surLeCadreDrop == true) //On sait que c'est sur le cadre on fait rien
            {
                estAttrapeTampon = estAttrape;
                return;
            }
            if (surLeCadreDrop == false)//On sait que c'est nul part on tp sur tpcoord, arbitrairement mis à une valeur dans l'éditeur
            {
                croquis.GetComponent<RectTransform>().anchoredPosition = coordTP;
            }
        }
        estAttrapeTampon = estAttrape;
    }
    void MoveObject()
    {
        if (croquis == null) return;
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
        croquis.GetComponent<RectTransform>().anchoredPosition = mousePosition - offset;
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

                // Calcule l'offset initial au moment du clic
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    Input.mousePosition,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                    out Vector2 localMousePosition
                );

                RectTransform croquisRect = croquis.GetComponent<RectTransform>();
                offset = localMousePosition - croquisRect.anchoredPosition;
                return;
            }
        }

        // Réinitialise si aucun objet correspondant n'est trouvé
        croquis = null;
    }
}
