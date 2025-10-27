using UnityEngine;

public class BaseToggleSetter : MonoBehaviour, IToggle
{
    [SerializeField] private BaseToggleComponent _toggleComponent = null;
    
    public void Activate()
    {
        _toggleComponent.Activate();
    }
    
    public void Deactivate()
    {
        _toggleComponent.Deactivate();
    }
    
    public void Switch()
    {
        _toggleComponent.Switch();
    }
}
