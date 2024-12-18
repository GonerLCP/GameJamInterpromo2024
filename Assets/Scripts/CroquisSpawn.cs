using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CroquisSpawn : MonoBehaviour
{
    public GameObject listeDesCroquis;
    public GameObject croquis;
    public bool croquisSurLeCadre;
    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        croquisSurLeCadre = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetCroquis()
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, GetComponent<RectTransform>().position);

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        // Stocker les résultats du Raycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Filtrer les résultats pour le calque d'UI (vérifie si layer 5 est correct)
        var uiResults = results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")).ToList();


        croquis = null;
        foreach (var result in uiResults)
        {
            if (result.gameObject.tag == "Croquis") //Si trouvé l'objet Croquis, alors on renvoie le gameObject, potentiellement conflit avec d'autres gameobjects
            {
                croquisSurLeCadre = true;
                print("Croquis sur le cadre");
                return;
            }
        }
        print("En fait non");
        SpawnCroquis();
        croquisSurLeCadre = false;
    }

    void SpawnCroquis()
    {
        if (listeDesCroquis.transform.childCount <= 0) { return; }
        int numberOfChild = listeDesCroquis.transform.childCount;
        croquis = listeDesCroquis.transform.GetChild(Random.Range(0, numberOfChild)).gameObject;
        player.TeleporterSurCadre(croquis);
    }
}
