

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] KeyCode jumpButton = KeyCode.Space;
    [SerializeField] KeyCode sprintButton = KeyCode.LeftShift;
    CharacterController controller;
    [SerializeField] float adsSpeed, walkSpeed, runSpeed, gravityMult, jumpForce, floorStickForce, speedChangeFactor;
    float gravity = 9.8f, yVel;
    float curMoveSpeed;
    bool jumpTrigger, isGrounded;
    float xInput, yInput;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ApplyMovement();
    }

    void GetInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(jumpButton)) ApplyJumpForce();
    }

    void ApplyJumpForce()
    {
        if(controller.isGrounded)
        {
             jumpTrigger = true;
        }
    }

    float GetCurrentMoveSpeed()
    {
        if(!controller.isGrounded) return curMoveSpeed;
        float targetSpeed;

        if(PlayerGun.currentGun.isAiming && controller.isGrounded)
        {
            targetSpeed = adsSpeed;
        }
        else if(Input.GetKey(sprintButton) && controller.isGrounded && yInput > 0)
        {
            targetSpeed = runSpeed;
        }
        else
        {
            targetSpeed = walkSpeed;
        }
        curMoveSpeed = Mathf.Lerp(curMoveSpeed, targetSpeed, Time.deltaTime * speedChangeFactor);
        if(curMoveSpeed > runSpeed - 1 && targetSpeed == runSpeed) curMoveSpeed = runSpeed;
        return curMoveSpeed;
    }

    void CalculateUpDownVelocity()
    {
        yVel = controller.isGrounded ? -floorStickForce : yVel - (gravity * gravityMult * Time.deltaTime);
        if(isGrounded && !controller.isGrounded && yVel < 0)
        {
            //removes stick force when first falling
            yVel = 0;
        }
        isGrounded = controller.isGrounded;
        yVel = jumpTrigger ? jumpForce : yVel;
        jumpTrigger = false;
    }

    Vector3 moveDir()
    {
        return (transform.right * xInput + transform.forward * yInput).normalized;
    }

    float GetInputAmount()
    {
        //todo : lerp mag 
        float inpMag = new Vector2(xInput, yInput).magnitude;
        inpMag = inpMag > 1 ? 1 : inpMag;
        return inpMag;
    }

    Vector3 GetPlayerMoveAmount()
    {
        Vector3 moveAmnt = GetInputAmount() * moveDir() * GetCurrentMoveSpeed();
        return moveAmnt;
    }

    void ApplyMovement()
    {
        CalculateUpDownVelocity();
        Vector3 yVector = Vector3.up * yVel;
        Vector3 playerMovement = GetPlayerMoveAmount() + yVector;
        controller.Move(playerMovement * Time.deltaTime);
    }
}
