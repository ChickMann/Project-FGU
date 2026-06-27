using UnityEngine;

public class DoorHint : MonoBehaviour
{
    public GameObject hintPanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerKey key = other.GetComponent<PlayerKey>();

        if (key != null && key.hasKey)
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerKey>() != null)
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
        }
    }
}