using UnityEngine;
using TMPro;

public class CategorieNameDetector : MonoBehaviour
{
    public SFX audioSource;

    private TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public void InputFieldModified()
    {
        if(inputField.text=="Shrek" || inputField.text == "shrek")
        {
            audioSource.playInputField();
        }
    }
}
