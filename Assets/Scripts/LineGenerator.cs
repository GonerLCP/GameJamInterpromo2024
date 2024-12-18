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

    // Update is called once per frame
    void Update()
    {
        GetCroquisScript();
        DessinLigne();
    }

    private void GetCroquisScript()
    {
        // Cr�ation des donn�es d'�v�nement
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // Stocker les r�sultats du Raycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Filtrer les r�sultats pour le calque d'UI (v�rifie si layer 5 est correct)
        var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();
        
        
        croquis = null;
        dansLeCadre = false;
        dansLeCroquis = false;
        foreach (var result in uiResults)
        {
            if (result.gameObject.name == "Cadre")
            {
                dansLeCadre = true;
            }
            if (result.gameObject.tag == "Croquis") //Si trouv� l'objet Croquis, alors on renvoie le gameObject, potentiellement conflit avec d'autres gameobjects
            {
                dansLeCroquis = true;
                croquis = result.gameObject;
            }
        }
    }
    private void DessinLigne()
    {
        //Trace la ligne si l'on est bien sur le croquis
        if (Input.GetMouseButtonDown(0) && dansLeCadre == true)
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
    public void ClickButton()
    {
        bouttonPresser = true;
    }
}
