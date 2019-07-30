using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    public ActionState DefaultState;
    public List<ActionState> ActionStates;

    void Awake()
    {
        InitActionStates();
    }

    private void InitActionStates()
    {
        List<ActionState> tempActionStates = new List<ActionState>();

        // loop through all components, if it is derived from actionState, add to list
        foreach (Component component in gameObject.GetComponents<ActionState>())
        {
            if (component.GetType().BaseType == typeof(ActionState))
            {
                tempActionStates.Add((ActionState)component);
            }
        }

        ActionStates = tempActionStates;
        TriggerNext(DefaultState);
    }

    public void TriggerNext(ActionState requestedState)
    {
        // turn all ActionStates off - do all to confirm no defects
        foreach (ActionState actionState in ActionStates)
        {
            actionState.enabled = false;
        }

        foreach (ActionState actionState in ActionStates)
        {
            // catch current state
            if (actionState.GetType() == requestedState.GetType())
            {
                EnterState(actionState);
            }
        }
    }

    private void EnterState(ActionState currentState)
    {
        currentState.enabled = true;
    }
}
