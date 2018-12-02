// Víctor Hasim Elexpe Ahamri
// 2017

using UnityEngine;
public class contactSensorLeft : MonoBehaviour {
    [SerializeField] private Movement Robot;
    [SerializeField] private Renderer leftSensor;
    float originalSpeed;
	void Start ()
    {
        leftSensor.material.color = Color.red;
		originalSpeed = Robot.speed;
		Robot.leftSensorTriggered = false;
	}
    void OnTriggerEnter(Collider collision)
    {
		Robot.leftSensorTriggered = true;
		Robot.contactSensors = true;
        Robot.speed = 0f;
	}
	void OnTriggerStay(Collider collision)
	{
		if (Robot.rightSensorTriggered == false)
		{
			leftSensor.material.color = Color.blue;
			Robot.contactSensors = true;
			Robot.speed = 0f;
			Robot.transform.Rotate(new Vector3(0f, 180, 0f) * Time.deltaTime);
		}
	}
    void OnTriggerExit(Collider collision)
    {
		Robot.leftSensorTriggered = false;
		leftSensor.material.color = Color.red;
		if (Robot.rightSensorTriggered == false)
		{
			Robot.contactSensors = false;
			Robot.speed = originalSpeed;
		}
	}
}
