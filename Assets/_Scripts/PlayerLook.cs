using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 0.2f;

    public Transform playerBody;
    public GameObject gameController;

    private float _xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool isPaused = gameController.GetComponent<MenuController>().GetIsPaused();
        if (!isPaused)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;


            _xRotation -= mouseY; //Y is controlled in camera script.
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    public float GetSens()
    {
        return mouseSensitivity;
    }

    public void SetSens(float input)
    {
        mouseSensitivity = input;
    }
}
