using System;

public class Bindable<T>
{
    public Bindable(T value)
    {
        _value = value;
    }

    public event Action<T> ValueChanged = _ => { };

    public void BindValueChanged(Action<T> action, bool triggerImmediately = false)
    {
        ValueChanged = action;
        if (triggerImmediately) action.Invoke(_value);
    }

    public void TriggerChange()
    {
        ValueChanged.Invoke(_value);
    }

    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                ValueChanged.Invoke(value);
            }
        }
    }
}
