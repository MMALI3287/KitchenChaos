using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter SelectedCounter;
    }

    [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private GameInput gameInput;

    [SerializeField]
    private LayerMask counterLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of Player");
        }
        Instance = this;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
            selectedCounter.Interact();
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);

        if (movementVector != Vector3.zero)
            lastInteractDir = movementVector;

        float interactDistance = 2f;
        if (
            Physics.Raycast(
                transform.position,
                lastInteractDir,
                out RaycastHit raycastHit,
                interactDistance,
                counterLayerMask
            )
        )
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
                else
                {
                    SetSelectedCounter(null);
                }
            }
        }
        else
        {
            selectedCounter = null;
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        float moveDistance = movementSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            movementVector,
            moveDistance
        );

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(movementVector.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(
                transform.position,
                transform.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance
            );
            if (canMove)
                movementVector = moveDirX;
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, movementVector.z).normalized;
                canMove = !Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance
                );
                if (canMove)
                    movementVector = moveDirZ;
            }
        }
        if (canMove)
            transform.position += movementVector * moveDistance;
        transform.forward = Vector3.Slerp(
            transform.forward,
            movementVector,
            Time.deltaTime * rotationSpeed
        );
        isWalking = movementVector != Vector3.zero;
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(
            this,
            new OnSelectedCounterChangedEventArgs { SelectedCounter = selectedCounter }
        );
    }
}
