using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNumber : MonoBehaviour
{

    int startNumber;
    int newNumber;
    Text textNumber;

    bool change;
    float time;

    public Color increaseColor;
    public Color decreaseColor;

    Color originalColor;
    // Use this for initialization
    void Awake()
    {
        textNumber = GetComponent<Text>();

        originalColor = textNumber.color;
        //increaseColor = Color.green;
        //decreaseColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (change)
        {
            time += Time.deltaTime;

            textNumber.text =  ((int)Mathf.Lerp(startNumber, newNumber, time)).ToString();

            transform.localScale = Vector3.one * (1 + Mathf.Sin(time * Mathf.PI) * 0.2f);

            if (time >= 1)
            {
                change = false;
                textNumber.text = newNumber.ToString();
                startNumber = newNumber;
                textNumber.color = originalColor;
            }
        }
    }

    public void setNumber(int value)
    {
        startNumber = value;
        textNumber.text = value.ToString();
    }

    public void Change(int newValue)
    {
        if (startNumber == newValue)
            return;

        newNumber = newValue;

        if (newNumber > startNumber)
        {
            textNumber.color = increaseColor;
        }
        else
        {
            textNumber.color = decreaseColor;
        }

        time = 0;
        change = true;
    }
}
