using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{

    public Rigidbody m_Shell;                   // Prefab of the shell.
    [SerializeField] private Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    [SerializeField] private Transform m_BackRTransform;
    [SerializeField] private Transform m_BackLTransform;
    [SerializeField] private Transform m_BackMTransform;
    [SerializeField] private AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    [SerializeField] private AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    public Transform target;
    [SerializeField] private float speed = 12;
    [SerializeField] private float turnSpeed = 2;
    private float turnDst = 5;
    private float stoppingDst = 10;
    private float radiusOfSatisfaction = 120f;

    private float shotCooldown = 1.1f;
    private bool offCooldown = true;
    private int sittingStill = 0;
    private Vector3 lastPos;

    Paths path;

    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private void FixedUpdate()
    {
        if (offCooldown && CheckBarrelLOS())
        {
            StopCoroutine("FollowPath");
            TurnAndShoot();
        }
        else
        {
            if (CheckGeneralLOS())
            {
                StopCoroutine("FollowPath");
                MoveToTarget(target.position);                           
            }
            else
            {
                if (Round(transform.position) == lastPos)
                {
                    sittingStill++;
                    if (sittingStill > 30)
                    {
                        print("we stuck");
                        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                        sittingStill = 0;
                    }
                }
                else
                {
                    lastPos = Round(transform.position);
                    sittingStill = 0;
                }
            }
        }
    }

    public Vector3 Round(Vector3 vector3)
    {
        Vector3 yeet = new Vector3(
            Mathf.RoundToInt(vector3.x),
            Mathf.RoundToInt(vector3.y),
            Mathf.RoundToInt(vector3.z));
        return yeet;

    }

    private void MoveToTarget(Vector3 _target)
    {
        Vector3 towards = _target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(towards);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
    }

    private void TurnAndShoot()
    {
        Vector3 blastTarget = Random.insideUnitSphere * 24;
        blastTarget.y = 0;
        blastTarget = blastTarget + target.position;
        Vector3 towards = blastTarget - transform.position;
        Quaternion targetAngle = Quaternion.LookRotation(towards);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * turnSpeed*20);
        Blast();
        StartCoroutine(Reload());
    }

    private bool CheckGeneralLOS()
    {
        RaycastHit hit;

        if (Physics.Raycast(m_BackRTransform.position, (target.position - m_BackRTransform.position).normalized * radiusOfSatisfaction, out hit, radiusOfSatisfaction))
        {
            if (hit.collider.CompareTag("Player") && hit.transform != transform)
            {
                if(Physics.Raycast(m_BackLTransform.position, (target.position - m_BackLTransform.position).normalized * radiusOfSatisfaction, out hit, radiusOfSatisfaction))
                {
                    if (hit.collider.CompareTag("Player") && hit.transform != transform)
                    {
                        Debug.DrawLine(m_BackRTransform.position, target.position, Color.red);
                        Debug.DrawLine(m_BackLTransform.position, target.position, Color.red);
                        return true;

                    }
                }
            }
            else
            {
                Debug.DrawLine(m_BackRTransform.position, target.position, Color.green);
                Debug.DrawLine(m_BackLTransform.position, target.position, Color.green);
            }
        }
        // otherwise we return false
        return false;
    }

    private bool CheckBarrelLOS()
    {
        RaycastHit hit;
        //draws lines out form the center forward to check for raycast hits.
        Debug.DrawLine(m_BackMTransform.position, m_BackMTransform.position + (m_BackMTransform.forward * radiusOfSatisfaction), Color.black);
        if (Physics.Raycast(m_BackMTransform.position, m_BackMTransform.forward, out hit, radiusOfSatisfaction))
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


    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Paths(waypoints, transform.position, turnDst, stoppingDst);

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {

        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {

        bool followingPath = true;
        int pathIndex = 0;
        //transform.LookAt(path.lookPoints[0]);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {

                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    //print(speedPercent);
                    if (speedPercent < 0.30f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }

    public void Blast()
    {
        Rigidbody shellInstance =
        Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = 100f * m_FireTransform.forward;
        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}