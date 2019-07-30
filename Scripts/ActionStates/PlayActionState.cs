using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayActionState : ActionState
{
    [SerializeField] private GameObject _ball;
    [SerializeField] private float _throwForceScaler = 25000;
    [SerializeField] private float _runSpeed = 5;

    private bool _ballReturned = true;
    private FetchObjectAttachPoint _attachPoint;
    private Creature _creatureRef;

    void Awake()
    {
        _attachPoint = GetComponentInChildren<FetchObjectAttachPoint>();
        _creatureRef = GetComponent<Creature>();
    }

    // initialize all local state on enable
    public override void OnEnable()
    {
        _creatureRef._animator.SetTrigger("LionWaitTrigger");
        _ball.SetActive(true);
        _ball.transform.parent = Camera.main.transform;
        _ball.transform.localPosition = Vector3.forward * 0.5f; // resets the fetch bass to front of player
        print("Exercising...");
    }

    void OnDisable()
    {
        StopAllCoroutines();
        _ball.SetActive(false);
    }

    void Update()
    {
        // cannot launch ball if ball is not returned
        if (_ballReturned)
        {
            if (Input.GetMouseButtonUp(0))
            {
                LaunchBall();
                _ballReturned = false;
                StartCoroutine(GoToBall(_ball.transform));
            }
        }
    }

    public override void HandleOutTriggered(ActionState outState)
    {
        var nextState = Helper.GetActionStateInstance(outState, GetComponent<StateMachine>());
        GetComponent<StateMachine>().TriggerNext(nextState);
    }

    void LaunchBall()
    {
        _ball.transform.parent = null;
        var ballRB = _ball.GetComponent<Rigidbody>();
        ballRB.isKinematic = false;
        ballRB.AddForce(Camera.main.transform.forward * Time.deltaTime * _throwForceScaler);
    }

    IEnumerator GoToBall(Transform target)
    {
        _creatureRef._animator.SetTrigger("RunTrigger");

        while (Vector3.Distance(transform.position, target.position) > 0.5f)
        {
            _creatureRef.ModifyWeight(-0.025f);
            _creatureRef.ModifyHappiness(0.35f);

            var speed = _runSpeed * Time.deltaTime;
            var moveToPos = Vector3.MoveTowards(transform.position, target.position, speed);
            var moveToGroundPosition = new Vector3(moveToPos.x, Helper.playerHeight, moveToPos.z);
            transform.position = moveToGroundPosition;

            Rotate(target, 5);
            yield return null;
        }

        StartCoroutine(GoToHuman(Player.playerInstance.transform));
    }

    IEnumerator GoToHuman(Transform target)
    {
        float creatureHeight = Helper.playerHeight;
        while (Vector3.Distance(new Vector3(transform.position.x, creatureHeight, transform.position.z), target.position) > _creatureRef.minDistToHuman)
        {
            _creatureRef.ModifyWeight(-0.025f);
            _creatureRef.ModifyHappiness(0.25f);

            var speed = _runSpeed * Time.deltaTime;
            var moveToPos = Vector3.MoveTowards(transform.position, target.position, speed);
            var moveToGroundPosition = new Vector3(moveToPos.x, Helper.playerHeight, moveToPos.z);
            transform.position = moveToGroundPosition;

            _ball.transform.position = _attachPoint.transform.position;

            Rotate(target, 5);
            yield return null;
        }

        _creatureRef._animator.SetTrigger("IdleTrigger");

        _ball.GetComponent<Rigidbody>().isKinematic = true;
        _ballReturned = true;

        ResetToDefaultMode();
    }

    void ResetToDefaultMode()
    {
        var nextState = GetComponent<FollowingActionState>();
        var stateMachine = GetComponent<StateMachine>();
        HandleOutTriggered(Helper.GetActionStateInstance(nextState, stateMachine));
    }


    void Rotate(Transform target, float rotateSpeed)
    {
        Vector3 targetPosition = target.position;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(targetPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 bodyXRotationLockHeight = new Vector3(0, Helper.playerHeight, 0);
            Vector3 targetDir = hit.point - transform.position + bodyXRotationLockHeight;
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
}
