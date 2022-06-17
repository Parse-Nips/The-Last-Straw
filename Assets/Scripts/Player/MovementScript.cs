using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public Rigidbody2D RB2DRef;
    public Animator Animator;

    public Transform BottomRayPos;
    public Transform SideRayPos;

    public GameObject VisComponents;
    public SpriteRenderer MyRenderer;
    public GameObject MobileUI;

    public float Speed = 30;
    public float MaxSpeed = 30;

    public float JumpStrength = 300;
    public bool IsInAir;
    public bool IsMoving;
    public bool IsCasting = false;

    public float GroundRayOffset = 2.15f;
    public float SideRayOffset = 2.15f;

    private bool mobileMoveRight = false;
    private bool mobileMoveLeft = false;
    private bool mobileJump = false;

    public bool MobileMoveRight
    {
        get { return MobileMoveRight; }
        set { mobileMoveRight = value; }
    }
    public bool MobileMoveLeft
    {
        get { return MobileMoveLeft; }
        set { mobileMoveLeft = value; }
    }
    public bool MobileJump
    {
        get { return mobileJump; }
        set { mobileJump = value; }
    }






    // Use this for initialization
    void Start()
    {
        Debug.Log("MainGame");

            
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsCasting) {
            IsMoving = (CheckMove(KeyCode.A, mobileMoveLeft, Vector3.left) || CheckMove(KeyCode.D, mobileMoveRight, Vector3.right));
            CheckJump(new KeyCode[] { KeyCode.W, KeyCode.Space });

            Vector2 velocity = RB2DRef.velocity;
            if (Mathf.Abs(velocity.x) > MaxSpeed)
            {
                velocity.x = (velocity.x > 0) ? MaxSpeed : -MaxSpeed;
                RB2DRef.velocity = velocity;
            }
        }
        Animator.SetBool("IsWalking", IsMoving);
        Animator.SetBool("IsFlying", IsInAir);
        Animator.SetBool("IsCasting", IsCasting);
    }

    private bool CheckMove(KeyCode pKey, bool pMobileInput, Vector3 pDir)
    {
        if (Input.GetKey(pKey) || pMobileInput)
        {
            Vector3 offset = SideRayPos.position;
            Vector3 offsetUpper = offset + (Vector3.up * SideRayOffset);
            Vector3 offsetLower = offset + (Vector3.down * SideRayOffset);

            float deltaSpeed = Speed * Time.deltaTime;

            if (GameController.DEBUGMODE)
            {
                Debug.DrawRay(offsetLower, pDir * deltaSpeed, Color.green);
                Debug.DrawRay(offsetUpper, pDir * deltaSpeed, Color.green);
            }

            List<RaycastHit2D> Hits = new List<RaycastHit2D>();
            Hits.AddRange(Physics2D.RaycastAll(offsetLower, pDir, deltaSpeed).Where(o => (o.collider.tag == "Ground")).ToArray());
            Hits.AddRange(Physics2D.RaycastAll(offsetUpper, pDir, deltaSpeed).Where(o => (o.collider.tag == "Ground")).ToArray());

            if (Hits.Count > 0)
            {
                float dist = Hits[0].distance;
                if (dist > 0)
                {
                    Vector2 velo = RB2DRef.velocity;
                    velo.x = 0;
                    Vector3 move = pDir * dist;
                    RB2DRef.velocity = velo + (Vector2)(move * Time.deltaTime);
                }
            }
            else RB2DRef.velocity += (Vector2)pDir * deltaSpeed;

            Vector3 newScale = Vector3.one;
            if (pDir.x < 0)
            {
                MyRenderer.flipX = true;
                newScale.x = -1f;
            }
            else
            {
                MyRenderer.flipX = false;
                newScale.x = 1;
            }
            VisComponents.transform.localScale = newScale;

            return true;
        }
        return false;
    }
    private void CheckJump(KeyCode[] pKeys)
    {
        List<RaycastHit2D> AirbornSolverHits = new List<RaycastHit2D>();

        AirbornSolverHits.AddRange(Physics2D.RaycastAll(BottomRayPos.position + Vector3.left / GroundRayOffset, Vector2.down, 0.03f).Where(o => o.collider.tag == "Ground"));
        AirbornSolverHits.AddRange(Physics2D.RaycastAll(BottomRayPos.position + Vector3.right / GroundRayOffset, Vector2.down, 0.03f).Where(o => o.collider.tag == "Ground"));

        if (GameController.DEBUGMODE)
        {
            Debug.DrawRay(BottomRayPos.position + Vector3.left / GroundRayOffset, Vector2.down * 0.03f, Color.green);
            Debug.DrawRay(BottomRayPos.position + Vector3.right / GroundRayOffset, Vector2.down * 0.03f, Color.green);
        }

        IsInAir = !(AirbornSolverHits.Count > 0);

        if (IsInAir)
        {
            float yVelo = RB2DRef.velocity.y;
            if (yVelo < 0)
            {
                float dist = yVelo * Time.deltaTime;
                RaycastHit2D[] GroundSolverHits = Physics2D.RaycastAll(BottomRayPos.position, Vector2.down, Mathf.Abs(dist)).Where(o => o.collider.tag == "Ground").ToArray();

                if(GameController.DEBUGMODE) Debug.DrawRay(BottomRayPos.position, Vector2.down * Mathf.Abs(dist), Color.green);

                if (GroundSolverHits.Length > 0)
                {
                    Vector2 velo = RB2DRef.velocity;
                    velo.y = 0;
                    RB2DRef.velocity = velo + (Vector2)(Vector3.down * GroundSolverHits[0].distance) * Time.deltaTime;
                }
            }
        }
        else
        {
            bool keyPressed = pKeys.Any(o => (Input.GetKeyDown(o)));
            if (keyPressed || mobileJump)
            {
                RB2DRef.AddForce(Vector2.up * JumpStrength);
                Animator.SetTrigger("IsJumping");
            }
        }
    }
}