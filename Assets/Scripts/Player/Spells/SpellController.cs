using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpellController : MonoBehaviour {

    public PlayerScript PlayerScript;
    public GameObject TargetPrefab;
    public BoidController FlockController;
    public GameObject spell;
    public AudioSource AS;
    public AudioClip ControlAudio;

    private RaycastHit mHit;
    private Camera mCam;
    public static GameObject TARGET;

    // Use this for initialization
    void Start()
    {
        TARGET = Instantiate(TargetPrefab, transform.position,transform.rotation);
        TARGET.SetActive(false);
        FlockController.Target = transform;
        mCam = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        bool overlaying;
        int touchID = 0;
        if (Input.touchCount > 0) touchID = Input.GetTouch(0).fingerId;


        if (Input.GetMouseButtonDown(0))
        {
            overlaying = (GameController.IS_MOBILE) ?
                EventSystem.current.IsPointerOverGameObject(touchID) :
                EventSystem.current.IsPointerOverGameObject();

            if (!overlaying)
            {
                AS.clip = ControlAudio;
                AS.Play();
                FlockController.Target = TARGET.transform;
                FlockController.Move();
                TARGET.SetActive(true);
            }
        }
        if (Input.GetMouseButton(0))
        {
            overlaying = (GameController.IS_MOBILE) ?
                EventSystem.current.IsPointerOverGameObject(touchID) :
                EventSystem.current.IsPointerOverGameObject();

            if (!overlaying)
            {
                if (Physics.Raycast(mCam.ScreenPointToRay(Input.mousePosition), out mHit))
                {
                    PlayerScript.IsCasting = true;
                    spell.SetActive(true);

                    Vector3 pos = mHit.point;
                    pos.z = 0;
                    TARGET.transform.position = pos;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            AS.Stop();
            spell.SetActive(false);
            TARGET.SetActive(false);
            PlayerScript.IsCasting = false;
            FlockController.Target = transform;
            FlockController.Idle();
        }
    }
}