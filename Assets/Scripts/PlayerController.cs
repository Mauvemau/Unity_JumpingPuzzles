using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Vector3 velocity = new Vector3();
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private Rigidbody rb;
    public void UpdateVelocity(Vector2 velocity)
    {
        this.velocity.x = velocity.x * (speed + Time.deltaTime);
        this.velocity.z = velocity.y * (speed + Time.deltaTime);
    }

    private void MovePlayer()
    {
        this.transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void MyFixedUpdate()
    {
        //rb.AddForce(new Vector3(velocity.x, 0, velocity.y) * speed * Time.fixedDeltaTime, ForceMode.Impulse);
        //rb.AddForce(new Vector3(velocity.x, 0, velocity.y) * speed, ForceMode.Force);
    }

    private void Update()
    {
        MovePlayer();
    }
}
