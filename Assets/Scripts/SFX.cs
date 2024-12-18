using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioClip addColumn;
    public AudioClip deleteColumn;
    public AudioClip croquisCategorized;
    public AudioClip croquisBackToCadre;
    public AudioClip beehive;
    public AudioClip inputField;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void playAddColumn()
    {
        source.PlayOneShot(addColumn);
    }
    public void playDeleteColumn()
    {
        source.PlayOneShot(deleteColumn);
    }
    public void playCroquisCategorized()
    {
        source.PlayOneShot(croquisCategorized);
    }
    public void playCroquisBackToCadre()
    {
        source.PlayOneShot(croquisBackToCadre);
    }

    public void playBeehive()
    {
        source.PlayOneShot(beehive);
    }
    public void playInputField()
    {
        source.PlayOneShot(inputField);
    }
}
