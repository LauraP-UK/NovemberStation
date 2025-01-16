
using System;
using System.Collections.Generic;
using Godot;

public class ScrollDisplayList : FormBase {
    
    private readonly VBoxContainerLayout _displayList;
    private readonly ScrollContainerLayout _scrollContainer;
    private Action<IFormObject> _onSelectElement;
    private Action<Key, ScrollDisplayList> _keyboardBehaviour;
    private bool _keyboardEnabled = false;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/HybridLayouts/ScrollDisplayList.tscn",
        DISPLAY_LIST = "ScrollContainer/DisplayList",
        SCROLL_CONTAINER = "ScrollContainer";

    public ScrollDisplayList(string formName, Action<Key, ScrollDisplayList> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        VBoxContainer displayList = FindNode<VBoxContainer>(DISPLAY_LIST);
        ScrollContainer scrollContainer = FindNode<ScrollContainer>(SCROLL_CONTAINER);
        
        _keyboardBehaviour = keyboardBehaviour;
        
        _displayList = new VBoxContainerLayout(displayList);
        _scrollContainer = new ScrollContainerLayout(scrollContainer);
        
        _menuLayout = new ControlLayout(_menu, _ => {
            foreach (IFormObject element in getAllElements()) {
                switch (element) {
                    case IFormElement formElement: {
                        formElement.SetTopLevelLayout(_menuLayout);
                        break;
                    }
                    case ILayoutElement layoutElement: {
                        layoutElement.SetTopLevelLayout(_menuLayout);
                        layoutElement.Build(_menuLayout, new HashSet<ILayoutElement>(), false);
                        break;
                    }
                }
            }
            
            foreach (IFormObject listObject in GetDisplayObjects()) {
                switch (listObject) {
                    case IFormElement formElement:
                        formElement.SetTopLevelLayout(_menuLayout);
                        break;
                    case ILayoutElement layoutElement:
                        layoutElement.SetTopLevelLayout(_menuLayout);
                        layoutElement.Build(_menuLayout, new HashSet<ILayoutElement>(), false);
                        break;
                    case FormBase form:
                        form.GetTopLevelLayout().Build(_menuLayout, new HashSet<ILayoutElement>(), false);
                        break;
                }
            }
        });
        _menuLayout.Build();
    }

    protected override List<IFormObject> getAllElements() => new() { _displayList, _scrollContainer };
    
    public VBoxContainerLayout GetDisplayList() => _displayList;
    public ScrollContainerLayout GetScrollContainer() => _scrollContainer;
    public List<IFormObject> GetDisplayObjects() => _displayList.GetDisplayObjects();
    public void AddElement(IFormObject element) => _displayList.AddChild(element);
    public void SetOnSelectElement<T>(Action<T> onSelectElement) where T : IFormObject => _onSelectElement = obj => onSelectElement((T) obj);

    public Action<IFormObject> GetOnSelectElement() => _onSelectElement;
    
    public void SetFollowFocus(bool value) => GetScrollContainer().GetContainer().SetFollowFocus(value);
    
    public void SetKeyboardEnabled(bool value) => _keyboardEnabled = value;
    public bool IsKeyboardEnabled() => _keyboardEnabled;
    
    private void MoveFocus(int steps) {
        List<IFormObject> listObjects = GetDisplayObjects();
        if (listObjects == null || listObjects.Count == 0) return;
        
        int currentIndex = listObjects.FindIndex(obj => obj.GetNode().HasFocus());
        int originalIndex = currentIndex;

        do {
            currentIndex = (currentIndex + steps + listObjects.Count) % listObjects.Count;
            if (currentIndex == originalIndex) return;
        } while (listObjects[currentIndex] is not IFocusable focusObj);

        ((IFocusable) listObjects[currentIndex]).GrabFocus();
    }
    public IFormObject GetFocusedElement() => GetDisplayObjects().Find(obj => obj.GetNode().HasFocus());
    public IFormObject FocusElement(int index) {
        List<IFormObject> listObjects = GetDisplayObjects();
        if (listObjects == null || listObjects.Count == 0) {
            GD.PrintErr("ERROR: ScrollDisplayList.FocusElement() : No elements to focus.");
            return null;
        }
        if (index < 0 || index >= listObjects.Count)
            GD.PrintErr($"ERROR: ScrollDisplayList.FocusElement() : Index {index} out of bounds, clamping to 0 to {listObjects.Count - 1}.");
        index = Mathf.Clamp(index, 0, listObjects.Count - 1);
        IFormObject toFocus = listObjects[index];
        toFocus.GetNode().GrabFocus();
        return toFocus;
    }
    
    public void SetKeyboardBehaviour(Action<Key, ScrollDisplayList> keyboardBehaviour) => _keyboardBehaviour = keyboardBehaviour;
    public static void DefaultKeyboardBehaviour(Key key, ScrollDisplayList form) {
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
                form._onSelectElement.Invoke(focusedElement);
                break;
            }
        }
    }
    protected override void KeyboardBehaviour(Key key) {
        if (!IsKeyboardEnabled()) return;
        if (_keyboardBehaviour != null) {
            _keyboardBehaviour(key, this);
            return;
        }
        DefaultKeyboardBehaviour(key, this);
    }
}