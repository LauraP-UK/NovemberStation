using Godot;

public class ShopMenu : PreMadeMenu<TestDisplayForm> {
    protected override FormBase Build() {
        GameManager gameManager = GameManager.I();

        TestDisplayForm testDisplayForm = new(GetFormName());
        ScrollDisplayList scrollDisplay = testDisplayForm.GetScrollDisplay();
        scrollDisplay.SetKeyboardEnabled(true);
        scrollDisplay.SetOnSelectElement<ShopItemDisplayButton>(elem => elem.GetButton().ForcePressed());
        testDisplayForm.SetOnReady(form => {
            Items.GetItemButtons().ForEach(btn => {
                btn.OnPressed(btn => {
                    ItemType itemType = btn.GetItemType();
                    RigidBody3D rigidBody3D = itemType.CreateInstance();
                    gameManager.GetSceneObjects().AddChild(rigidBody3D);
                    Vector3 spawn = Raycast.Trace(gameManager.GetPlayer(), 2.0f).GetEnd();
                    rigidBody3D.SetPosition(spawn);
                    rigidBody3D.SetRotation(gameManager.GetPlayer().GetModel().GetRotation());
                    
                    GameManager.I().RegisterObject(rigidBody3D);
                    
                    Close();
                });
                btn.SetTopLevelLayout(form.GetTopLevelLayout());
                form.GetScrollDisplay().AddElement(btn);
            });
            ShopItemDisplayButton closeButton = Items.GetCloseButton();
            closeButton.SetTopLevelLayout(form.GetTopLevelLayout());
            closeButton.OnPressed(_ => Close());
            form.GetScrollDisplay().AddElement(closeButton);
        });
        scrollDisplay.SetKeyboardBehaviour((pressedKey, form, isPressed) => {
            switch (pressedKey) {
                case Key.W: {
                    if (!isPressed) return;
                    form.MoveFocus(-1);
                    break;
                }
                case Key.S: {
                    if (!isPressed) return;
                    form.MoveFocus(1);
                    break;
                }
                case Key.Space: {
                    if (isPressed) {
                        if (form.GetOnSelectElement() == null) {
                            GD.PrintErr("ERROR: ScrollDisplayList.DefaultKeyboardBehaviour() : No onSelectElement action set.");
                            break;
                        }

                        IFormObject focusedElement = form.GetFocusedElement() ?? form.FocusElement(0);
                        if (focusedElement != null) {
                            form.GetOnSelectElement().Invoke(focusedElement);
                            ((ShopItemDisplayButton) focusedElement).VisualPress(true);
                        }
                    }
                    else {
                        IFormObject focusedElement = form.GetFocusedElement();
                        ((ShopItemDisplayButton)focusedElement)?.VisualPress(false);
                    }
                    break;
                }
                case Key.Escape: {
                    Close();
                    break;
                }
            }
        });
        
        return testDisplayForm;
    }

    protected override string GetFormName() => "TestMenu";
}