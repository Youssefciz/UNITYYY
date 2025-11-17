using UnityEngine;

public class WallSectionController : MonoBehaviour
{
    private const int COUNT_TO_OPEN = 2;

    public void UpdateWallState(int currentCount)
    {
        if (currentCount >= COUNT_TO_OPEN)
        {
            gameObject.SetActive(false);
        }
    }
}
