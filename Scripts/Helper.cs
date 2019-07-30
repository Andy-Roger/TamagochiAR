using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

    public static float playerHeight = 0.5f;
    public static ActionState GetActionStateInstance(ActionState requestedActionStateType, StateMachine creatureStateMachine)
    {
        foreach(ActionState currentActionState in creatureStateMachine.ActionStates)
        {
            if (requestedActionStateType.GetType() == currentActionState.GetType())
            {
                return currentActionState;
            }
        }
        Debug.LogError("No ActionState on Creature called: " + requestedActionStateType.GetType().ToString());
        return creatureStateMachine.DefaultState;
    }

    public static ActionState GetCurrentStateInstance(StateMachine creatureStateMachine)
    {
        foreach (ActionState actionState in creatureStateMachine.ActionStates)
        {
            if (actionState.enabled == true)
            {
                return actionState;
            }
        }
        Debug.LogError("No ActionStates are currently active, something went wrong");
        return creatureStateMachine.DefaultState;
    }

    public static string GenerateRandomName()
    {
        string result = "";

        string[] kana = new string[] {"ka", "ki", "ku", "ke", "ko",
                                      "sa", "shi", "su", "se", "so",
                                      "ma", "mi", "mu", "me", "mo",
                                      "a", "i", "u", "e", "o",
                                      "ta", "chi", "tsu", "te", "to",
                                      "na", "ni", "nu", "ne", "no",
                                      "ha", "hi", "fu", "he", "ho",
                                      "ya", "yu", "yo",
                                      "ra", "ri", "ru", "re", "ro",
                                      "wa"};


        for (int i = 0; i < 3; i++)
        {
            int randIndex = Random.Range(0, kana.Length - 1);
            result += kana[randIndex];
        }

        return result;
    }
}
