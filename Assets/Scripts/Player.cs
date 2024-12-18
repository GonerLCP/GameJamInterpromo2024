using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private GameObject croquis;
    public GameObject listeCroquis;
    public GameObject target;

    public Canvas canvas;

    public CroquisSpawn croquisSpawn;

    public Texture2D grabCursor;
    public Texture2D penCursor;
    public Texture2D eraserCursor;

    [SerializeField]
    private Button[] buttons;

    private Vector2 offset;

    public bool estAttrape, estAttrapeTampon;
    private bool surLeCadreDrop;
    public bool peutGrab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        peutGrab = false;

        //Je supprime lui pour que le crayon soit non interactable par défaut
        //SetAllButtonsInteractable();

        //Sélectionner le crayon par défaut
        ClickButtonDraw();
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
                    if (croquis.transform.childCount >= 0)
                    {
                        for (int i = 0; i < croquis.transform.childCount; i++)
                        {
                            croquis.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                    estAttrapeTampon = estAttrape;
                    croquisSpawn.GetCroquis();
                    return;
                }
            }
            if (surLeCadreDrop == true) //On sait que c'est sur le cadre on fait rien
            {
                estAttrapeTampon = estAttrape;
                if (croquis.transform.parent.name == "Rangements")
                {
                    //TeleporterSurCadre(croquis);
                    //Modifier le croquis comme avant car il à été resize par le tableau
                    croquis.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                    croquis.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                    croquis.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                    croquis.GetComponent<RectTransform>().sizeDelta = new Vector2(509.34f, 509.34f);

                    if (croquis.transform.childCount >= 0)
                    {
                        for (int i = 0; i < croquis.transform.childCount; i++)
                        {
                            croquis.transform.GetChild(i).gameObject.SetActive(true);
                        }
                    }

                    //On remet le croquis enfant de la liste
                    croquis.transform.parent = listeCroquis.transform;
                    croquis.transform.SetAsLastSibling();
                    Vector2 mousePosition;
                    croquis.GetComponent<CroquisScript>().estAttrapeCroquis = true;

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
                return;
            }
            if (surLeCadreDrop == false)//On sait que c'est nul part on tp sur tpcoord, arbitrairement mis à une valeur dans l'éditeur
            {
                if (croquis == null || target == null) return;

                TeleporterSurCadre(croquis);
            }
        }
        estAttrapeTampon = estAttrape;
    }
    void MoveObject()
    {
        if (croquis == null) return;
        if (!Input.GetMouseButton(0) || !peutGrab) { estAttrape = false; croquis.GetComponent<CroquisScript>().estAttrapeCroquis = false; return; }
        croquis.transform.SetAsLastSibling();
        Vector2 mousePosition;
        estAttrape = true;
        croquis.GetComponent<CroquisScript>().estAttrapeCroquis = true;

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
        Cursor.SetCursor(grabCursor, new Vector2(0, grabCursor.height), CursorMode.ForceSoftware);
    }

    public void ClickButtonDraw()
    {
        peutGrab = false;
        Cursor.SetCursor(penCursor, new Vector2(0, penCursor.height), CursorMode.ForceSoftware);
    }

    public void ClickButtonEreaser()
    {
        peutGrab = false;
        Cursor.SetCursor(eraserCursor, new Vector2(0, eraserCursor.height), CursorMode.ForceSoftware);
    }

    public void TeleporterSurCadre(GameObject croquis)
    {
        Vector2 moveTo;

        // Étape 1 : Position cible en espace écran
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);

        // Étape 2 : Canevas racine
        RectTransform canvasRectTransform = canvas.transform as RectTransform;

        // Étape 3 : Convertir en espace local du canevas racine
        bool validPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out moveTo
        );

        if (!validPoint)
        {
            Debug.LogError("La conversion de la position a échoué !");
            return;
        }

        Debug.Log($"Local Point in Canvas: {moveTo}");

        // Étape 4 : Ajuster l'espace local pour l'objet croquis
        RectTransform croquisParent = croquis.transform.parent as RectTransform;

        if (croquisParent != null)
        {
            // Convertir du canevas racine à l'espace local du parent de croquis
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

            //Modifier le croquis comme avant car il à été resize par le tableau
            croquis.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            croquis.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            croquis.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            croquis.GetComponent<RectTransform>().sizeDelta = new Vector2(509.34f, 509.34f);

            //On remet le croquis enfant de la liste
            croquis.transform.parent = listeCroquis.transform;
            Debug.Log($"Position dans le parent de croquis : {localPointInCroquisParent}"); ;
        }
        else
        {
            Debug.LogError("Croquis n'a pas de parent RectTransform !");
        }
    }
}
