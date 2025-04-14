using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [SerializeField]
    private PlayerController controllerReference;
    [SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference jumpAction;

    private void HandleJumpInput(InputAction.CallbackContext ctx) {
        if(controllerReference) {
            controllerReference.SetJumping(ctx.ReadValue<float>());
        }
    } 

    private void HandleMoveInput(InputAction.CallbackContext ctx) {
        if (controllerReference) {
            controllerReference.UpdateVelocity(ctx.ReadValue<Vector2>());
        }
    }

    private void Update() {
        if(moveAction) {
            moveAction.action.started += HandleMoveInput; // Cuando deja de ser 0
            moveAction.action.performed += HandleMoveInput; // Solo corre cuando cambian los valores
            moveAction.action.canceled += HandleMoveInput; // Cuando vuelve a 0
        }
        if(jumpAction) {
            jumpAction.action.started += HandleJumpInput;
            jumpAction.action.canceled += HandleJumpInput;
        }
    }
}
