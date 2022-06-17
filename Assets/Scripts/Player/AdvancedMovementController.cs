using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is a movement script without the use of a physics body
/// Next step is to adjust it to use raycast assisted rigidbody collisions and movement
/// </summary>
[RequireComponent(typeof(PlayerScript))]
public class AdvancedMovementController : MonoBehaviour
{
    public const float cSkinWidth = 0.015f;
    public int HorizontalRayCount = 4;
    public int VerticalRayCount = 4;

    public float MaxClimbAngle = 80;
    public float MaxDescendAngle = 75;

    public Collider2D Collider;
    public LayerMask CollisionMask;
    public CollisionInfo collisions;

    private RaycastOrigins mRaycastOrigins;
    private float mHorizontalRaySpacing, mVerticalRaySpacing;
    private Rigidbody2D mMyRB;
    // Start is called before the first frame update
    private void Start()
    {
        CalculateRaySpacing();
    }

    public Vector3 Move(Vector3 pVelocity, float pDelta)
    {
        pVelocity *= pDelta;

        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.VelocityOld = pVelocity;

        if (pVelocity.y < 0) DescendSlope(ref pVelocity);
        if (pVelocity.x != 0) HorizontalCollisions(ref pVelocity);
        if (pVelocity.y != 0) VerticalCollisions(ref pVelocity);

        //if (collisions.Below)
        //{
        //    Vector3 YMove = new Vector3(0, pVelocity.y + cSkinWidth);
        //    transform.position += YMove;
        //    pVelocity.y = 0;
        //}

        //transform.Translate(pVelocity);
        pVelocity /= pDelta;

        return pVelocity; ////
    }

    void HorizontalCollisions(ref Vector3 pVelocityRef)
    {
        float xDir = Mathf.Sign(pVelocityRef.x);
        float rayLen = Mathf.Abs(pVelocityRef.x) + cSkinWidth;
        for (int i = 0; i < HorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (xDir == -1) ? mRaycastOrigins.BottomLeft : mRaycastOrigins.BottomRight;
            rayOrigin += Vector2.up * (mHorizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * xDir, rayLen, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * xDir * rayLen, Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= MaxClimbAngle)
                {
                    if(collisions.DescendingSlope)
                    {
                        collisions.DescendingSlope = false;
                        pVelocityRef = collisions.VelocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - cSkinWidth;
                        pVelocityRef.x -= distanceToSlopeStart * xDir;
                    }
                    ClimbSlope(ref pVelocityRef, slopeAngle);
                    pVelocityRef.x += distanceToSlopeStart * xDir;
                }

                if(!collisions.ClimbingSlope || slopeAngle > MaxClimbAngle)
                {
                    pVelocityRef.x = (hit.distance - cSkinWidth) * xDir;
                    rayLen = hit.distance;

                    if(collisions.ClimbingSlope)
                    {
                        pVelocityRef.y = Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(pVelocityRef.x);
                    }

                    collisions.Left = xDir == -1;
                    collisions.Right = xDir == 1;
                }
            }
        }
    }
    void VerticalCollisions(ref Vector3 pVelocityRef)
    {
        float yDir = Mathf.Sign(pVelocityRef.y);
        float rayLen = Mathf.Abs(pVelocityRef.y) + cSkinWidth;
        for (int i = 0; i < VerticalRayCount; i++)
        {
            Vector2 rayOrigin = (yDir == -1) ? mRaycastOrigins.BottomLeft : mRaycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (mVerticalRaySpacing * i + pVelocityRef.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * yDir, rayLen, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * yDir * rayLen, Color.red);

            if(hit)
            {
                pVelocityRef.y = (hit.distance - cSkinWidth) * yDir;
                rayLen = hit.distance;

                if(collisions.ClimbingSlope)
                {
                    pVelocityRef.x = pVelocityRef.y / Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(pVelocityRef.x);
                }

                collisions.Below = yDir == -1;
                collisions.Above = yDir == 1;
            }
        }
        if(collisions.ClimbingSlope)
        {
            float dirX = Mathf.Sign(pVelocityRef.x);
            rayLen = Mathf.Abs(pVelocityRef.x) + cSkinWidth;
            Vector2 rayOrigin = ((dirX == -1) ? mRaycastOrigins.BottomLeft : mRaycastOrigins.BottomRight) + Vector2.up * pVelocityRef.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLen, CollisionMask);

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.SlopeAngle)
                {
                    pVelocityRef.x = (hit.distance - cSkinWidth) * dirX;
                    collisions.SlopeAngle = slopeAngle;
                }
            }
        }
    }
    void ClimbSlope(ref Vector3 pVelocityRef, float pSlopeAngle)
    {
        float moveDist = Mathf.Abs(pVelocityRef.x);
        float climbVelocityY = Mathf.Sin(pSlopeAngle * Mathf.Deg2Rad) * moveDist;

        if(pVelocityRef.y <= climbVelocityY)
        { 
            pVelocityRef.y = climbVelocityY;
            pVelocityRef.x = Mathf.Cos(pSlopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(pVelocityRef.x);
            collisions.Below = true;
            collisions.ClimbingSlope = true;
            collisions.SlopeAngle = pSlopeAngle;
        }
    }

    void DescendSlope(ref Vector3 pVelocityRef)
    {
        float dirX = Mathf.Sign(pVelocityRef.x);
        Vector2 rayOrigin = (dirX == -1) ? mRaycastOrigins.BottomRight : mRaycastOrigins.BottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, CollisionMask);

        if(hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= MaxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - cSkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(pVelocityRef.x)) 
                    {
                        float moveDist = Mathf.Abs(pVelocityRef.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                        pVelocityRef.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(pVelocityRef.x);
                        pVelocityRef.y -= descendVelocityY;

                        collisions.SlopeAngle = slopeAngle;
                        collisions.DescendingSlope = true;
                        collisions.Below = true;
                    }
                }
            }
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = Collider.bounds;
        bounds.Expand(cSkinWidth * -2);


        mRaycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        mRaycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        mRaycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        mRaycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    private void CalculateRaySpacing()
    {
        Bounds bounds = Collider.bounds;
        bounds.Expand(cSkinWidth * -2);

        HorizontalRayCount = Mathf.Clamp(HorizontalRayCount, 2, int.MaxValue);
        VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

        mHorizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
        mVerticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 TopLeft, TopRight;
        public Vector2 BottomLeft, BottomRight;
    }
    public struct CollisionInfo
    {
        public bool Above, Below, Left, Right, ClimbingSlope, DescendingSlope;
        public float SlopeAngle, SlopeAngleOld;
        public Vector3 VelocityOld;
        public void Reset()
        {
            Above = Below = Left = Right = ClimbingSlope = DescendingSlope = false;
            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0;
        }
    }
}
