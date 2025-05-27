using UnityEngine;

public class ObjectHoveringEffect : MonoBehaviour {
    [Tooltip("Amplitude of the hover (how high it goes up and down)")]
    public float amplitude = 0.5f;
    [Tooltip("Speed of the hovering motion")]
    public float frequency = 1f;
    
    private Vector3 _originalPosition;
    
    private void Awake()
    {
        _originalPosition = transform.position;
    }

    private void Update()
    {
        var yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = _originalPosition + new Vector3(0, yOffset, 0);
    }
 
}
