using UnityEngine;
using UnityEngine.InputSystem;

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
    private DisableColliders disableColliders;
    private bool swapping = false;

    private void OnEnable()
    {
        leftIAR.action.performed += SwitchController;
        rightIAR.action.performed += SwitchController;
    }

    private void SwitchController(InputAction.CallbackContext context) => rightControllerActive = !rightControllerActive;

    private void Start()
    {
        sportEquipmentRigid = sportEquipment.GetComponent<Rigidbody>();
        disableColliders = sportEquipment.GetComponent<DisableColliders>();
    }

    private void FixedUpdate()
    {
        if (!rightControllerActive) MoveSportEquipment(leftControllerFollower);
        else MoveSportEquipment(rightControllerFollower);
    }

    private void MoveSportEquipment(Transform follower)
    {
        if (sportEquipment.transform.parent != follower)
        {
            swapping = true;
            disableColliders.Disable();
            sportEquipment.transform.parent = follower;
        }
        sportEquipmentRigid.MovePosition(follower.position);
        sportEquipmentRigid.MoveRotation(follower.rotation);
        if (swapping)
        {
            StartCoroutine(disableColliders.Enable());
            swapping = false;
        }
    }
}