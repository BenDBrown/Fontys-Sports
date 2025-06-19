using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{   
    public GameObject[] cubes;
    public Transform[] points;
    public float beat = (60/105)*2;
    private float timer;

    public GameObject[] footPlates;
    public Transform[] footPoints;
    public float footBeat = (60/105)*2;
    private float footTimer;

    private bool isSpawning = false; // Added flag to control spawning

    void Update()
    {
        if (!isSpawning) return; // Prevents spawning if not enabled

        if (timer > beat)
        {
            GameObject cube = Instantiate(cubes[Random.Range(0, cubes.Length)], points[Random.Range(0,points.Length)]);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.Rotate(transform.forward, 90 * Random.Range(0,4));
            timer -= beat;
        }

        timer += Time.deltaTime;

        if (footTimer > footBeat)
        {
            GameObject footCube = Instantiate(footPlates[Random.Range(0,footPlates.Length)], footPoints[Random.Range(0, footPoints.Length)]);
            footTimer -= footBeat;
        }

        footTimer += Time.deltaTime;
    }

    public void StartSpawning()
    {
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
}
