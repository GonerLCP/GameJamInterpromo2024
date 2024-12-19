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
    private RectTransform rectTransform;
    public bool estAttrapeCroquis;
    public SFX audioSource;
    private int nFramesHasBeenAlive=0;
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

    private void FixedUpdate()
    {
        nFramesHasBeenAlive++;
    }

    void DetectUIElement()
    {
        // Récupération du RectTransform de cet objet
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Création des données d'événement
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position)
        };

        // Stocker les résultats du Raycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Filtrer les résultats pour le calque d'UI (vérifie si layer 5 est correct)
        var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();

        // Afficher les noms des éléments trouvés
        if (uiResults.Count > 0)
        {
            foreach (var result in uiResults)
            {
                //Chercher dans les results si le cadre y est
                //Debug.Log($"Element trouvé : {result.gameObject.name}");
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
        if (GetComponentInParent<ListeDesCroquis>())
        {
            audioSource.playCroquisBackToCadre();
            print("liste");
        }
        else if (GetComponentInParent<RangementsCroquis>())
        {
            print("rangements");
            if (nFramesHasBeenAlive > 60)
            {
                audioSource.playCroquisCategorized();
            }
        }
    }
}
