using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Health")]
    const float maxHealth = 150f;
    public float currentHealth;

    [Header("Ref & Physics")]
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    Vector3 moveDirection;
    Transform cameraGameObject;
    public Slider healthBarSlider;
    public GameObject playerUI;
    public bool isSprinting;
    public bool isMoving;
    public float movementSpeed = 2f;
    public float rotationSpeed = 13f;
    public float sprintingSpeed = 5f;
    Rigidbody playerRigidBody;
    PlayerController playerController;
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;
    public bool isGrounded;
    public bool isJumping;
    public float jumpingHeight = 4f;
    public float gravityIntensity = -15f;
    PhotonView view;
    public int playerTeam;

    private void Awake()
    {
        currentHealth = maxHealth;
        view = GetComponent<PhotonView>();
        if (view != null && view.InstantiationData != null && view.InstantiationData.Length > 0 && view.InstantiationData[0] != null)
        {
            playerController = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("InstantiationData is null or incomplete. Ensure proper instantiation.");
        }

        // Find and assign required components
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();

        // Ensure playerManager is assigned properly
        playerManager = GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager is not assigned or attached to the GameObject.");
        }

        playerRigidBody = GetComponent<Rigidbody>();
        cameraGameObject = Camera.main.transform;
        healthBarSlider.minValue = 0f;
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;
    }

    void Start()
    {
        if(!view.IsMine)
        {
            Destroy(playerUI);
            Destroy(playerRigidBody);
        }
        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        // Ensure playerManager is assigned and not interacting
        if (playerManager != null && playerManager.isInteracting)
            return;

        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        if (isJumping)
            return;

        // Move based on camera direction and input
        moveDirection = new Vector3(cameraGameObject.forward.x, 0f, cameraGameObject.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + cameraGameObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // Sprinting logic
        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else
        {
            if (inputManager.movementAmount >= 0.5f)
            {
                moveDirection *= movementSpeed;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidBody.velocity = new Vector3(movementVelocity.x, playerRigidBody.velocity.y, movementVelocity.z);
    }

    void HandleRotation()
    {
        if (isJumping)
            return;

        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraGameObject.forward * inputManager.verticalInput + cameraGameObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position + Vector3.up * rayCastHeightOffset;
        Vector3 targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (playerManager != null && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Falling", true);
            }

            inAirTimer += Time.deltaTime;
            playerRigidBody.AddForce(transform.forward * leapingVelocity);
            playerRigidBody.AddForce(Vector3.down * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, rayCastHeightOffset + 0.1f, groundLayer))
        {
            if (!isGrounded && playerManager != null && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Landing", true);
            }

            targetPosition.y = hit.point.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !isJumping)
        {
            if (playerManager != null && (playerManager.isInteracting || inputManager.movementAmount > 0))
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnim("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpingHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidBody.velocity = playerVelocity;

            isJumping = true;
            isGrounded = false;
        }
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }

    public void ApplyDamage(float damageValue)
    {
        view.RPC("RPC_TakeDamage", RpcTarget.All, damageValue);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!view.IsMine)
            return;

        currentHealth -= damage;
        healthBarSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log("Damage Taken: " + damage);
        Debug.Log("Current Health: " + currentHealth);
    }

    private void Die()
    {
        playerController.Die();

        ScoreBoard.Instance.PlayerDied(playerTeam);
     
    }
}
