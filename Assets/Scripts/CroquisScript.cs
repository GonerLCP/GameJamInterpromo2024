using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CroquisScript : MonoBehaviour
{
    public bool estSurCadre;
    private RectTransform rectTransform;
    public bool estAttrapeCroquis;
    private void Start()
    {
        estAttrapeCroquis = false;
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        print(estAttrapeCroquis);   
        DetectUIElement();
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
                //Debug.Log($"Element trouv� : {result.gameObject.name}");
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
    }


    private void OnTransformParentChanged()
    {
        print("Parent du croquis "+name+" modifi�");
    }
}
