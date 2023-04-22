using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationScript : MonoBehaviour
{
    int firstValue, secondValue, temp, result;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) //for addition
        {
            Calculation("add");
        }
        if (Input.GetKeyDown(KeyCode.S)) //for subtraction
        {
            Calculation("subtract");
        }
        if (Input.GetKeyDown(KeyCode.M)) //for multiplication
        {
            Calculation("multiplication");
        }
        if (Input.GetKeyDown(KeyCode.D)) //for division
        {
            Calculation("divide");
        }
    }

    public void Calculation(string operation)
    {
        firstValue = Random.Range(1, 10);
        secondValue = Random.Range(1, 10);

        //for subtraction to avoid negative answers
        if(firstValue - secondValue < 0)
        {
            temp = secondValue;
            secondValue = firstValue;
            firstValue = temp;
        }

        if(operation == "add")
        {
            result = firstValue + secondValue;
        }
        if(operation == "subtract")
        {
            result = firstValue - secondValue;
        }
        if (operation == "multiplication")
        {
            result = firstValue * secondValue;
        }
        if (operation == "divide")
        {
            result = firstValue / secondValue;
        }

        Debug.Log("first value: " + firstValue + " second value: " + secondValue + " Result is: " + result);

    }
}
