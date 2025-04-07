using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Vector3 velocity = new Vector3();
    [SerializeField]
    private float speed = 10.0f;
    public void UpdateVelocity(Vector2 velocity)
    {
        this.velocity = (Vector3)(velocity * (speed + Time.deltaTime));
    }

    private void MovePlayer()
    {
        this.transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void Update()
    {
        MovePlayer();
    }
}
