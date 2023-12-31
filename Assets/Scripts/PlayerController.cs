using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Variables p�blicas para ajustar par�metros
    public float horizontalMove;
    public float verticalMove;
    private Vector3 playerInput;
    public float gravity = 22f;        // Gravedad aplicada al personaje
    public float fallVelocity;         // Velocidad de ca�da
    public float jumpForce = 8;        // Fuerza de salto

    public CharacterController player; // Componente CharacterController que controla al personaje

    public float playerSpeed;          // Velocidad de movimiento del jugador
    private Vector3 movePlayer;       // Vector de movimiento

    public Camera mainCamera;          // C�mara principal
    private Vector3 camForward;       // Direcci�n hacia adelante de la c�mara
    private Vector3 camRight;         // Direcci�n hacia la derecha de la c�mara

    public bool isOnSlope = false;    // Indica si el jugador est� en una pendiente
    private Vector3 hitNormal;        // Normal de la superficie golpeada
    public float slideVelocity;       // Velocidad de deslizamiento en pendientes
    public float slopForceDown;       // Fuerza hacia abajo al deslizar en pendientes

    //Variables de animacion
    public Animator playerAnimatorcontroller;

    // M�todo llamado al inicio del juego
    void Start()
    {
        // Inicializa el componente CharacterController
        player = GetComponent<CharacterController>();
        playerAnimatorcontroller = GetComponent<Animator>();
    }

    // M�todo llamado en cada fotograma del juego
    void Update()
    {
        // Obtener las entradas del jugador para el movimiento
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        // Crear un vector de entrada del jugador y limitar su magnitud a 1
        playerInput = new Vector3(horizontalMove, 0, verticalMove);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        playerAnimatorcontroller.SetFloat("PlayerWalkVelocity", playerInput.magnitude * playerSpeed);

        // Calcular la direcci�n de la c�mara en relaci�n al personaje
        camDirection();

        // Calcular el vector de movimiento del jugador
        movePlayer = playerInput.x * camRight + playerInput.z * camForward;
        movePlayer = movePlayer * playerSpeed;

        // Hacer que el personaje mire en la direcci�n del movimiento
        player.transform.LookAt(player.transform.position + movePlayer);

        // Aplicar la gravedad
        SetGravity();

        // Gestionar las habilidades del jugador, como el salto
        PlayerSkills();

        // Mover al jugador en base al vector de movimiento
        player.Move(movePlayer * Time.deltaTime);

        // Imprimir la magnitud de la velocidad del jugador en la consola
        Debug.Log(player.velocity.magnitude);
    }

    // Calcular la direcci�n de la c�mara
    void camDirection()
    {
        camForward = mainCamera.transform.forward;
        camRight = mainCamera.transform.right;

        // Anular los componentes de altura (y) y normalizar los vectores
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }

    // Funci�n para las habilidades del jugador
    public void PlayerSkills()
    {
        // Verificar si el jugador est� en el suelo y presiona el bot�n de salto
        if (player.isGrounded && Input.GetButtonDown("Jump"))
        {
            fallVelocity = jumpForce;
            movePlayer.y = fallVelocity;
            playerAnimatorcontroller.SetTrigger("PlayerJump");
        }
    }

    // Funci�n para gestionar la gravedad
    void SetGravity()
    {
        if (player.isGrounded)
        {
            // El jugador est� en el suelo, restablecer la velocidad de ca�da
            fallVelocity = -gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
            playerAnimatorcontroller.SetFloat("PlayerVerticalVelocity", player.velocity.y);
        }
        else
        {
            // El jugador no est� en el suelo, aplicar la gravedad
            fallVelocity -= gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
        }
        playerAnimatorcontroller.SetBool("IsGrounded", player.isGrounded);
        SlideDown();
    }


    // Funci�n para deslizarse en pendientes
    public void SlideDown()
    {
        isOnSlope = Vector3.Angle(Vector3.up, hitNormal) >= player.slopeLimit;
        if (isOnSlope)
        {
            movePlayer.x += ((1f - hitNormal.y) * hitNormal.x) * slideVelocity;
            movePlayer.z += ((1f - hitNormal.y) * hitNormal.z) * slideVelocity;

            movePlayer.y += slopForceDown;
        }
    }


    // M�todo llamado cuando el CharacterController choca con un collider
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    private void OnAnimatorMove()
    {
        
    }
}
