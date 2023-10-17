using UnityEngine;

public class move_Camera : MonoBehaviour
{
    float mainSpeed = 10.0f; //regular speed
    float shiftAdd = 250.0f; //multiplied by how long shift is held. Basically running
    float maxShift = 1000.0f; //Maximum speed when holding shift
    float rotateSpeed = 100.0f;
    private float totalRun = 1.0f;
    private Vector3 rotateCenter = new Vector3(4.5f, -0.05f, -8f);
    private Vector3 initialHitPoint; // 초기 hit 점의 위치

    private void Start()
    {
        //카메라 초점 확인
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            initialHitPoint = hit.point; // 초기 hit 점 저장
            Debug.Log(initialHitPoint);
        }
    }
    void Update()
    {
        //Keyboard commands
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            transform.Translate(p);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            RotateAroundPoint(new Vector3(4.75f, rotateCenter.y, 5.0f), -rotateSpeed * Time.deltaTime); // 음의 각도로 왼쪽 회전
        }
        if (Input.GetKey(KeyCode.E))
        {
            RotateAroundPoint(new Vector3(4.75f, rotateCenter.y, 5.0f), rotateSpeed * Time.deltaTime); // 양의 각도로 오른쪽 회전
        }
    }

    private Vector3 GetBaseInput()
    {
        //returns the basic values, if it's 0 then it's not active.
        Vector3 p_Velocity = new Vector3();
        if (!stone.isFight)
        {
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 0, 1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(1, 0, 0);
            }
        }
        return p_Velocity;
    }
    void RotateAroundPoint(Vector3 point, float angle)
    {
        Vector3 axis = Vector3.up;
        transform.RotateAround(point, axis, angle);
    }
}
