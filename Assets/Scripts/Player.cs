using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private GameInput gameInput;

    private bool isWalking;

    private void Update()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        transform.position += movementVector * Time.deltaTime * movementSpeed;
        transform.forward = Vector3.Slerp(
            transform.forward,
            movementVector,
            Time.deltaTime * rotationSpeed
        );
        isWalking = movementVector != Vector3.zero;
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
