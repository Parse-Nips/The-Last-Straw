using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnslavementSpell : MonoBehaviour {
    public PlayerScript PlayerScript;
    public BoidController Flock;

    public AbsorbScript AbsorptionPrefab;
    public LayerMask Mask;
    public float Radius;

    private PeasantScript mPeasant;
    private AbsorbScript Absorber;
    private bool Absorbing;
    private bool mobile = false;

    void Start ()
    {
        mobile = Application.isMobilePlatform;
        Absorber = Instantiate(AbsorptionPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (mPeasant)
        {
            if (mobile)
            {
                Touch touch = Input.GetTouch(1);
                if (touch.phase == TouchPhase.Began)
                {
                    Absorbing = true;
                    Absorber.transform.position = mPeasant.transform.position;
                    Absorber.Target = transform;
                    PlayerScript.IsCasting = true;
                    Absorber.PS.Play();
                    Invoke("Absorb", mPeasant.AbsorbtionLength);
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    StopAbsorb();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Absorbing = true;
                    Absorber.transform.position = mPeasant.transform.position;
                    Absorber.Target = transform;
                    PlayerScript.IsCasting = true;
                    Absorber.PS.Play();
                    Invoke("Absorb", mPeasant.AbsorbtionLength);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopAbsorb();
        }
    }

    private void Absorb()
    {
        Flock.Add(mPeasant.Worth);
        Destroy(mPeasant.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!mPeasant)
        {
            PeasantScript p;
            if(p = collision.GetComponent<PeasantScript>())
            {
                mPeasant = p;
                mPeasant.SelectedPS.Play();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!mPeasant)
        {
            PeasantScript p;
            if (p = collision.GetComponent<PeasantScript>())
            {
                mPeasant = p;
                mPeasant.SelectedPS.Play();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        PeasantScript p;

        if (p = collision.GetComponent<PeasantScript>())
        {
            if (p == mPeasant)
            {
                mPeasant.SelectedPS.Stop();
                StopAbsorb();
                mPeasant = null;
            }
        }
    }
    private void StopAbsorb()
    {
        if (Absorbing)
        {
            Absorbing = false;
            Absorber.transform.position = transform.position;
            Absorber.Target = transform;
            PlayerScript.IsCasting = false;
            Absorber.PS.Stop();
            CancelInvoke("Absorb");
        }
    }
}
