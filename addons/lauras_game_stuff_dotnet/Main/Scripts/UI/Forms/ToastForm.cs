using System;
using System.Collections.Generic;
using Godot;

public class ToastForm : FormBase, IProcess {
    private readonly VBoxContainerElement _display;

    private readonly SmartDictionary<ToastMessage, (ulong startTime, long duration)> _messages = new();
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ToastScreen.tscn",
        VBOX_CONTAINER = "Display";

    public ToastForm(string formName) : base(formName, FORM_PATH) {
        VBoxContainer container = FindNode<VBoxContainer>(VBOX_CONTAINER);
        SetTopLevelLayout(this);
        _display = new VBoxContainerElement(container);
        _menuElement = new ControlElement(_menu);
    }

    protected override List<IFormObject> GetAllElements() => new() { _display };
    protected override void OnDestroy() {
        foreach (IFormObject displayObject in _display.GetDisplayObjects())
            ((ToastMessage) displayObject).Destroy();
    }

    public override bool LockMovement() => false;

    public void Process(float delta) {
        ulong currentMillis = Time.GetTicksMsec();
        _messages.RemoveWhere(entry => {
            ToastMessage message = entry.Key;
            ulong start = entry.Value.startTime;
            long duration = entry.Value.duration;

            if (start + (ulong)duration >= currentMillis) {
                long remaining = (long)start + duration - (long)currentMillis;
                float alpha = Mathsf.Remap(500, 0, remaining, 1f, 0f);
                if (Math.Abs(alpha - 1.0f) > 0.01f) message.SetAlpha(alpha);
                return false;
            }
            _display.RemoveChild(message);
            return true;
        });
    }

    public void DisplayMessage(ToastMessage message, long duration) {
        _messages.Add(message, (Time.GetTicksMsec(), duration));
        _display.AddChild(message, 0);
    }
}