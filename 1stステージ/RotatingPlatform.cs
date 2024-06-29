using UnityEngine;

//ギミックを回すためのスクリプト
[RequireComponent(typeof(Rigidbody))]
public class RotatingPlatform : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; //回転軸（デフォルトはY軸）
    public float rotationSpeed = 10f; //回転速度

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
    }

    private void FixedUpdate()
    {
        // 回転
        Quaternion deltaRotation = Quaternion.Euler(rotationAxis * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
