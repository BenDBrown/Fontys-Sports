using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SportEquipmentFollowHandler : MonoBehaviour
{
    [SerializeField] private bool rightControllerActive = true;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    [SerializeField] private GameObject sportEquipment;

    private Rigidbody sportEquipmentRigid;

    private void Start()
    {
        sportEquipmentRigid = sportEquipment.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!rightControllerActive)
        {
            sportEquipmentRigid.MovePosition(leftController.position);
            sportEquipmentRigid.MoveRotation(leftController.rotation);
        }
        else
        {
            sportEquipmentRigid.MovePosition(rightController.position);
            sportEquipmentRigid.MoveRotation(rightController.rotation);
        }
    }
}