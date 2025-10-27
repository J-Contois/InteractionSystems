using UnityEngine;

public abstract class BaseToggleComponent : MonoBehaviour, IToggle
{
    private bool _state = false;
    
    public void Activate()
    {
        _state = true;
        ActivateComponent();
    }
    
    public void Deactivate()
    {
        _state = false;
        DeactivateComponent();
    }

    protected abstract void ActivateComponent();
    
    protected abstract void DeactivateComponent();
    
    public void Switch()
    {
        if (_state)
            Deactivate();
        else
            Activate();
    }
}
