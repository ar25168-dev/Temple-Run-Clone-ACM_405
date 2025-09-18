using System.Runtime.ConstrainedExecution;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class PLayerRunner : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    [SerializeField] private int lane = 0;
    [SerializeField] private float laneWidth = 12f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            lane -= 1;
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            lane += 1;
        }
        lane = Mathf.Clamp(lane, -1, 1);

    }
}