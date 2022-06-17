using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AdvancedMovementController))]
public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D MyRB;
    public AdvancedMovementController MyMovemenController;
    public Animator MyAnimator;
    public SpriteRenderer MySpriteRenderer;
    public GameObject MyBody;
    public GameObject MyMobileUI;

    public float MoveSpeed = 6;
    public float JumpHeight = 4;
    public float TimeToJumpApex = 0.4f;

    public float AccelerationTimeAirborn = 0.2f;
    public float AccelerationTimeGrounded = 0.1f;
    public bool IsCasting = false;

    public Vector2 Input= Vector2.zero;

    private float mJumpVelocity;
    private float mGravity;
    private float VelocityXSmoothing;


    private Vector3 mVelocity;

    private void Start()
    {
        if (!MyBody) Debug.LogError("PlayerScript - Make sure Body reference is assigned");
        if (!MyMovemenController)
        {
            MyMovemenController = GetComponent<AdvancedMovementController>();
            if (!MyMovemenController) Debug.LogError("PlayerScript - Make sure AdvancedMovemenController reference is assigned or on the same GameObject");
        }
        if (!MyRB)
        {
            MyRB = GetComponent<Rigidbody2D>();
            if (!MyRB) Debug.LogError("PlayerScript - Make sure RigidBody2D reference is assigned or on the same GameObject");
        }
        if(!MyAnimator ) Debug.LogError("PlayerScript - Make sure Animator reference is assigned");

        mGravity = -(2 * JumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
        mJumpVelocity = Mathf.Abs(mGravity) * TimeToJumpApex;
        Debug.Log(string.Format("Gravity: {0}   JumpVelocity: {1}", mGravity, mJumpVelocity));
    }

    private void Update()
    {
        mVelocity = MyRB.velocity;

        if (MyMovemenController.collisions.Above || MyMovemenController.collisions.Below) mVelocity.y = 0;

        if (IsCasting) Input = Vector2.zero;
        else
        {
            //if (!GameController.IS_MOBILE) Input = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Jump"));
            if (Input.y > 0 && MyMovemenController.collisions.Below)
            {
                mVelocity.y = mJumpVelocity;
                MyAnimator.SetTrigger("IsJumping");
            }
        }

        //mVelocity.x = Input.x * MoveSpeed;
        float targetVelocityX = Input.x * MoveSpeed;

        MyAnimator.SetBool("IsWalking", targetVelocityX != 0);
        MyAnimator.SetBool("IsFlying", !MyMovemenController.collisions.Below);

        mVelocity.x = Mathf.SmoothDamp(mVelocity.x, targetVelocityX, ref VelocityXSmoothing, (MyMovemenController.collisions.Below) ? AccelerationTimeGrounded : AccelerationTimeAirborn);
        mVelocity.y += mGravity * Time.deltaTime;

        Vector3 newVelocity = MyMovemenController.Move(mVelocity, Time.deltaTime);
        MyRB.velocity = newVelocity;

        if(Input.x != 0) MySpriteRenderer.flipX = (Input.x < 0);



        Input = Vector2.zero;
    }

    public void AddHorizontalMove(int pAmount)
    {
        Input.x += pAmount;
    }
    public void Jump()
    {
        Input.y = 1f;
    }
}