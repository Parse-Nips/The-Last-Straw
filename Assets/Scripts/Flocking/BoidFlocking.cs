using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidFlocking : MonoBehaviour
{
	internal BoidController MyBoidController;
    public Rigidbody2D MyRB;
    public GameObject Body;
    public SpriteRenderer Renderer;
    public Animator Animator;

    public Vector2 CurrentVelocity;
    public float RandomMult;
    public float BounceForce;
    public float PushForce;

    private float mWaitTimeBaseMin = 0.3f;
    private float mWaitTimeBaseMax = 0.5f;
    private float mWaitTimeMin = 0.3f;
    private float mWaitTimeMax = 0.5f;

    private float mWaitTime = 0.3f;
    private Coroutine RoutineFlock;
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


    public void Adapt()
    {
        //if(MyBoidController)
        //{
        //    //if(RoutineFlock != null) StopCoroutine(RoutineFlock);
        //    //switch(MyBoidController.MoveType)
        //    //{
        //    //    case FlockMoveType.Idle:
        //    //        RoutineFlock = StartCoroutine(Flock());
        //    //        break;
        //    //    case FlockMoveType.Move:
        //    //        RoutineFlock = StartCoroutine(Flock());
        //    //        break;
        //    //}
        //    //if (mWaitTime > mWaitTimeBaseMax * 1.5f) mRedoMove = true;
        //}

        if (MyBoidController)
        {
            switch (MyBoidController.MoveType)
            {
                case FlockMoveType.Idle:
                    RoutineFlock = StartCoroutine(Flock());
                    break;
                case FlockMoveType.Move:
                    RoutineFlock = StartCoroutine(Flock());
                    break;
            }
        }
        mRedoMove = true;
    }

    private void Start()
    {
        Animator.SetFloat("RandomOffset",Random.Range(0.6f, 0.8f));
        RoutineFlock = StartCoroutine(Flock());
    }

    private void Update()
    {
        Vector2 follow = MyBoidController.Target.position - transform.position;

        float dist = follow.magnitude / 2;

        mWaitTimeMax = mWaitTimeBaseMax * (dist + 0.2f);
        mWaitTimeMin = mWaitTimeBaseMin * (dist + 0.2f);


        if (Pushing)
        {
            MyRB.velocity += (Vector2)((MyBoidController.Target.position - transform.position) * PushForce) * Time.deltaTime;
            ClampVelocity();
        }

        #region LookAtVelocity
        Vector2 vel = MyRB.velocity;
        if (Pushing)
        {
            vel = follow;
        }
        float rot_z = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(rot_z) > 90)
        {
            Renderer.flipY = true;
        }
        else Renderer.flipY = false;

        Body.transform.rotation = Quaternion.Lerp(Body.transform.rotation, Quaternion.Euler(0f, 0f, rot_z), 0.5f);
        #endregion

        //Vector2 follow = MyBoidController.Target.position - transform.position;

        //float dist = follow.magnitude/3;

        //mWaitTimeMax = mWaitTimeBaseMax * (dist + 0.2f);
        //mWaitTimeMin = mWaitTimeBaseMin * (dist + 0.2f);

        //Vector2 vel = MyRB.velocity;
        //float rot_z = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;

        //if (MyBoidController.MoveType == FlockMoveType.Move)
        //{
        //    MyRB.velocity += (((Vector2)MyBoidController.Target.position - (Vector2)transform.position).normalized * 5f) * Time.deltaTime;
        //    ClampVelocity();
        //}
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

            //NextMove();
            //mWaitTime = Random.Range(mWaitTimeMin, mWaitTimeMax);
            //yield return new WaitForSeconds(mWaitTime);
        }
    }

    private void NextMove()
    {
        if (MyBoidController)
        {
            CurrentVelocity = Steer();
        }
        RandomMult = (Random.Range(MyBoidController.MinVelocity, MyBoidController.MaxVelocity) * 10);

        MyRB.velocity = ((CurrentVelocity * RandomMult) * mWaitTime);

        ClampVelocity();
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
            randomMultiplier = toTargetVector.magnitude +1 ;
        }
		RandomDirection *= randomMultiplier;
        Vector2 result = mAwayVector + (toCentreVector + velocityDiff + ((toTargetVector * 8) + RandomDirection));
        mAwayVector = Vector2.zero;
        return result;
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (MyBoidController)
        {
            switch (MyBoidController.MoveType)
            {
                case FlockMoveType.Idle:
                    Vector2 normal = collision.GetContact(0).normal;
                    mAwayVector = normal * BounceForce;
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

        //if (MyBoidController)
        //{
        //    switch (MyBoidController.MoveType)
        //    {
        //        case FlockMoveType.Idle:
        //            Vector2 normal = collision.GetContact(0).normal;
        //            mAwayVector = normal * BounceForce;
        //            transform.position += (Vector3)normal * 0.1f;
        //            break;
        //        case FlockMoveType.Move:
        //            mAwayVector = Vector2.zero;
        //            break;
        //    }
        //}
        //mRedoMove = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        //if (MyBoidController)
        //{
        //    switch (MyBoidController.MoveType)
        //    {
        //        case FlockMoveType.Idle:
        //            Vector2 normal = collision.GetContact(0).normal;
        //            mAwayVector = normal * BounceForce;
        //            transform.position += (Vector3)normal * 0.1f;
        //            break;
        //        case FlockMoveType.Move:
        //            mAwayVector = Vector2.zero;
        //            break;
        //    }
        //}
        //mRedoMove = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Pushing = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (Pushing) ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.2f, -0.2f),0.1f);

        //Gizmos.color = (Pushing) ? Color.green : Color.red;
        //Gizmos.DrawSphere(transform.position + new Vector3(0, 0.2f, -0.2f), 0.1f);

        //Gizmos.color = (Pushing) ? Color.green : Color.red;
        //Gizmos.DrawSphere(transform.position + new Vector3(0, 0.2f, -0.2f), 0.1f);
    }
}