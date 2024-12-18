using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousVehicleController1 : MonoBehaviour
{
    public float maxSpeed = 15f;  // 차량의 최대 속도
    public float acceleration = 4f;  // 가속도
    public float deceleration = 6f;  // 감속도
    public float turnSpeed = 2f;
    public float maxSteerAngle = 30f; // 최대 조향 각도 (앞바퀴가 좌우로 회전하는 각도)
    public float wheelRotationSpeed = 720f; // 바퀴 회전 속도

    private Transform wheelFL;  // 앞 왼쪽 바퀴
    private Transform wheelFR;  // 앞 오른쪽 바퀴
    private Transform wheelRL;  // 뒤 왼쪽 바퀴
    private Transform wheelRR;  // 뒤 오른쪽 바퀴
    private Rigidbody rb;
    private float currentSpeed = 0f;  // 현재 속도
    private float currentSteerAngle = 0f; // 현재 조향각

    void Start()
    {
        // 바퀴 찾기
        wheelFL = transform.Find("Wheel_FL");
        wheelFR = transform.Find("Wheel_FR");
        wheelRL = transform.Find("Wheel_RL");
        wheelRR = transform.Find("Wheel_RR");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveVehicle();
    }

    public void SetSteering(float steeringAngle)
    {
        // 차선에 따라 계산된 핸들 조작 값을 받아 설정
        currentSteerAngle = Mathf.Clamp(steeringAngle, -maxSteerAngle, maxSteerAngle);
    }

    void MoveVehicle()
    {
        float moveInput = 1f; // 자동 전진
        float steer = currentSteerAngle;

        // 앞바퀴의 조향 각도 적용
        wheelFL.localRotation = Quaternion.Euler(0, steer, 0);
        wheelFR.localRotation = Quaternion.Euler(0, steer, 0);

        // 속도 계산 (가속/감속 처리)
        currentSpeed += moveInput * acceleration * Time.deltaTime; // 가속도 적용
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed); // 최대 속도 제한

        // 차량 이동 처리 (Rigidbody의 velocity로 직접 속도 제어)
        Vector3 moveDirection = transform.forward * currentSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // 차량 회전 처리 (속도에 따라 회전 각도 조절)
        if (currentSpeed != 0)
        {
            float turnAmount = steer * turnSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turnAmount, 0));
        }

        // 바퀴 구름 처리
        RotateWheels(moveInput);
    }

    void RotateWheels(float moveInput)
    {
        // 바퀴가 차량이 전진/후진할 때 구르는 회전
        if (moveInput != 0f)
        {
            // 바퀴의 회전 각도 계산
            float wheelTurnAngle = moveInput * wheelRotationSpeed * Time.deltaTime;
            // 바퀴가 구르도록 회전 적용
            wheelFL.Rotate(Vector3.right, wheelTurnAngle);
            wheelFR.Rotate(Vector3.right, wheelTurnAngle);
            wheelRL.Rotate(Vector3.right, wheelTurnAngle);
            wheelRR.Rotate(Vector3.right, wheelTurnAngle);
        }
    }
}