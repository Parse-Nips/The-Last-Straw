using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadoxScript : MonoBehaviour {
    public GameObject GhostPrefab;
    public BoidController MyFlockController;
    public AudioSource AS;
    public SpriteRenderer Sprite;
    public AudioClip ParadoxSound;

    public float FadeTime = 1f;

    public List<Transform> History = new List<Transform>();

    private Vector3 PlayerStart;
    private GameObject mGhost;
    private float mTargetAlpha;
    private Coroutine mParadoxRoutine;
    private Coroutine mFadeRoutine;

    private bool mMobileParadox = false;

    private bool mPreviouslySimulatedRBB;

    public bool MobileParadox
    {
        get { return mMobileParadox; }
        set { mMobileParadox = value; }
    }

    private void Start()
    {
        PlayerStart = transform.position;
    }
    void Update () {

        if(MobileParadox)
        {

        }
        else
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                StartParadoxShift();
            }
            else if(Input.GetKeyUp(KeyCode.F))
            {
                StopParadoxShift();
            }
        }
	}

    void StartParadoxShift()
    {
        if (AS.clip != ParadoxSound) AS.clip = ParadoxSound;
        AS.Play();

        if (mGhost != null) Destroy(mGhost);

        mGhost = Instantiate(GhostPrefab, transform.position, transform.rotation);
        mGhost.GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rbb;
        if (rbb = GetComponent<Rigidbody2D>())
        {
            mPreviouslySimulatedRBB = rbb.simulated;
            rbb.simulated = false;
        }

        if (mFadeRoutine != null) StopCoroutine(mFadeRoutine);
        mFadeRoutine = StartCoroutine(Fade(0.3f, FadeTime));
        ParadoxObject.SHIFTING = true;
    }
    void StopParadoxShift()
    {
        Rigidbody2D rbb;
        if (rbb = GetComponent<Rigidbody2D>())
        {
            rbb.simulated = mPreviouslySimulatedRBB;
        }

        ParadoxObject.SHIFTING = false;
        mGhost.GetComponent<Collider2D>().enabled = true;
        MyFlockController.Teleport(transform.position);

        if (mFadeRoutine != null) StopCoroutine(mFadeRoutine);
        mFadeRoutine = StartCoroutine(Fade(1, FadeTime));
    }

    IEnumerator Paradox(float pDelay)
    {
        for (float i = pDelay; i > 0; i -= Time.deltaTime)
        {
            yield return null;
        }

        transform.SetPositionAndRotation(PlayerStart, Quaternion.identity);
        MyFlockController.Teleport(transform.position);
        mGhost.GetComponent<Collider2D>().enabled = true;

        DoFade(1, pDelay / 2);

        mMobileParadox = false;
        mParadoxRoutine = null;
    }

    void DoFade(float pTargetAlpha, float pFadeTime)
    {
        if (mFadeRoutine != null) StopCoroutine(mFadeRoutine);
        mFadeRoutine = StartCoroutine(Fade(pTargetAlpha, pFadeTime));
    }

    IEnumerator Fade(float pTargetAlpha, float pFadeTime)
    {

        while (true)
        {
            Color c = Sprite.color;
            c.a = Mathf.Lerp(Sprite.color.a, pTargetAlpha, Time.deltaTime / pFadeTime);
            Sprite.color = c;
            if (Mathf.Approximately(Sprite.color.a, pTargetAlpha))
            {
                yield break;
            };
            yield return null;
        }
    }
}
