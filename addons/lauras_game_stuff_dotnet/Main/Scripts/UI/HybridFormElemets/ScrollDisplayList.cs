using System;
using System.Collections.Generic;
using Godot;

public class ScrollDisplayList : FormBase {
    
    private readonly VBoxContainerElement _displayList;
    private readonly ScrollContainerElement _scrollContainer;
    private Action<IFormObject> _onSelectElement;
    private Action<Key, ScrollDisplayList, bool> _keyboardBehaviour;
    private bool _keyboardEnabled = false;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/HybridLayouts/ScrollDisplayList.tscn",
        DISPLAY_LIST = "ScrollContainer/DisplayList",
        SCROLL_CONTAINER = "ScrollContainer";

    public ScrollDisplayList(string formName, Action<Key, ScrollDisplayList, bool> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        VBoxContainer displayList = FindNode<VBoxContainer>(DISPLAY_LIST);
        ScrollContainer scrollContainer = FindNode<ScrollContainer>(SCROLL_CONTAINER);
        
        _keyboardBehaviour = keyboardBehaviour;
        
        _displayList = new VBoxContainerElement(displayList);
        _scrollContainer = new ScrollContainerElement(scrollContainer);
        
        _menuElement = new ControlElement(_menu, _ => {
            foreach (IFormObject element in GetAllElements()) element.SetTopLevelLayout(GetTopLevelLayout());
            foreach (IFormObject listObject in GetDisplayObjects()) listObject.SetTopLevelLayout(GetTopLevelLayout());
        });
    }

    protected override List<IFormObject> GetAllElements() => new() { _displayList, _scrollContainer };
    
    public VBoxContainerElement GetDisplayList() => _displayList;
    public ScrollContainerElement GetScrollContainer() => _scrollContainer;
    public List<IFormObject> GetDisplayObjects() => _displayList.GetDisplayObjects();
    public void AddElement(IFormObject element) {
        _displayList.AddChild(element);
        element.SetTopLevelLayout(GetTopLevelLayout());
    }

    public void SetOnSelectElement<T>(Action<T> onSelectElement) where T : IFormObject => _onSelectElement = obj => onSelectElement((T) obj);

    public Action<IFormObject> GetOnSelectElement() => _onSelectElement;
    
    public void SetFollowFocus(bool value) => GetScrollContainer().GetElement().SetFollowFocus(value);
    
    public void SetKeyboardEnabled(bool value) => _keyboardEnabled = value;
    public bool IsKeyboardEnabled() => _keyboardEnabled;

    protected override void OnDestroy() {
        foreach (IFormObject displayObject in GetDisplayObjects()) {
            if (displayObject is FormBase form) {
                form.Destroy();
                form.GetMenu().QueueFree();
            }
            else displayObject.GetNode().QueueFree();
        }
    }

    public void MoveFocus(int steps) {
        if (!IsValid()) return;
        List<IFormObject> listObjects = GetDisplayObjects();
        if (listObjects == null || listObjects.Count == 0) return;
        
        int currentIndex = listObjects.FindIndex(obj => obj is IFocusable focusable && focusable.HasFocus());
        
        int originalIndex = currentIndex;

        do {
            currentIndex = (currentIndex + steps + listObjects.Count) % listObjects.Count;
            if (currentIndex == originalIndex) return;
        } while (listObjects[currentIndex] is not IFocusable);
        
        ((IFocusable) listObjects[currentIndex]).GrabFocus();
    }
    public IFormObject GetFocusedElement() {
        return GetDisplayObjects().Find(obj => obj is IFocusable focusable && focusable.HasFocus());
    }

    public IFormObject FocusElement(int index) {
        if (!IsValid())
            return null;
        List<IFormObject> listObjects = GetDisplayObjects();
        if (listObjects == null || listObjects.Count == 0) {
            GD.PrintErr("ERROR: ScrollDisplayList.FocusElement() : No elements to focus.");
            return null;
        }

        if (index < 0 || index >= listObjects.Count) {
            GD.PrintErr($"ERROR: ScrollDisplayList.FocusElement() : Index {index} out of bounds, clamping to 0 to {listObjects.Count - 1}.");
            index = Mathf.Clamp(index, 0, listObjects.Count - 1);
        }
        
        IFormObject toFocus = listObjects[index];

        if (toFocus is IFocusable focusable)
            focusable.GrabFocus();
        else {
            GD.PrintErr($"ERROR: ScrollDisplayList.FocusElement() : Element at index {index} does not implement IFocusable.");
            return null;
        }
        
        return toFocus;
    }
    
    public void SetKeyboardBehaviour(Action<Key, ScrollDisplayList, bool> keyboardBehaviour) => _keyboardBehaviour = keyboardBehaviour;
    public static void DefaultKeyboardBehaviour(Key key, ScrollDisplayList form, bool isPressed) {
        if (!isPressed) return;
        switch (key) {
            case Key.W: {
                form.MoveFocus(-1);
                break;
            }
            case Key.S: {
                form.MoveFocus(1);
                break;
            }
            case Key.Space: {
                if (form._onSelectElement == null) {
                    GD.PrintErr("ERROR: ScrollDisplayList.DefaultKeyboardBehaviour() : No onSelectElement action set.");
                    break;
                }
                IFormObject focusedElement = form.GetFocusedElement() ?? form.FocusElement(0);
                if (focusedElement != null) form._onSelectElement.Invoke(focusedElement);
                break;
            }
        }
    }
    public override void KeyboardBehaviour(Key key, bool isPressed) {
        if (!IsKeyboardEnabled()) return;
        if (_keyboardBehaviour != null) {
            _keyboardBehaviour(key, this, isPressed);
            return;
        }
        DefaultKeyboardBehaviour(key, this, isPressed);
    }
}