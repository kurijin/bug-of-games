using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 10f; // 車の進む速度

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 車の前方向に力を加える(自動で進む)
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }
}
