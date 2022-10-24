using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 100;
    public float maxMoveSpeed = 40;
    public float airMoveMultiplier = 0.45f;
    public float slideDrag = 10;
    public float slideDownForce = 60;
    public float groundDrag = 5;
    public float jumpForce = 20;

    //minimum velocity required to activate drag
    float stopSpeed = 0.01f;

    [HideInInspector]
    public bool isGrounded, sliding, jumping;
    public Transform feet, orientation;
    public LayerMask whatIsGround;

    [Header("WallRun")]
    public float wallCheckDist = 1;
    public float wallGravity = 5;
    public LayerMask wallRunnable;
    RaycastHit wallHit;
    bool wallRunning;
    //speed to clamp velocity while wall running
    float clampSpeed;

    [Header("Camera Look")]
    public Transform cam;
    public Camera gameCam;
    public float sensitivityX = 15;
    public float sensitivityY = 15;
    public float xClamp = 90;
    public float FovBySpeed = 0.1f;
    public float maxFOV = 100;

    float normalFOV, FOV;
    float tilt, camTilt;

    Rigidbody rb;

    Vector2 movement;
    Vector2 mouse;
    float mouseX, mouseY, xRotation, yRotation;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        normalFOV = gameCam.fieldOfView;
        FOV = normalFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        //input
        if (Input.GetButtonDown("Jump")) Jump();

        if (Input.GetButtonDown("Fire3")) Slide();
        if (Input.GetButtonUp("Fire3")) stopSlide();

        //fall - reset
        if (transform.position.y < -50)
        {
            transform.position = new Vector3(0, 0, 0);
            rb.velocity = Vector3.zero;
        }
    }
    void FixedUpdate()
    {
        move();
        wallRun();
        rotateHead();
    }
    void rotateHead()
    {
        //get mouse input
        mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
        mouseX = mouse.x * sensitivityX;
        mouseY = mouse.y * sensitivityY;

        //add to rotation values
        yRotation += mouseX * Time.deltaTime * (1 + (1 - Time.timeScale));
        xRotation -= mouseY * Time.deltaTime * (1 + (1 - Time.timeScale));
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);

        //set rotation
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        //camera effects
        if(sliding && isGrounded)
        {
            FOV = 100;
            //tilt = 5;
        }
        else if(!wallRunning)
        {
            FOV = normalFOV;
            tilt = 0;
        }
        
        //add camera effects
        camTilt = Mathf.Lerp(camTilt, tilt, 5 * Time.deltaTime);
        cam.GetComponent<CameraScript>().setRotation(new Vector3(xRotation, yRotation, camTilt));

        //wider feild of view when moving quicker
        float bySpeed = rb.velocity.magnitude * FovBySpeed;

        gameCam.fieldOfView = Mathf.Clamp(Mathf.Lerp(gameCam.fieldOfView, FOV, 10 * Time.deltaTime) + bySpeed, 0, maxFOV);
    }
    void move()
    {
        //first instance of grounded/ player lands
        if (!isGrounded && Physics.CheckSphere(feet.position, 0.3f, whatIsGround))
        {
            //camera dampen
            if(!sliding) cam.GetComponent<CameraScript>().land();

            //clampSpeed = maxMoveSpeed;
        }

        if (rb.velocity.y < 0) jumping = false;

        isGrounded = Physics.CheckSphere(feet.position, 0.3f, whatIsGround);
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //x y velocity relative to where the player is looking
        Vector2 dirVelocity = FindVelRelativeToLook();


        controlDrag(dirVelocity);

        //used to cancel out movement if moving to fast
        float x = 1, y = 1;
        if (movement.x > 0 && dirVelocity.x > maxMoveSpeed) x = 0;
        if (movement.x < 0 && dirVelocity.x < -maxMoveSpeed) x = 0;
        if (movement.y > 0 && dirVelocity.y > maxMoveSpeed) y = 0;
        if (movement.y < 0 && dirVelocity.y < -maxMoveSpeed) y = 0;

        //multipliers for movement
        float multiplier = 1;
        if (!isGrounded) multiplier = airMoveMultiplier;

        if (!sliding)
        {
            if (isGrounded || wallRunning)
            {
                //add move force
                rb.AddForce(speed * orientation.right * movement.x * x * multiplier);
                rb.AddForce(speed * orientation.forward * movement.y * y * multiplier);
            }
            //if in air, normalize speed to limit diagonal movement (because movement is not clamped when in air)
            else rb.AddForce(((orientation.right * movement.x * x) + (orientation.forward * movement.y * y)).normalized * speed * multiplier);
        }
        //down force pick up speed when sliding down slopes
        else if(isGrounded) rb.AddForce(Vector3.down * slideDownForce);
    }
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    void controlDrag(Vector2 dirVelocity)
    {
        //no drag when in air
        if (!isGrounded && !wallRunning) return;

        //slide drag
        if (sliding)
        {
            Vector3 drag = new Vector3(-rb.velocity.x, 0, -rb.velocity.z).normalized;
            rb.AddForce(drag * slideDrag);
            return;
        }

        //drag
        float multiplier = 1;
        if (wallRunning) multiplier = airMoveMultiplier;

        if (Mathf.Abs(dirVelocity.x) > stopSpeed && movement.x == 0)
            rb.AddForce(orientation.right * -dirVelocity.x * groundDrag * multiplier);

        if (Mathf.Abs(dirVelocity.y) > stopSpeed && movement.y == 0)
            rb.AddForce(orientation.forward * -dirVelocity.y * groundDrag * multiplier);

        //clamp players velocity
        if (rb.velocity.magnitude > clampSpeed && !jumping)
        {
            Vector3 maxed = rb.velocity.normalized * clampSpeed;
            rb.velocity = new Vector3(maxed.x, rb.velocity.y, maxed.z);
        }
    }
    void Jump()
    {
        if (!isGrounded && !wallRunning) return;

        if (sliding) stopSlide();

        //reset y velocity to 0 so no small jump if falling
        if(rb.velocity.y < 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //add up force
        if (wallRunning)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.AddForce(wallHit.normal * jumpForce, ForceMode.Impulse);
        }
        else rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumping = true;
    }
    void wallRun()
    {
        if (isGrounded)
        {
            wallRunning = false;
            return;
        }

        //check if there is a wall to the left or right of the player
        bool leftHit, rightHit = false;

        leftHit = Physics.Raycast(transform.position, -orientation.right, out wallHit, wallCheckDist, wallRunnable);
        if(!leftHit) rightHit = Physics.Raycast(transform.position, orientation.right, out wallHit, wallCheckDist, wallRunnable);

        //just landed on wall
        if(!wallRunning && (leftHit || rightHit))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //set clamp speed when starting wall run
            if (rb.velocity.magnitude > maxMoveSpeed) clampSpeed = rb.velocity.magnitude;
            else clampSpeed = maxMoveSpeed;
        }

        //lower clamp speed as player slows down
        if (rb.velocity.magnitude < clampSpeed) clampSpeed = rb.velocity.magnitude;
        if (clampSpeed < maxMoveSpeed) clampSpeed = maxMoveSpeed;

        //update wall run state
        if (leftHit || rightHit)
        {
            wallRunning = true;

            if (leftHit) tilt = -20;
            if (rightHit) tilt = 20;
        }
        else wallRunning = false;

        //wall run mechanics
        if (wallRunning)
        {
            rb.useGravity = false;

            //simulate gravity
            rb.AddForce(Vector3.down * wallGravity);

            FOV = 100;
        }
        else rb.useGravity = true;
    }
    void Slide()
    {
        sliding = true;

        //scale player down
        transform.localScale = new Vector3(transform.localScale.x, 0.75f, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - 1.25f, transform.position.z);

        if (rb.velocity.y > 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //reset camera pos to zero, so when landing camera doesn't glitch through the ground
        cam.GetComponent<CameraScript>().resetPosition();
    }
    void stopSlide()
    {
        sliding = false;

        //scale player to normal size
        transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z);
    }
}