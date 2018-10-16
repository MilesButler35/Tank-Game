using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    private float radiusOfSatisfaction = 70f;
    public Transform target;
    float speed = 6;
    float turnSpeed = 2;
    Vector3[] path;
    int targetIndex;
    bool m_Fired = false;
    Vector3 destination, towards;
    int maxShell = 1;
    int count = 0;

    void Update()
    {

        destination = target.position; //updates the position of the player
        Vector3 towards = destination - transform.position; //the distance between
        Debug.Log(towards.magnitude);
        if (towards.magnitude > radiusOfSatisfaction)
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
        else
        {
            if(count < maxShell)
            {
                Blast();
                
            }
           

        }
        
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Vector3 targetDir = currentWaypoint - transform.position;
            Quaternion angle = Quaternion.LookRotation(targetDir);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed  * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, angle, turnSpeed * Time.deltaTime);
            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 2);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }


    public void Blast()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        //Forward Raycast


        Debug.Log("Boom!");
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        //  shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 
        shellInstance.velocity = 100f * m_FireTransform.forward;
        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        count++ ;
    }
}