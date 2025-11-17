using UnityEngine;

public class DoorController : MonoBehaviour
{
    private const int COUNT_TO_OPEN = 2;

    public void UpdateDoorState(int currentCount)
    {
        if (currentCount >= COUNT_TO_OPEN)
        {
            gameObject.SetActive(false);
        }
    }
}
