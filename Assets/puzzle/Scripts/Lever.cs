using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Thông tin Lever")]
    public string leverColor;

    [Header("UI")]
    public GameObject hintUI;

    private bool playerNear = false;
    private bool activated = false;

    private PlayerKey playerKey;
    private SpriteRenderer sr;
    private Color originalColor;
    private void Start()


    {
        sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;

        if (hintUI != null)
            hintUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerNear)
            return;

        if (activated)
            return;

        if (playerKey == null)
            return;

        if (!playerKey.hasKey)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            activated = true;

            PuzzleManager.instance.CheckLever(leverColor);

            transform.rotation = Quaternion.Euler(0, 0, -45);

            sr.color = Color.green;

            if (hintUI != null)
                hintUI.SetActive(false);

            Debug.Log("Đã gạt cần " + leverColor);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNear = true;

        playerKey = other.GetComponent<PlayerKey>();

        if (hintUI != null)
            hintUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNear = false;

        if (hintUI != null)
            hintUI.SetActive(false);
    }

    // PuzzleManager sẽ gọi hàm này nếu người chơi gạt sai
    public void ResetLever()
    {
        Debug.Log(gameObject.name + " -> ResetLever()");

        activated = false;

        transform.rotation = Quaternion.identity;

        sr.color = originalColor;
    }
}