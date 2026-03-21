using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    private float fixedX;
    private float fixedY;

    private void Start()
    {
        fixedX = transform.position.x;
        fixedY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float newX = followX
            ? Mathf.Lerp(transform.position.x, target.position.x, followSpeed * Time.deltaTime)
            : fixedX;

        float newY = followY
            ? Mathf.Lerp(transform.position.y, target.position.y, followSpeed * Time.deltaTime)
            : fixedY;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
