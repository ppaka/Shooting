using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount;
    private float shakeTime;
    private Vector3 initialPosition;

    public void VibrateForTime(float time)
    {
        shakeTime = time;
    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (shakeTime > 0)
        {
            var shake = Random.insideUnitCircle * shakeAmount;
            transform.position = new Vector3(shake.x, shake.y, 0) + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeTime = 0;
            transform.position = initialPosition;
        }
    }
}
