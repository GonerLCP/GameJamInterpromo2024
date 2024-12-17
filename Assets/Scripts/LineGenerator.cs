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
        if (croquisScript == null) { activeLine = null; return; }
        if (Input.GetMouseButtonDown(0) && croquisScript.estSurCadre == true)
        {
            GameObject newLine = Instantiate(linePrefab);
            activeLine = newLine.GetComponent<Line>();
            newLine.transform.parent = croquis.transform;
        }

        if (Input.GetMouseButtonUp(0) || croquisScript.estSurCadre == false)
        {
            activeLine = null;
        }

        if (activeLine != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            activeLine.Updateline(mousePos);
        }
    }

    private void GetCroquisScript()
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();

        foreach (var result in uiResults)
        {
            if (result.gameObject.tag == "Croquis")
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

    public void ClickButton()
    {
        bouttonPresser = true;
    }
}
