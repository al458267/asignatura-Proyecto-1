using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0.0f, 1.0f, -10.0f);
    [SerializeField] public float smoothness = 0.3f;

    Vector3 velocity;

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothness);
    }
}
