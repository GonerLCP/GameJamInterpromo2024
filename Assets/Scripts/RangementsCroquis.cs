using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

public class RangementsCroquis : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image croquisPlaceholder;
    public TMP_InputField inputText;

    private bool isHovered = false;

    private void Start()
    {
        croquisPlaceholder.enabled = false;
        inputText.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        croquisPlaceholder.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        croquisPlaceholder.enabled = false;
    }

    private void Update()
    {
 
    }
}
