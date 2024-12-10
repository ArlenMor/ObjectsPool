using UnityEngine;

public class ColorChanger
{
    public Color GetRandomColor()
    {
        return Random.ColorHSV();
    }
}