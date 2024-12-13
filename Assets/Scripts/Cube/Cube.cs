using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    [SerializeField] private LayerMask _platformLayerMask;

    private ColorChanger _colorChanger = new ColorChanger();
    private MeshRenderer _renderer;

    public event Action<Cube> OnCubeCollision;

    public bool IsReleased { get; set; }
    public Color DefaultColor { get; private set; }
    public bool IsDefaultColor { get; private set; }

    private void Awake()
    {
        DefaultColor = Color.white;
        IsDefaultColor = true;

        _renderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if((1 << collision.gameObject.layer) == _platformLayerMask)
        {
            if(IsDefaultColor == true)
            {
                _renderer.material.color = _colorChanger.GetRandomColor();
                IsDefaultColor = false;
            }

            OnCubeCollision?.Invoke(this);
        }
    }

    public void SetDefaultColor()
    {
        _colorChanger.SetColor(DefaultColor, ref _renderer);
        IsDefaultColor = true;
    }
}