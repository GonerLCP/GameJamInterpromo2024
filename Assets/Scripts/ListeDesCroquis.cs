using UnityEngine;

public class ListeDesCroquis : MonoBehaviour
{
    public CroquisScript[] croquisArray = new CroquisScript[14];
    public SFX audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        croquisArray = GetComponentsInChildren<CroquisScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
