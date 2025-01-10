using System;

public interface IButtonElement {
    public void SetOnPressed(Action action);
    public void SetOnButtonDown(Action action);
    public void SetOnButtonUp(Action action);
    public void SetOnToggled(Action<object[]> action);
}