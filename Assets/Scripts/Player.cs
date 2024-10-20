using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 9f;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    float angle;
    Vector3 velocity;
    Rigidbody rb;
    bool isDisabled;
    public event Action OnPlayerWin;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Guard.OnPlayerSpotted += DisablePlayer;
    }
    void Update()
    {
        Vector3 input = Vector3.zero;
        if (!isDisabled )
        {
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = input.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude,inputMagnitude,ref smoothMoveVelocity,smoothMoveTime);
        float targetAngle = Mathf.Atan2(input.x, input.z)*Mathf.Rad2Deg;
        angle =Mathf.LerpAngle(angle,targetAngle,Time.deltaTime*turnSpeed*inputMagnitude);
        //transform.eulerAngles = Vector3.up * angle;
        //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime*smoothInputMagnitude);
        velocity = transform.forward * smoothInputMagnitude * moveSpeed;
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Finish"))
        {
            DisablePlayer();
            OnPlayerWin?.Invoke();
        }
    }
    void DisablePlayer()
    {
        isDisabled = true;
    }
    void OnDestroy()
    {
       Guard.OnPlayerSpotted -= DisablePlayer;
        
    }
}
