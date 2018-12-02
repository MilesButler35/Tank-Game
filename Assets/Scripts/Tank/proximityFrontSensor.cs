using UnityEngine;
public class proximityFrontSensor : MonoBehaviour {

    [SerializeField] private Movement Robot;
    [SerializeField] private Renderer frontSensor;
    float originalSpeed;

    void Start()
    {
        frontSensor.material.color = Color.green;
		originalSpeed = Robot.speed;
    }
    void OnTriggerEnter(Collider other)
    {
		frontSensor.material.color = Color.red;
		Robot.speed = originalSpeed * 0.5f;
		Robot.speed = originalSpeed * 0.4f;
		Robot.speed = originalSpeed * 0.3f;
		Robot.speed = originalSpeed * 0.2f;
		Robot.speed = originalSpeed * 0.1f;
	}
    void OnTriggerStay(Collider other)
    {
		frontSensor.material.color = Color.red;
		if (Robot.contactSensors == false)
		{
			Robot.speed = originalSpeed * 0.1f;
		}
    }
    void OnTriggerExit(Collider other)
    {
		frontSensor.material.color = Color.green;
		Robot.speed = originalSpeed;
	}
}
