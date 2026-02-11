using UnityEngine;

public class PropellerController : MonoBehaviour
{
    [Header("Movement")]
    public AirplaneController airplane;
    public float speedMultiplier = 200f;

    void Update()
    {
        if (airplane == null) return;

        float rotateSpeed = airplane.FlySpeed * speedMultiplier;
        transform.Rotate(rotateSpeed * Time.deltaTime, 0f, 0f);
    }
}
