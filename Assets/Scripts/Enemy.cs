using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    private float radiusOfSatisfaction = 70f;
    private float radius = 4;
    public Transform target;
    float speed = 6;
    float turnSpeed = 2;
    Vector3[] path;
    int targetIndex;
    Vector3 destination, towards;
    private float shotCooldown = 1.5f;
    private bool offCooldown = true;
    private bool KA = false;

    void Update()
    {
        //Debug.Log(towards.magnitude);
        //Debug.Log(offCooldown);
        if(offCooldown && CheckBarrelLOS())
        {
            TurnAndShoot();
        }
        else
        {
            if(CheckGeneralLOS())
            {
                KA = true;
                MoveToTarget(target.position);                
            }
            else
            {
                KA = false;
                MoveToPlayer();
            }
        }
        
    }

    private void TurnAndShoot()
    {
        Vector3 blastTarget = Random.insideUnitSphere * 10;
        blastTarget.y = 0;
        blastTarget = blastTarget + target.position;
        Debug.Log(blastTarget);
        // Calculate direction from character to target
        Vector3 towards = blastTarget - transform.position;
        Quaternion targetAngle = Quaternion.LookRotation(towards);
        for(int i=0; i < 20; i++)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * turnSpeed/100f);
        }
        Blast();
        StartCoroutine(Reload());
    }

    private void MoveToTarget(Vector3 _target)
    {
        // Calculate direction from character to target
        Vector3 towards = _target - transform.position;

        // --Jugen Code--
        // If we haven't reached the target yet
        if (towards.magnitude > radius)
        {
            // Checking how long it takes to move there
            towards /= turnSpeed;

            //if too fast, normalize the vector and cap the speed
            if (towards.magnitude > speed)
            {
                // Normalize vector to get just the direction
                towards.Normalize();
                towards *= speed;
            }
            //rb.velocity = towards;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        // Get and set angle of unit towards target
        Quaternion targetAngle = Quaternion.LookRotation(towards);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * turnSpeed);

    }


    private void MoveToPlayer()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private bool CheckGeneralLOS()
    {
        RaycastHit hit;
        //draws lines out form the center forward to check for raycast hits.
        Debug.DrawLine(transform.position, target.position, Color.red);
        if (Physics.Raycast(transform.position, (target.position - transform.position).normalized * radiusOfSatisfaction, out hit, radiusOfSatisfaction))
        {
            if (hit.collider.CompareTag("Player") && hit.transform != transform)
            {
                return true;
            }
        }
        // otherwise we return false
        return false;
    }
    private bool CheckBarrelLOS()
    {
        RaycastHit hit;
        //draws lines out form the center forward to check for raycast hits.
        Debug.DrawLine(transform.position, transform.position + (transform.forward * radiusOfSatisfaction), Color.black);
        if (Physics.Raycast(transform.position, transform.forward, out hit, radiusOfSatisfaction))
        {
            if (hit.collider.CompareTag("Player") && hit.transform != transform)
            {
                return true;
            }
        }
        // otherwise we return false
        return false;
    }

    IEnumerator Reload()
    {
        offCooldown = false;
        yield return new WaitForSeconds(shotCooldown);
        offCooldown = true;
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
        while (!KA)
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
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, radiusOfSatisfaction);
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.blue;
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
        int layerMask = 1 << 8;
        //Forward Raycast


        //Debug.Log("Boom!");

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        //  shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 
        shellInstance.velocity = 100f * m_FireTransform.forward;
        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}