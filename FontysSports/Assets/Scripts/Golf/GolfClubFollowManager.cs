using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GolfClubFollowManager : MonoBehaviour
{
    [SerializeField] private Transform xrRigTransform;
    [SerializeField] private Transform golfClubLeftController;
    [SerializeField] private Transform golfClubRightController;
    [SerializeField] private GameObject golfClubLeft;
    [SerializeField] private GameObject golfClubRight;
    [SerializeField] private bool xrSimulatorActive = false;

    private float xrRigCameraHeight;
    private Rigidbody golfClubLeftRigid;
    private Rigidbody golfClubRightRigid;

    private void Start()
    {
        xrRigCameraHeight = xrRigTransform.GetChild(0).transform.position.y;
        golfClubLeftRigid = golfClubLeft.GetComponent<Rigidbody>();
        golfClubRightRigid = golfClubRight.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (golfClubLeft.activeSelf)
        {
            golfClubLeftRigid.MovePosition(golfClubLeftController.position);
            golfClubLeftRigid.MoveRotation(golfClubLeftController.rotation);
        }

        if (golfClubRight.activeSelf)
        {
            golfClubRightRigid.MovePosition(golfClubRightController.position);
            golfClubRightRigid.MoveRotation(golfClubRightController.rotation);
        }
    }

    private void Update()
    {
        transform.position = xrRigTransform.position;
        if (xrSimulatorActive) transform.position += new Vector3(0, xrRigCameraHeight, 0);
        transform.eulerAngles = xrRigTransform.eulerAngles;
    }
}