using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float jumpForce = 14;
    [SerializeField] private float friction = 0.6f;
    [SerializeField] private float maxSpeed = 5;

    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Animator animator;
    //[SerializeField] private AudioSource jumpAudio;
    //[SerializeField] private AudioSource attackAudio;

    private float _coyoteTime = 0.2f; // чем выше это значение, тем дольше игрок сможет прыгать после отрыва от земли
    private float _jumpBufferTime = 0.2f;// сколько действует буфер
    private float _coyoteTimeCounter;// счётчик времени кайота
    private float _jumpBufferCounter;// счётчик времени буфера
    private float _jumpFramesTimer;// дополнительная проверка на баг бесконечного прыжка

    private float _xAxis;
    private bool _grounded;
    private float _xEuler =90;

    //public static PlayerMove Instance { get; private set; }

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
     //       Instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private void Update()
    {
        _jumpFramesTimer += Time.deltaTime;

        _xAxis = Input.GetAxisRaw("Horizontal");

        if (_grounded)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f && _jumpFramesTimer > 0.4f)
        {
            _jumpFramesTimer = 0f;
            _jumpBufferCounter = 0f;
            Jump();
        }
        if (Input.GetKeyUp(KeyCode.Space) && playerRigidbody.velocity.x > 0f)
        {
            _coyoteTimeCounter = 0f;
        }
        //повороты
        if(_xAxis>0)
            _xEuler=90f;
        else if(_xAxis<0)
            _xEuler=270f;
        transform.localRotation = Quaternion.Euler(0,_xEuler,0);
        animator.SetBool("Walk", Input.GetAxis("Horizontal") != 0);


       // if (EventSystem.current.IsPointerOverGameObject() == false)
       // {
            if (Input.GetMouseButtonDown(0))
            {
               // attackAudio.Play();
               animator.SetTrigger("Attack");
            }
       // }
    }

    public void Jump()
    {
        //jumpAudio.Play();
        animator.SetTrigger("Jump");
        playerRigidbody.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        float speedMultiplierInAir = 1f;
        float input = Input.GetAxis("Horizontal");

        //чтобы трение действовало только на земле
        if (_grounded)
        {
            playerRigidbody.AddForce(input * moveSpeed, 0, 0, ForceMode.VelocityChange);
        }
        else// управляемый полёт
        {
            speedMultiplierInAir = 0.2f;
            // ограничиваем максимальную скорость в полёте
            if (playerRigidbody.velocity.x > maxSpeed && input > 0)
            {
                speedMultiplierInAir = 0;
            }
            if (playerRigidbody.velocity.x < -maxSpeed && input < 0)
            {
                speedMultiplierInAir = 0;
            }
            playerRigidbody.AddForce(input * moveSpeed * speedMultiplierInAir, 0, 0, ForceMode.VelocityChange);
        }
        // трение по оси Х, чтобы не ускоряться бесконечно
        playerRigidbody.AddForce(-playerRigidbody.velocity.x * friction * speedMultiplierInAir, 0, 0, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision)
    {
        // определяем угол, чтобы не прыгать об стены, смотрим точку контакта с поверхностями, и куда она направлена
        for (int i = 0; i < collision.contactCount; i++)
        {
            float angle = Vector3.Angle(collision.contacts[i].normal, Vector3.up);
            // если угол, на котором мы стоим, меньше 45, значит, м ына замле, а если больше — гора или стена
            if (angle < 45f)
            {
                _grounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _grounded = false;
    }

    //private void OnDestroy()
    //{
    //    if (Instance == this)
    //    {
    //        Instance = null;
    //    }
    //}
}
