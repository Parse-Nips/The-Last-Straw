using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerScript))]
public class PlayerInput : MonoBehaviour
{
    public GameObject MobileUI;

    public UnityEvent Move_Right;
    public UnityEvent Move_Left;
    public UnityEvent Move_Jump;

    void Start()
    {
        MobileUI.SetActive(GameController.IS_MOBILE);
    }

    void Update()
    {
        float Input = UnityEngine.Input.GetAxisRaw("Horizontal");
        bool jump = UnityEngine.Input.GetAxisRaw("Jump") > 0;

        if (Input >= 0.9f) Move_Right.Invoke();
        if (Input <= -0.9f) Move_Left.Invoke();
        if (jump) Move_Jump.Invoke();
    }
}
