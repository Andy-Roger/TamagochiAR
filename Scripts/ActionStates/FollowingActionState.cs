using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingActionState : ActionState
{
    [SerializeField] private float _moveToSpeed = 3;
    [SerializeField] private float _minLookAwayAngle = 0.99f;
    [SerializeField] private float _maxLookAwayAngle = 0.8f;
    [SerializeField] private float _rotateToSpeed = 3;

    private float _distToHuman;
    private bool _currentlyMoving = false;
    private float _angleToHuman;
    private bool _currentlyRotating = false;
    private Creature _creatureRef;

    void Awake()
    {
        _creatureRef = GetComponent<Creature>();
    }

    public override void OnEnable()
    {
        print("Following...");
    }

    void Update()
    {
        DoActionStateBehaviour();
    }

    IEnumerator FollowPlayer()
    {
        _creatureRef._animator.SetTrigger("WalkTrigger");

        while(_distToHuman >= _creatureRef.minDistToHuman)
        {
            Rotate(1); // it didnt look natural if the creature didn't rotate to the player here
            var newPos = Vector3.MoveTowards(transform.position, Player.playerInstance.transform.position, _moveToSpeed * Time.deltaTime);
            var newPosOnGround = new Vector3(newPos.x, Helper.playerHeight, newPos.z);
            transform.position = newPosOnGround;
            yield return null;
        }

        ReturnToIdle();
    }

    IEnumerator LookAtPlayer()
    {
        // Note: this should walk while rotating but there was a bug that made it keep walking after done
        _creatureRef._animator.SetTrigger("WalkTrigger");
        while (_angleToHuman <= _minLookAwayAngle)
        {
            Rotate(1);
            yield return null;
        }
        ReturnToIdle();
    }

    public override void HandleOutTriggered(ActionState outState)
    {
        var nextState = Helper.GetActionStateInstance(outState, GetComponent<StateMachine>());
        GetComponent<StateMachine>().TriggerNext(nextState);
    } 

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void DoActionStateBehaviour()
    {
        _creatureRef.ModifyHappiness(-0.1f);
        _creatureRef.ModifyWeight(-0.1f);

        Vector3 playerPosition = Player.playerInstance.transform.position;
        DoMoveDetector(playerPosition);
        DoRotateDetector(playerPosition);
    }

    void DoMoveDetector(Vector3 playerPosition)
    {
        _distToHuman = Vector3.Distance(transform.position, GetPointUnder(playerPosition));
        if (_distToHuman >= _creatureRef.maxDistToHuman && !_currentlyMoving)
        {
            _currentlyMoving = true;
            StartCoroutine(FollowPlayer());
        }
    }

    void DoRotateDetector(Vector3 playerPosition)
    {
        if (!_currentlyMoving)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = GetPointUnder(playerPosition) - transform.position;
            toOther.Normalize();
            _angleToHuman = Vector3.Dot(forward, toOther);
            if (Vector3.Dot(forward, toOther) < _maxLookAwayAngle && _distToHuman > _creatureRef.minDistToHuman - 0.3f)
            {
                _currentlyRotating = true;
                StartCoroutine(LookAtPlayer());
            }
        }
    }

    void Rotate(float speedMultiplier)
    {
        Vector3 playerPosition = Player.playerInstance.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(playerPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 bodyXRotationLockHeight = new Vector3(0, Helper.playerHeight, 0);
            Vector3 targetDir = hit.point - transform.position + bodyXRotationLockHeight;
            float step = _rotateToSpeed * Time.deltaTime * speedMultiplier;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    Vector3 GetPointUnder(Vector3 playerPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return playerPosition;
    }

    void ReturnToIdle()
    {
        _creatureRef._animator.SetTrigger("IdleTrigger");
        _currentlyRotating = false;
        _currentlyMoving = false;
    }
}
