using System;

public readonly struct ValueChangedEvent<T>
{
    public readonly T OldValue;
    public readonly T NewValue;

    public ValueChangedEvent(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

public class Bindable<T>
{
    public Bindable(T value)
    {
        this.value = value;
    }

    public event Action<ValueChangedEvent<T>> ValueChanged = delegate { };

    public void BindValueChanged(Action<ValueChangedEvent<T>> action, bool triggerImmediately = false)
    {
        ValueChanged = action;
        if (triggerImmediately) action.Invoke(new ValueChangedEvent<T>(value, value));
    }

    public void TriggerChange()
    {
        ValueChanged.Invoke(new ValueChangedEvent<T>(value, value));
    }

    private T value;

    public T Value
    {
        get => value;
        set
        {
            if (!Equals(this.value, value))
            {
                var valueChangedEvent = new ValueChangedEvent<T>(this.value, value);
                this.value = value;
                ValueChanged.Invoke(valueChangedEvent);
            }
        }
    }

    public void SetSilent(T value) => this.value = value;
}
