using UnityEngine;
using UnityEngine.Events;

public class PingPongBall : MonoBehaviour
{
    public UnityEvent<float> scored;

    [SerializeField]
    BoxCollider Table1;
    [SerializeField]
    BoxCollider Table2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
