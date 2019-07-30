using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _ageText;
    [SerializeField] private Slider _weightSlider;
    [SerializeField] private Slider _happinessSlider;

    [SerializeField] private int _age = 0;
    [SerializeField] private float _weight = 12;
    [SerializeField] private float _happiness = 10;

    public float minDistToHuman = 5;
    public float maxDistToHuman = 10;
    public Animator _animator;

    void Start()
    {
        _nameText.text = "name: " + Helper.GenerateRandomName();
        _happinessSlider.maxValue = 10;
        _weightSlider.maxValue = 25;
        _ageText.text = _age.ToString();
        _weightSlider.value = _weight;
        _happinessSlider.value = _happiness;
    }

    public int Age {
        set {
            _age = value;
            _ageText.text = "AGE: " + _age.ToString();
        }
    }

    public float Weight {
        set {
            _weight += value;
            if (_weight <= 0.25f || _weight >= _weightSlider.maxValue)
            {
                _animator.SetTrigger("DieTrigger");
                GetComponent<StateMachine>().enabled = false;
            }
            else
            {
                _weightSlider.value = _weight;
            }
        }
    }

    public float Happiness {
        set {
            if (_happiness <= 0.25f)
            {
                _animator.SetTrigger("DieTrigger");
                GetComponent<StateMachine>().enabled = false;
            }
            else if(_happiness + value > _happinessSlider.maxValue)
            {
                _happiness = 10;
                return;
            }
            else
            {
                _happiness += value;
                _happinessSlider.value = _happiness;
            }
        }
    }

    void Update()
    {
        Age = Mathf.FloorToInt(Time.time / 10);
    }

    public void UITriggerState(ActionState stateToTrigger)
    {
        if(stateToTrigger.GetComponent<StateMachine>() != GetComponent<StateMachine>())
        {
            Debug.LogError("It looks like this event was hooked into the wrong creature");
        }
        else
        {
            var currentState = Helper.GetCurrentStateInstance(GetComponent<StateMachine>());
            currentState.HandleOutTriggered(Helper.GetActionStateInstance(stateToTrigger, GetComponent<StateMachine>()));
        }
    }

    public void ModifyWeight(float amount)
    {
        Weight = amount * Time.deltaTime;
    }

    public void ModifyHappiness(float amount)
    {
        Happiness = amount * Time.deltaTime;
    }
}
