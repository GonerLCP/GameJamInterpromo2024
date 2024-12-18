using NUnit.Framework;
using UnityEditor.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    private GameObject croquis;
    public bool estAttrape, estAttrapeTampon;
    private bool surLeCadreDrop;
    public bool peutGrab;
    public GameObject target;
    public Canvas canvas;
    private Vector2 offset;
    public Vector3 coordTP;
    public Button btn;
    [SerializeField]
    private Button[] buttons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        peutGrab = false;
        SetAllButtonsInteractable();
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
            // Cr�ation des donn�es d'�v�nement
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            // Stocker les r�sultats du Raycast
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Filtrer les r�sultats pour le calque d'UI (v�rifie si layer 5 est correct)
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
            if (surLeCadreDrop == false)//On sait que c'est nul part on tp sur tpcoord, arbitrairement mis � une valeur dans l'�diteur
            {
                if (surLeCadreDrop == false) // Si l'objet n'est sur aucun cadre, on le t�l�porte
                {
                    if (croquis == null || target == null) return;

                    Vector2 moveTo;

                    // �tape 1 : Position cible en espace �cran
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);

                    // �tape 2 : Canevas racine
                    RectTransform canvasRectTransform = canvas.transform as RectTransform;

                    // �tape 3 : Convertir en espace local du canevas racine
                    bool validPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRectTransform,
                        screenPoint,
                        canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                        out moveTo
                    );

                    if (!validPoint)
                    {
                        Debug.LogError("La conversion de la position a �chou� !");
                        return;
                    }

                    Debug.Log($"Local Point in Canvas: {moveTo}");

                    // �tape 4 : Ajuster l'espace local pour l'objet croquis
                    RectTransform croquisParent = croquis.transform.parent as RectTransform;

                    if (croquisParent != null)
                    {
                        // Convertir du canevas racine � l'espace local du parent de croquis
                        Vector3 worldPosition = canvasRectTransform.TransformPoint(moveTo);
                        Vector2 localPointInCroquisParent;

                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            croquisParent,
                            Camera.main.WorldToScreenPoint(worldPosition),
                            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                            out localPointInCroquisParent
                        );

                        // Appliquer la position dans l'espace local du parent de croquis
                        croquis.GetComponent<RectTransform>().anchoredPosition = localPointInCroquisParent;

                        Debug.Log($"Position dans le parent de croquis : {localPointInCroquisParent}");
                    }
                    else
                    {
                        Debug.LogError("Croquis n'a pas de parent RectTransform !");
                    }
                }
            }
        }
        estAttrapeTampon = estAttrape;
    }
    void MoveObject()
    {
        if (croquis == null) return;
        if (!Input.GetMouseButton(0) || !peutGrab) { estAttrape = false; return; }
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

        // Met � jour la position de l'image
        croquis.GetComponent<RectTransform>().anchoredPosition = mousePosition - offset;
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

        foreach (var result in uiResults)
        {
            if (result.gameObject.CompareTag("Croquis")) // V�rifie si l'objet a le tag "Croquis"
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

        // R�initialise si aucun objet correspondant n'est trouv�
        croquis = null;
    }

    public void OnButtonClicked(Button clickedButton)
    {
        int buttonIndex = System.Array.IndexOf(buttons, clickedButton);

        if (buttonIndex == -1)
            return;

        SetAllButtonsInteractable();

        clickedButton.interactable = false;
    }

    public void SetAllButtonsInteractable()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void ClickButtonGrab()
    {
        peutGrab = true;
    }

    public void ClickButtonDrawAndEreaser()
    {
        peutGrab = false;
    }
}
