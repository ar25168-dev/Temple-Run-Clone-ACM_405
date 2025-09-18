using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.XR;

public class PlayerRunnerExample : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 8f;
    [SerializeField] float laneWidth = 2f;
    [SerializeField] float laneChangeSpeed = 12f;
    [SerializeField] int currentLane = 0; // -1 = left, 0 = center, 1 = right
                                          // Update is called once per frame

    [Header("Input Keys")]
    [SerializeField] KeyCode moveLeft = KeyCode.A;
    [SerializeField] KeyCode moveRight = KeyCode.D;
    [SerializeField] int minLane = -1;
    [SerializeField] int maxLane = 1;


    void HandleLandInput()
    {
        if(Input.GetKeyDown(moveLeft) && currentLane > minLane)
        {
            currentLane--;
        }
        else if(Input.GetKeyDown(moveRight) && currentLane < maxLane)
        {
            currentLane++;
        }
    }
    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (GetComponent<PlayerJumpExample>().IsGrounded()==true)
        {
            HandleLandInput();
        }
        

        // Target position based on lane
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, currentLane * laneWidth, Time.deltaTime * laneChangeSpeed);
        transform.position = pos;
    }
}
