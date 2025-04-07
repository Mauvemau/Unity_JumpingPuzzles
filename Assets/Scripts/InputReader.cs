using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [SerializeField]
    private InputActionReference moveAction;

    private void HandleMoveInput(InputAction.CallbackContext ctx)
    {
        string state = "";
        if (ctx.performed)
        {
            state = "performed";
        } 
        else if (ctx.canceled)
        {
            state = "cancelled";
        } 
        else
        {
            state = "started";
        }
        Debug.Log("Move Input " + state + ":" + ctx.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        moveAction.action.started += HandleMoveInput; // Cuando deja de ser 0
        moveAction.action.performed += HandleMoveInput; // Solo corre cuando cambian los valores
        moveAction.action.canceled += HandleMoveInput; // Cuando vuelve a 0

    }
}
