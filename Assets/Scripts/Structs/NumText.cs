/*using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NumText //A custom class that stores a nummeric value and a text to display it as soon as it is changed. Not implemented as per current version.
{
    public Text text;

    private float num;
    public float Num
    {
        get { return num; }
        set
        {
            if (num != value)
            {
                num = value;
                UpdateText(num);
            }
        }
    }

    void UpdateText(float value)
    {
        text.text = Mathf.RoundToInt(value).ToString();
    }
}*/