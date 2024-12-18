using System.Collections;
using UnityEngine;

public class EndGameDetector : MonoBehaviour
{
    public CroquisScript[] croquisArray = new CroquisScript[14];
    public float croquisLerpDuration = 2f;
    public GameObject rucheFinale;

    private bool tousLesCroquisSontSurLeCalque = true;
    private bool endHasBeenCalled = false;
    private RectTransform cadreRectTransform;

    private void Start()
    {
        cadreRectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if (endHasBeenCalled == false)
        {
            tousLesCroquisSontSurLeCalque = true;

            foreach (CroquisScript croquis in croquisArray)
            {
                if (!croquis.estSurCadre)
                {
                    tousLesCroquisSontSurLeCalque = false;
                }
            }

            if (tousLesCroquisSontSurLeCalque)
            {
                endHasBeenCalled = true;

                foreach (CroquisScript croquis in croquisArray)
                {
                    // Détruire les enfants de type Line
                    foreach (Line child in croquis.GetComponentsInChildren<Line>())
                    {
                        Destroy(child.gameObject);
                    }
                    // Lancer la coroutine pour déplacer chaque croquis
                    StartCoroutine(MoveToCenter(croquis.GetComponent<RectTransform>()));
                }
                StartCoroutine(DelayBeforeRucheSpawn(croquisLerpDuration + 1));
            }
        }
    }

    IEnumerator MoveToCenter(RectTransform elementRect)
    {
        // Position de départ dans l'espace local
        Vector3 startPosition = elementRect.anchoredPosition;

        // Convertir la position du cadre dans l'espace local du parent de elementRect
        Vector3 targetPosition = GetLocalPositionInParentSpace(elementRect, cadreRectTransform);

        float elapsedTime = 0f;

        // Animation fluide entre startPosition et targetPosition
        while (elapsedTime < croquisLerpDuration)
        {
            elementRect.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / croquisLerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null; // Attendre la prochaine frame
        }

        // S'assurer que la position finale est exacte
        elementRect.anchoredPosition = targetPosition;
    }
    private Vector3 GetLocalPositionInParentSpace(RectTransform sourceRect, RectTransform targetRect)
    {
        // Convertir la position du cadre (targetRect) en coordonnées mondiales
        Vector3 worldPosition = targetRect.position;

        // Convertir la position mondiale en coordonnées locales par rapport au parent de sourceRect
        return sourceRect.parent.InverseTransformPoint(worldPosition);
    }

    private IEnumerator DelayBeforeRucheSpawn(float delay)
    {
        // Attendre le délai spécifié
        yield return new WaitForSeconds(delay);

        // Appeler la fonction
        EndGame();
    }

    private void EndGame()
    {
        GameObject rucheFinaleInstance = Instantiate(rucheFinale);
        rucheFinaleInstance.transform.position = croquisArray[0].transform.position;


        foreach (CroquisScript croquis in croquisArray)
        {
            Destroy(croquis.gameObject);
        }
    }
}
