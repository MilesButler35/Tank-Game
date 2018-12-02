using UnityEngine;

public class Movement : MonoBehaviour {

	public float speed;
	public bool proximitySensors = false;
	public bool contactSensors = false;
	public bool rightSensorTriggered = false;
	public bool leftSensorTriggered = false;
    [SerializeField] private GameObject player;
    [SerializeField] private TankHealth targetHealth;
    [SerializeField] private TankMovement playerMove;

    private void Start()
    {
         //targetHealth= player.GetComponent<TankHealth>();
    }
    void FixedUpdate () {
		transform.Translate (Vector3.forward * speed * Time.deltaTime);

        if (player.activeSelf)
        {
            if ((player.transform.position - transform.position).magnitude < 25)
            {
                targetHealth.TakeDamage(0.2f);
                playerMove.StartCoroutine("Slow");
            }
        }
    }
    
    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 25);
    }


}
