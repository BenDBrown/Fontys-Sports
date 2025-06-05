using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GolfClubFollowManager : MonoBehaviour
{
    [SerializeField] private InputActionManager inputAction;
    [SerializeField] private XRInputModalityManager inputManager;
    [SerializeField] private GameObject golfClubLeftController;
    [SerializeField] private GameObject golfClubLeft;
    [SerializeField] private GameObject golfClubRightController;
    [SerializeField] private GameObject golfClubRight;
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

    private Rigidbody golfClubLeftRigid;
    private Rigidbody golfClubRightRigid;

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

        golfClubLeftRigid = golfClubLeft.GetComponent<Rigidbody>();
        golfClubRightRigid = golfClubRight.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (golfClubLeft.activeSelf) SetGolfClubLeftPos();
        if (golfClubRight.activeSelf) SetGolfClubRightPos();
    }

    private void SetGolfClubLeftPos()
    {
        golfClubLeftRigid.MovePosition(golfClubLeftController.transform.position);
        golfClubLeftRigid.MoveRotation(golfClubLeftController.transform.rotation);
    }

    private void SetGolfClubRightPos()
    {
        golfClubRightRigid.MovePosition(golfClubRightController.transform.position);
        golfClubRightRigid.MoveRotation(golfClubRightController.transform.rotation);
    }

    private void Update()
    {
        if (golfClubLeftController.activeSelf) SetGolfClubLeftControllerPos();
        if (golfClubRightController.activeSelf) SetGolfClubRightControllerPos();

        transform.position = xrRigTransform.position;
        if (xrSimulatorActive) transform.position += new Vector3(0, xrRigCameraHeight, 0);
        transform.eulerAngles = xrRigTransform.eulerAngles;
    }

    private void SetGolfClubLeftControllerPos()
    {
        golfClubLeftController.transform.SetLocalPositionAndRotation(xriLeftPos.ReadValue<Vector3>(), xriLeftRot.ReadValue<Quaternion>());
        golfClubLeftController.transform.position += golfClubLeftController.transform.forward * forwardMultiplier + golfClubLeftController.transform.up * upMultiplier;
    }

    private void SetGolfClubRightControllerPos()
    {
        golfClubRightController.transform.SetLocalPositionAndRotation(xriRightPos.ReadValue<Vector3>(), xriRightRot.ReadValue<Quaternion>());
        golfClubRightController.transform.position += golfClubRightController.transform.forward * forwardMultiplier + golfClubRightController.transform.up * upMultiplier;
    }
}