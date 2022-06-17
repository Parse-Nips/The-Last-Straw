using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidObject : MonoBehaviour
{
	internal BoidController MyBoidController;
    public Rigidbody2D MyRB;
    public GameObject Body;
    public SpriteRenderer Renderer;
    public Animator Animator;

    private float mWaitTimeBaseMin = 0.3f;
    private float mWaitTimeBaseMax = 0.5f;
    private float mWaitTimeMin = 0.3f;
    private float mWaitTimeMax = 0.5f;

    private float mWaitTime = 0.3f;
    private Coroutine RoutineFlock;
    private Vector2 TravelDirection = Vector2.zero;
    private bool mRedoMove = false;
    private Vector2 mAwayVector = Vector2.zero;

    private bool Pushing = false;

    /// <summary>
    /// TODO:
    /// Add State detection for Chase/Push (Check for objects on the way to detect which state)
    /// (Move Towards one point, the closer the more tight the flock, the further the target the more force)
    /// 
    /// Chase:
    /// - Probably delay flock yield based on distance
    /// - Apply force on fixed yield update
    /// - Recalculate force on collision with anything
    /// 
    /// Push:
    /// Appy force on update to ensure the same amount gets applied regardless of framertate
    /// - Detection:
    ///     - Raycast?
    ///     - Centre of flock?
    ///     - Per crow?
    ///     - User command?
    ///         Right click while controlling crows?
    /// 
    /// 
    /// 
    /// try:
    /// Add per frame update (Co routine calculations)
    /// Add colision raycast to stop flickering while pushing
    /// Pushing detection with per crow raycast
    /// </summary>

    private void Start()
    {
        Animator.SetFloat("RandomOffset", Random.Range(0.6f, 0.8f));
        RoutineFlock = StartCoroutine(Flock());
    }

    private void Update()
    {
        Vector2 follow = MyBoidController.Target.position - transform.position;

        if(Pushing)
        {
            NextMove();
            if (Mathf.Abs(Vector2.Angle(TravelDirection, follow)) > 90) mRedoMove = true;
        }
        float dist = follow.magnitude / 3;

        mWaitTimeMax = mWaitTimeBaseMax * (dist + 0.15f);
        mWaitTimeMin = mWaitTimeBaseMin * (dist + 0.15f);

        Vector2 vel = MyRB.velocity;
        if (Pushing) vel = follow;

        LookAtDirection(vel);
    }

    private void FixedUpdate()
    {
        if (!Pushing) MyRB.velocity = (TravelDirection * Random.Range(MyBoidController.MinVelocity, MyBoidController.MaxVelocity)) * Time.deltaTime;
        else MyRB.velocity = (TravelDirection * MyBoidController.MaxVelocity) * Time.deltaTime;
        ClampVelocity();
    }

    public void Adapt()
    {
        mRedoMove = true;
    }
    private IEnumerator Flock()
    {
        while (true)
        {
            for (float timer = mWaitTime; timer >= 0f; timer -= Time.deltaTime)
            {
                if (mRedoMove)
                {
                    mRedoMove = false;
                    break;
                }
                yield return null;
            }
            if (!Pushing)
            {
                NextMove();
                mWaitTime = Random.Range(mWaitTimeMin, mWaitTimeMax);
            }
            yield return null;
        }
    }
    private void NextMove()
    {
        float RandomMult = (Random.Range(MyBoidController.MinVelocity, MyBoidController.MaxVelocity) * 10);
        TravelDirection = (Steer() * RandomMult).normalized;
    }
    private Vector2 Steer()
    {
        float randomMultiplier;
        Vector2 RandomDirection = (Random.insideUnitCircle * 5).normalized;

        Vector2 myPos = transform.position;

        Vector2 toCentreVector = MyBoidController.flockCenter - myPos;
        Vector2 velocityDiff = MyBoidController.flockVelocity - MyRB.velocity;
        Vector2 toTargetVector = (Vector2)MyBoidController.Target.position - myPos;

        randomMultiplier = MyBoidController.Randomness;
        if (MyBoidController.UseDistance)
        {
            randomMultiplier = toTargetVector.magnitude + 1;
        }
        RandomDirection *= randomMultiplier;
        Vector2 result = mAwayVector + (toCentreVector + velocityDiff + ((toTargetVector * 8) + RandomDirection));
        mAwayVector = Vector2.zero;
        return result;
    }
    private void ClampVelocity()
    {
        if (MyBoidController)
        {
            float speed = MyRB.velocity.magnitude;
            if (speed > MyBoidController.MaxVelocity)
            {
                MyRB.velocity = MyRB.velocity.normalized * MyBoidController.MaxVelocity;
            }
            else if (speed < MyBoidController.MinVelocity)
            {
                MyRB.velocity = MyRB.velocity.normalized * MyBoidController.MinVelocity;
            }
        }
    }
    private void LookAtDirection(Vector2 pDir)
    {
        float rot_z = Mathf.Atan2(pDir.y, pDir.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(rot_z) > 90)
        {
            Renderer.flipY = true;
        }
        else Renderer.flipY = false;

        Body.transform.rotation = Quaternion.Lerp(Body.transform.rotation, Quaternion.Euler(0f, 0f, rot_z), 0.5f);
    }

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (MyBoidController)
        {
            switch (MyBoidController.MoveType)
            {
                case FlockMoveType.Idle:
                    Vector2 normal = collision.GetContact(0).normal;
                    mAwayVector = normal * MyBoidController.BounceForce;
                    transform.position += (Vector3)normal * 0.1f;
                    Pushing = false;
                    mRedoMove = true;
                    break;
                case FlockMoveType.Move:
                    mAwayVector = Vector2.zero;
                    Pushing = true;
                    break;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Pushing = false;
        mRedoMove = true;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = (Pushing) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.2f, -0.2f), 0.1f);
        Gizmos.DrawRay(transform.position, TravelDirection);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}