using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;

    private float fixedY;

    private void Start()
    {
        fixedY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = Mathf.Lerp(transform.position.x, target.position.x, followSpeed * Time.deltaTime);
        transform.position = new Vector3(targetX, fixedY, transform.position.z);
    }
}
