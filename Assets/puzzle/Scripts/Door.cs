using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isOpen = false;

    BoxCollider2D doorCollider;

    private void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        // đổi màu để biết cửa đã mở
        GetComponent<SpriteRenderer>().color = Color.green;

        // bỏ va chạm
        doorCollider.enabled = false;

        Debug.Log("Door Open!");
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}