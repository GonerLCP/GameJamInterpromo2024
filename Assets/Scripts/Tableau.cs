using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tableau : MonoBehaviour
{
    public int maxNumberOfColumns = 10;
    public Button addColumnButton;

    public List<GameObject> columns = new List<GameObject>();

    void Start()
    {
        columns[0].GetComponentInChildren<Button>().interactable = false;
    }

    // Update is called once per frame
    public void TryToDeleteColumn(GameObject column)
    {
        if (columns.Count > 1)
        {
            columns.Remove(column);
            Destroy(column);
            addColumnButton.interactable = true;

            if (columns.Count == 1)
            {
                columns[0].GetComponentInChildren<Button>().interactable = false;
            }
        }
        else
        {
            Debug.Log("Impossible de supprimer la dernière colonne");
        }
    }

    public void TryToAddColumn()
    {
        if (columns.Count < maxNumberOfColumns)
        {
            columns[0].GetComponentInChildren<Button>().interactable = true;
            GameObject columnInstance = Instantiate(columns[0]);

            columnInstance.transform.SetParent(transform);

            columnInstance.transform.localPosition = Vector3.zero;
            columnInstance.transform.localRotation = Quaternion.identity;
            columnInstance.transform.localScale = Vector3.one;

            columns.Add(columnInstance);

            if (columns.Count >= maxNumberOfColumns)
            {
                addColumnButton.interactable = false;
            }
        }
        else
        {
            Debug.Log("Nombre max de colonnes atteint");
        }
    }
}
