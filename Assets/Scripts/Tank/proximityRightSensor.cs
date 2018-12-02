using UnityEngine;
public class proximityRightSensor : MonoBehaviour {
    [SerializeField] private Movement Robot;
    [SerializeField] private Renderer rightSensor;
	float originalSpeed;
    void Start()
    {
		rightSensor.material.color = Color.green;
		originalSpeed = Robot.speed;
    }
	private void OnTriggerEnter(Collider other)
	{
        rightSensor.material.color = Color.red;
		Robot.speed = originalSpeed * 0.1f;
	}
	void OnTriggerStay(Collider other)
    {
		if (Robot.contactSensors == false)
		{
            rightSensor.material.color = Color.red;
			Robot.transform.Rotate(new Vector3(0f, -45f, 0f) * Time.deltaTime);
		}
    }
    void OnTriggerExit(Collider other)
    {
        rightSensor.material.color = Color.green;
		if (Robot.contactSensors == false)
		{
			Robot.speed = originalSpeed;
		}
	}
}
