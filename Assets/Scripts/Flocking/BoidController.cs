using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// these define the flock's behavior
/// </summary>
public class BoidController : MonoBehaviour
{
    public BoidObject prefab;
    public Transform Target;

	public float MinVelocity = 5;
	public float MaxVelocity = 20;
	public float Randomness = 1;
	public int FlockSize = 20;
    public bool UseDistance = false;


    internal FlockMoveType MoveType;
    
	internal Vector2 flockCenter;
	internal Vector2 flockVelocity;
    internal float BounceForce;

    private GameObject mContainer;

	List<BoidObject> boids = new List<BoidObject>();

	void Start()
	{
        mContainer = new GameObject("Flock");
        SpawnCrows();
        Idle();
    }

	void Update()
	{
        if (FlockSize > 0)
        {
            Vector2 center = Vector2.zero;
            Vector2 velocity = Vector2.zero;
            FlockSize = boids.Count;
            foreach (BoidObject boid in boids)
            {
                center += (Vector2)boid.transform.localPosition;
                velocity += boid.GetComponent<Rigidbody2D>().velocity;
            }
            flockCenter = center / FlockSize;
            flockVelocity = velocity / FlockSize;

        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            ChangeMoveType(FlockMoveType.Idle);
        }
	}

    public void Add(int pAmount)
    {
        FlockSize += pAmount;
        SpawnCrows();
    }

    public void Move()
    {
        MinVelocity = 7;
        MaxVelocity = 20;
        Randomness = 5;
        UseDistance = true;
        ChangeMoveType(FlockMoveType.Move);
    }

    public void Idle()
    {
        MinVelocity = 3f;
        MaxVelocity = 10;
        Randomness = 25;
        UseDistance = false;
        ChangeMoveType(FlockMoveType.Idle);
    }

    public void ChangeMoveType(FlockMoveType pMoveType)
    {
        MoveType = pMoveType;
        foreach (BoidObject b in boids)
        {
            b.Adapt();
        }
    }

    public void Teleport(Vector2 pPos)
    {
        foreach (BoidObject b in boids)
        {
            b.transform.position = pPos;
        }
    }

    private void SpawnCrows()
    {
        for (int i = boids.Count; i < FlockSize; i++)
        {
            BoidObject boid = Instantiate(prefab, transform.position, transform.rotation, mContainer.transform) as BoidObject;
            boid.MyBoidController = this;
            boids.Add(boid);
        }
    }

    void OnDrawGizmos()
    {
        switch(MoveType)
        {
            case FlockMoveType.Idle:
                    Gizmos.color = Color.blue;
                break;
            case FlockMoveType.Move:
                    Gizmos.color = Color.red;
                break;
        }

        Gizmos.DrawSphere(flockCenter,0.05f);

        Gizmos.color = Color.white;
    }
}

public enum FlockMoveType
{
    Idle = 0,
    Move = 1,
}