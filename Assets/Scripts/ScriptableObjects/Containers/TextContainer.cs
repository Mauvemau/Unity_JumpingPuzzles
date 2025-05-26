using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Scriptable Object that contains a string
/// </summary>
[CreateAssetMenu(menuName = "Containers/Text Container")]
public class TextContainer : ScriptableObject {
    [SerializeField] 
    [TextArea] 
    private string text;

    public string Text => text;
}
