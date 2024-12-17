using NUnit.Framework;
using UnityEditor.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;

    public bool dansLeCadre, dansLeCroquis, bouttonPresser;

    Line activeLine;

    private GameObject croquis;
    CroquisScript croquisScript;

    // Update is called once per frame
    void Update()
    {
        GetCroquisScript();
        DessinLigne();
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
            if (result.gameObject.tag == "Croquis") //Si trouvé l'objet Croquis, alors on renvoie le gameObject, potentiellement conflit avec d'autres gameobjects
            {
                croquis = result.gameObject;
                croquisScript = result.gameObject.GetComponent<CroquisScript>();
                break;
            }
            else
            {
                croquis = null;
                croquisScript = null;
            }
        }
    }
    private void DessinLigne()
    {
        //Si le croquis n'a pas été détecté, on ne peux pas dessiner
        if (croquisScript == null) { activeLine = null; return; }

        //Trace la ligne si l'on est bien sur le croquis
        if (Input.GetMouseButtonDown(0) && croquisScript.estSurCadre == true)
        {
            GameObject newLine = Instantiate(linePrefab);
            activeLine = newLine.GetComponent<Line>();
            newLine.transform.parent = croquis.transform;
        }

        //Desactive le dessin si on relache le click ou qu'on sors
        if (Input.GetMouseButtonUp(0) || croquisScript.estSurCadre == false)
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
    public void ClickButton()
    {
        bouttonPresser = true;
    }
}
