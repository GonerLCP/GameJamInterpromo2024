using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;

    public bool dansLeCadre, dansLeCroquis, peutDessiner, peutEffacer;

    Line activeLine;

    private GameObject croquis;

    private GameObject lineDrawn;

    public float clickThreshold = 5f;

    private void Start()
    {
        peutDessiner = false; peutEffacer = false;

        //Sélectionner le crayon par défaut
        ClickButtonDraw();
    }

    // Update is called once per frame
    void Update()
    {
        GetCroquisScript();
        if (peutDessiner){ DessinLigne(); }
        if (peutEffacer) { EffacerLigne(); }
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
        
        
        croquis = null;
        dansLeCadre = false;
        dansLeCroquis = false;
        lineDrawn = null;
        foreach (var result in uiResults)
        {
            if (result.gameObject.name == "Cadre")
            {
                dansLeCadre = true;
            }
            if (result.gameObject.tag == "Croquis") //Si trouvé l'objet Croquis, alors on renvoie le gameObject, potentiellement conflit avec d'autres gameobjects
            {
                dansLeCroquis = true;
                croquis = result.gameObject;
            }
            if (result.gameObject.CompareTag("Line"))
            {
                lineDrawn = result.gameObject;
            }
        }
    }
    private void DessinLigne()
    {
        //Trace la ligne si l'on est bien sur le croquis
        if (Input.GetMouseButtonDown(0) && dansLeCadre && peutDessiner == true)
        {
            GameObject newLine = Instantiate(linePrefab);
            activeLine = newLine.GetComponent<Line>();
            newLine.transform.parent = croquis.transform;
        }

        //Desactive le dessin si on relache le click ou qu'on sors
        if (Input.GetMouseButtonUp(0) || dansLeCroquis == false)
        {
            activeLine = null;
        }

        //Continue le dessin 
        if (activeLine != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            activeLine.Updateline(mousePos);
        }
    }

    //entierement fait sous chatgpt, j'ai give up, je suis une merde, faut un threshold un peu grand genre 5
    private void EffacerLigne()
    {
        if (Input.GetMouseButton(0) && dansLeCadre && peutEffacer)
        {
            Vector2 mousePosition = Input.mousePosition; // Position de la souris en pixels
            LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>(); // Trouve tous les LineRenderers

            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                for (int i = 0; i < lineRenderer.positionCount - 1; i++)
                {
                    // Récupère les points du LineRenderer en espace écran
                    Vector3 screenPoint1 = RectTransformUtility.WorldToScreenPoint(Camera.main, lineRenderer.GetPosition(i));
                    Vector3 screenPoint2 = RectTransformUtility.WorldToScreenPoint(Camera.main, lineRenderer.GetPosition(i + 1));

                    // Vérifie si la souris est proche d'un segment
                    if (DistanceFromPointToSegment(mousePosition, screenPoint1, screenPoint2) <= clickThreshold)
                    {
                        Debug.Log($"Clicked on {lineRenderer.name}");
                        Destroy(lineRenderer.gameObject); // Détruit le GameObject associé au LineRenderer
                        return;
                    }
                }
            }
        }
    }
    float DistanceFromPointToSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
    {
        // Calcul de la projection
        Vector2 line = segmentEnd - segmentStart;
        Vector2 toPoint = point - segmentStart;

        float t = Mathf.Clamp01(Vector2.Dot(toPoint, line) / line.sqrMagnitude);
        Vector2 projection = segmentStart + t * line;

        // Retourne la distance entre la souris et la projection
        return Vector2.Distance(point, projection);
    }
    public void ClickButtonDraw()
    {
        peutDessiner = true;
        peutEffacer = false;
    }

    public void ClickButtonEreaser()
    {
        peutDessiner = false;
        peutEffacer = true;
    }

    public void clickButtonGrab()
    {
        peutDessiner = false;
        peutEffacer = false;
    }
}
