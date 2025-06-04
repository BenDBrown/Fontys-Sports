using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class FollowHandsHandler : MonoBehaviour
{
    [SerializeField] private InputActionManager inputAction;
    [SerializeField] private XRInputModalityManager inputManager;
    [SerializeField] private Transform golfClubLeft;
    [SerializeField] private Transform golfClubRight;
    [SerializeField] private bool xrSimulatorActive = false;

    [Header("Golf club offset for controller tracking")]
    [SerializeField] private float forwardMultiplier = -0.02f;
    [SerializeField] private float upMultiplier = 0;

    private Transform xrRigTransform;
    private float xrRigCameraHeight;

    private InputAction xriLeftPos;
    private InputAction xriLeftRot;
    private InputAction xriRightPos;
    private InputAction xriRightRot;

    private void Start()
    {
        xrRigTransform = inputAction.transform;
        xrRigCameraHeight = xrRigTransform.GetChild(0).transform.position.y;

        InputActionAsset actionAsset = inputAction.actionAssets[0];
        InputActionMap xriLeft = actionAsset.FindActionMap("XRI Left");
        xriLeftPos = xriLeft.FindAction("Position");
        xriLeftRot = xriLeft.FindAction("Rotation");

        InputActionMap xriRight = actionAsset.FindActionMap("XRI Right");
        xriRightPos = xriRight.FindAction("Position");
        xriRightRot = xriRight.FindAction("Rotation");
    }

    private void Update()
    {
        if (golfClubLeft != null) FollowLeftHand();
        if (golfClubRight != null) FollowRightHand();

        transform.position = xrRigTransform.position;
        if (xrSimulatorActive) transform.position += new Vector3(0, xrRigCameraHeight, 0);
        transform.eulerAngles = xrRigTransform.eulerAngles;
    }

    private void FollowLeftHand()
    {
        golfClubLeft.SetLocalPositionAndRotation(xriLeftPos.ReadValue<Vector3>(), xriLeftRot.ReadValue<Quaternion>());
        golfClubLeft.position += golfClubLeft.forward * forwardMultiplier + golfClubLeft.up * upMultiplier;
    }

    private void FollowRightHand()
    {
        golfClubRight.SetLocalPositionAndRotation(xriRightPos.ReadValue<Vector3>(), xriRightRot.ReadValue<Quaternion>());
        golfClubRight.position += golfClubRight.forward * forwardMultiplier + golfClubRight.up * upMultiplier;
    }
}