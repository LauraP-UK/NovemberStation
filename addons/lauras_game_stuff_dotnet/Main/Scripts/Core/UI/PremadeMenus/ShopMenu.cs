
using Godot;
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI;
using NovemberStation.Main.Scripts.Items;

public class ShopMenu : PreMadeMenu {
    protected override FormBase Build() {
        GameManager gameManager = GameManager.I();

        TestDisplayForm testDisplayForm = new(GetFormName());
        testDisplayForm.GetScrollDisplay().SetKeyboardEnabled(true);
        testDisplayForm.GetScrollDisplay().SetOnSelectElement<ShopItemDisplayButton>(elem => elem.GetButton().ForcePressed());
        testDisplayForm.SetOnReady(form => {
            Items.GetItemButtons().ForEach(btn => {
                btn.OnPressed(btn => {
                    ItemType itemType = btn.GetItemType();
                    RigidBody3D rigidBody3D = itemType.CreateInstance();
                    gameManager.GetSceneObjects().AddChild(rigidBody3D);
                    Vector3 spawn = Raycast.Trace(gameManager.GetPlayer(), 2.0f).GetEnd();
                    rigidBody3D.SetPosition(spawn);
                    rigidBody3D.SetRotation(gameManager.GetPlayer().GetModel().GetRotation());
                    
                    UIManager.CloseMenu(GetFormName());
                });
                btn.SetTopLevelLayout(form.GetTopLevelLayout());
                form.GetScrollDisplay().AddElement(btn);
            });
            ShopItemDisplayButton closeButton = Items.GetCloseButton();
            closeButton.SetTopLevelLayout(form.GetTopLevelLayout());
            closeButton.OnPressed(btn => UIManager.CloseMenu(GetFormName()));
            form.GetScrollDisplay().AddElement(closeButton);
        });
        testDisplayForm.GetScrollDisplay().SetKeyboardBehaviour((pressedKey, form, isPressed) => {
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
                    UIManager.CloseMenu(GetFormName());
                    break;
                }
            }
        });
        
        return testDisplayForm;
    }

    protected override string GetFormName() => "TestMenu";
}