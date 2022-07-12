using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float rotationSpeed = 380f;
    [SerializeField]
    private float limitY = 40f;

    private Vector3 cameraOffset;

    private void Awake()
    {
        cameraOffset = transform.position - target.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(GetComponent<CinemachineCollider>())
            CameraRotation();

        if (target != null)
        {
            Vector3 newPos = target.position + cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, 0.5f);
            transform.LookAt(target.position);
        }
    }

    private void CameraRotation()
    {
        Quaternion cumTurnAngelX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime, Vector3.up);
        Quaternion cumTurnAngelY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime, Vector3.left);

        if ((transform.rotation.y > 0.7f && transform.rotation.y > 0) || (transform.rotation.y < -0.7f && transform.rotation.y < 0))
        {
            cumTurnAngelY = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime, Vector3.left);
        }

        cameraOffset = cumTurnAngelX * cumTurnAngelY * cameraOffset;
    }
}
