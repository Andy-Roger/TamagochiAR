using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatActionState : ActionState
{
    [SerializeField] private float _actionLength = 3;
    [SerializeField] private float _timer = 0;

    private Creature _creatureRef;
    private float _happinessTimer = 0;

    void Awake()
    {
        _creatureRef = GetComponent<Creature>();
    }

    public override void OnEnable()
    {
        print("Eating...");
        _creatureRef._animator.SetTrigger("IdleTrigger");
        StartCoroutine(AutoLeaveState(_actionLength));
    }

    IEnumerator AutoLeaveState(float delay)
    {
        yield return new WaitForSeconds(delay);
        var nextState = GetComponent<FollowingActionState>();
        var stateMachine = GetComponent<StateMachine>();
        HandleOutTriggered(Helper.GetActionStateInstance(nextState, stateMachine));
    }

    void Update()
    {
        GainWeight();
        AddHappiness();
    }

    public override void HandleOutTriggered(ActionState outState)
    {
        var nextState = Helper.GetActionStateInstance(outState, GetComponent<StateMachine>());
        GetComponent<StateMachine>().TriggerNext(nextState);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        _happinessTimer = 0;
        _timer = 0;
    }

    void GainWeight()
    {
        // will incrementally gain weight
        _creatureRef.Weight = _timer;
        _timer = Time.deltaTime * 1f; // will gain 1 lbs a second
    }

    void AddHappiness()
    {
        _creatureRef.Happiness = _happinessTimer;
        _happinessTimer = Time.deltaTime * 0.4f; // will gain  0.4 happiness a second
    }
}
