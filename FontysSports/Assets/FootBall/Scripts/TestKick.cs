using UnityEngine;

public class TestKick : MonoBehaviour
{
    [SerializeField]
    private Quaternion windBackRotation;

    [SerializeField]
    private Quaternion windForwardRotation;

    [SerializeField]
    private float speed = 0.0001f;

    private float time = 0;

    private bool kicking = false;

    private Quaternion rotation;

    private void Update()
    {
        if (kicking)
        { 
            transform.rotation = Quaternion.Lerp(windBackRotation, windForwardRotation, time * speed);
            time += Time.deltaTime;
            if(transform.rotation == windForwardRotation) StopKicking();
        }
    }

    public void Kick()
    {
        time = 0;
        transform.rotation = windBackRotation;
        kicking = true;
    }

    private void StopKicking()
    { 
        kicking = false;
        transform.rotation = Quaternion.identity;
    }
}
