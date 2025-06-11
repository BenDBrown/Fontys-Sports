using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SportEquipmentFollowHandler : MonoBehaviour
{
    [SerializeField] private bool rightControllerActive = true;
    [SerializeField] private GameObject sportEquipment;
    [Space(10)]
    [SerializeField] private InputActionReference leftIAR;
    [SerializeField] private Transform leftControllerFollower;
    [Space(10)]
    [SerializeField] private InputActionReference rightIAR;
    [SerializeField] private Transform rightControllerFollower;

    private Rigidbody sportEquipmentRigid;

    private void OnEnable()
    {
        leftIAR.action.performed += SwitchController;
        rightIAR.action.performed += SwitchController;
    }

    private void SwitchController(InputAction.CallbackContext context) => rightControllerActive = !rightControllerActive;

    private void Start() => sportEquipmentRigid = sportEquipment.GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (!rightControllerActive)
        {
            sportEquipmentRigid.MovePosition(leftControllerFollower.position);
            sportEquipmentRigid.MoveRotation(leftControllerFollower.rotation);
        }
        else
        {
            sportEquipmentRigid.MovePosition(rightControllerFollower.position);
            sportEquipmentRigid.MoveRotation(rightControllerFollower.rotation);
        }
    }
}