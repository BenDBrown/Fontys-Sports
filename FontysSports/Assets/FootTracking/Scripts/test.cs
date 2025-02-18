using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    [SerializeField] InputAction a;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        Debug.Log(a.ReadValue<Vector3>().ToString());
    }

    
}
