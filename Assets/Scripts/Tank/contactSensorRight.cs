// Víctor Hasim Elexpe Ahamri
// 2017

using UnityEngine;
public class contactSensorRight : MonoBehaviour {
    [SerializeField] private Movement Robot;
    [SerializeField] private Renderer rightSensor;
    float originalSpeed;
    void Start()
    {
        rightSensor.material.color = Color.green;
        originalSpeed = Robot.speed;
		Robot.rightSensorTriggered = false;
	}
	void OnTriggerEnter(Collider other)
	{
		Robot.rightSensorTriggered = true;
		Robot.contactSensors = true;
        Robot.speed = 0f;
	}
	void OnTriggerStay(Collider other)
	{
		rightSensor.material.color = Color.red;
		Robot.rightSensorTriggered = true;
		Robot.contactSensors = true;
		Robot.speed = 0f;
		Robot.transform.Rotate(new Vector3(0f, -180, 0f) * Time.deltaTime);
	}
	void OnTriggerExit(Collider other)
	{
		Robot.rightSensorTriggered = false;
		rightSensor.material.color = Color.green;
		if (Robot.leftSensorTriggered == false)
		{
			Robot.contactSensors = false;
			Robot.speed = originalSpeed;
		}
		
	}
}
