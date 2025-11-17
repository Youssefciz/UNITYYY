using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
