using UnityEngine;

public class PlayerBrain : Brain
{
    public InputNames inputNames;

    private void Start()
    {
        if (inputNames == null) Debug.LogWarning($"Controls in {gameObject.name} could not work.", gameObject);
    }

    public override void GetActions()
    {
        //All available buttons
        Shooting = Mathf.Round(Input.GetAxisRaw(inputNames.Shoot ?? "Shoot")) > 0 || Input.GetButtonDown(inputNames.Shoot ?? "Shoot");
        Aiming = Input.GetButtonDown(inputNames.ChangeColor ?? "Change Color");
        Jumping = Input.GetButtonDown(inputNames.Jump ?? "Jump");
        Running = Input.GetButtonDown(inputNames.Run ?? "Run");
        Crouching = Input.GetButtonDown(inputNames.Crouch ?? "Crouch");
        Interacting = Input.GetButton(inputNames.Interact ?? "Interact");
        Submit = Input.GetButton(inputNames.Submit ?? "Submit");
        Cancel = Input.GetButton(inputNames.Cancel ?? "Cancel");

        //Directional input control.
        Direction = new Vector3(
            Input.GetAxis(inputNames.HorizontalMovement ?? "Horizontal"),
            0f,
            Input.GetAxis(inputNames.ForwardMovement ?? "Vertical")
            );
        Forward = Direction.y > 0f;
        Backwards = Direction.y < 0f;
        Right = Direction.x > 0f;
        Left = Direction.x < 0f;
    }
}