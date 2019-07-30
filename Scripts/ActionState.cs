using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionState : MonoBehaviour {

    public abstract void OnEnable();
    public abstract void HandleOutTriggered(ActionState outState);

}
