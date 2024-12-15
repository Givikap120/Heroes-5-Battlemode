using System;

public class Bindable<T>
{
    public Bindable(T value)
    {
        this.value = value;
    }

    public event Action<T> ValueChanged = _ => { };

    public void BindValueChanged(Action<T> action, bool triggerImmediately = false)
    {
        ValueChanged = action;
        if (triggerImmediately) action.Invoke(value);
    }

    public void TriggerChange()
    {
        ValueChanged.Invoke(value);
    }

    private T value;

    public T Value
    {
        get => value;
        set
        {
            if (!Equals(this.value, value))
            {
                this.value = value;
                ValueChanged.Invoke(value);
            }
        }
    }
}
