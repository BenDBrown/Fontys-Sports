using UnityEngine;
using UnityEngine.InputSystem;

public class SportEquipmentFollowHandler : MonoBehaviour
{
    [SerializeField] private bool xrRigSimActive = false;
    [SerializeField] private bool rightControllerActive = true;
    [SerializeField] private GameObject sportEquipment;
    [Space(10)]
    [SerializeField] private InputActionReference leftIAR;
    [SerializeField] private Transform leftControllerFollower;
    [Space(10)]
    [SerializeField] private InputActionReference rightIAR;
    [SerializeField] private Transform rightControllerFollower;

    private Rigidbody sportEquipmentRigid;
    private SE_ColliderHandler colliderHandler;

    private void OnEnable()
    {
        leftIAR.action.performed += SwitchController;
        rightIAR.action.performed += SwitchController;
    }

    private void SwitchController(InputAction.CallbackContext context) => rightControllerActive = !rightControllerActive;

    private void Start()
    {
        if (xrRigSimActive) transform.position = transform.parent.GetChild(0).position;
        sportEquipmentRigid = sportEquipment.GetComponent<Rigidbody>();
        colliderHandler = sportEquipment.GetComponent<SE_ColliderHandler>();
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
            colliderHandler.Disable();
            sportEquipment.transform.parent = follower;
        }
        sportEquipmentRigid.MovePosition(follower.position);
        sportEquipmentRigid.MoveRotation(follower.rotation);
    }
}