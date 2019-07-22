using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using System.Linq;

public class Player : MonoBehaviour
{
    public ExampleCharacterCamera OrbitCamera;
    public Transform CameraFollowPoint;
    public CharacterController Character;

    private Vector3 lookInputVector = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        OrbitCamera.SetFollowTransform(CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        OrbitCamera.IgnoredColliders = Character.GetComponentsInChildren<Collider>().ToList();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        float mouseLookAxisUp = Input.GetAxisRaw("Mouse Y");
        float mouseLookAxisRight = Input.GetAxisRaw("Mouse X");

        lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        float scrollInput = -Input.GetAxis("Mouse ScrollWheel");

        // Apply inputs to the camera
        OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level
        if (Input.GetMouseButtonDown(1))
        {
            OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f) ? OrbitCamera.DefaultDistance : 0f;
        }
    }
}
