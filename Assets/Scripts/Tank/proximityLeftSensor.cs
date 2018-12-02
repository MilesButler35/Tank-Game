using UnityEngine;
public class proximityLeftSensor : MonoBehaviour {

    [SerializeField] private Movement Robot;
    [SerializeField] private Renderer leftSensor;
    float originalSpeed;

    void Start()
    {
        leftSensor.material.color = Color.green;
		originalSpeed = Robot.GetComponent<Movement>().speed;
    }
    private void OnTriggerEnter(Collider other)
    {
		leftSensor.material.color = Color.red;
		Robot.speed = originalSpeed * 0.1f;
	}
    void OnTriggerStay(Collider other)
    {
		if (Robot.contactSensors == false)
		{
			leftSensor.material.color = Color.red;
			Robot.transform.Rotate(new Vector3(0f, 45f, 0f) * Time.deltaTime);
		}
    }
    void OnTriggerExit(Collider other)
    {
		leftSensor.material.color = Color.green;
		if (Robot.contactSensors == false)
		{
			Robot.speed = originalSpeed;
		}
	}
}
