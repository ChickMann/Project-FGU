using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    [Header("Door")]
    public Door door;

    [Header("Levers")]
    public Lever redLever;
    public Lever blueLever;
    public Lever yellowLever;

    private int step = 0;

    private void Awake()
    {
        instance = this;
        Debug.Log("PuzzleManager ID: " + GetInstanceID());
    }

    public void CheckLever(string color)
    {
        Debug.Log("[" + color + "]");
        Debug.Log("Length = " + color.Length);

        if (step == 0 && color == "Red")
        {
            Debug.Log("Đúng Red");

            step = 1;

            Debug.Log("Step = " + step);
            return;
        }

        if (step == 1 && color == "Blue")
        {
            Debug.Log("Đúng Blue");

            step = 2;

            Debug.Log("Step = " + step);
            return;
        }

        if (step == 2 && color == "Yellow")
        {
            Debug.Log("Đúng Yellow");
            door.OpenDoor();
            return;
        }

        Debug.Log("Sai thứ tự");
    }

}