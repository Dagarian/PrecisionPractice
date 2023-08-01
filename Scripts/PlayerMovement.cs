using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] float speed = 12f;
    [SerializeField] float gravityConst = -10f;

    [SerializeField] Transform onFloorCheck;
    [SerializeField] float floorCheckRadius = 0.2f;
    [SerializeField] LayerMask floorMask;

    private Vector3 _velocity;
    private bool _onFloor;

    private void Start()
    {
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.GetChild(2).transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    void Update()
    {
        _onFloor = Physics.CheckSphere(onFloorCheck.position, floorCheckRadius, floorMask);

        if (_onFloor && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movementVector = transform.right * x + transform.forward * z;

        controller.Move(movementVector * (speed * Time.deltaTime));

        _velocity.y += gravityConst * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);
    }
}
