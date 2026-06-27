using UnityEngine;

public class TargetZone : MonoBehaviour

{
    public GameObject leverRed;
    public GameObject leverBlue;
    public GameObject leverYellow;
    public GameObject key;
    public SpriteRenderer platformSprite;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Box"))
        {
            activated = true;

            leverRed.SetActive(true);
            leverBlue.SetActive(true);
            leverYellow.SetActive(true);
            key.SetActive(true);
        }
        platformSprite.color = Color.green;
    }
}