using UnityEngine;

public class ColorChanger
{
    public Color GetRandomColor()
    {
        return Random.ColorHSV();
    }

    public void SetColor(Color color, ref MeshRenderer renderer)
    {
        renderer.material.color = color;
    }
}