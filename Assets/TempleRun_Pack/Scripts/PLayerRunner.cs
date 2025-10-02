using JetBrains.Annotations;
using System.Runtime.ConstrainedExecution;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
public class PLayerRunner : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    [SerializeField] private int lane = 0;
    [SerializeField] private float laneWidth = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime); //This line moves the plsyer forward

        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    lane -= 1;
        //}
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    lane += 1;
        //}
        //lane = Mathf.Clamp(lane, -1, 1);



        transform.position = new Vector3(laneWidth * lane, transform.position.y, transform.position.z);
    }

    public void ChangeLane(InputAction.CallbackContext inputData)
    {
        Vector3 inputVector = inputData.ReadValue<Vector2>();

        Debug.Log(inputVector.x);

        if (inputVector.x != 0)
        {
            lane = Mathf.Clamp(lane + Mathf.RoundToInt(inputVector.x), -1, 1);
        }
    }
}