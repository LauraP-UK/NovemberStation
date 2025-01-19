using System.Linq;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.PremadeMenus;

public class PauseMenu : PreMadeMenu {
    
    protected override FormBase Build() {
        BinaryChoiceForm pauseMenu = new(GetFormName());
        pauseMenu.SetTitle("Pause Menu");
        pauseMenu.SetDescription("Do you want to quit?");
        pauseMenu.SetUpperText("Resume");
        pauseMenu.SetLowerText("Quit");
        
        pauseMenu.SetBackgroundType(BinaryChoiceForm.BackgroundType.IMAGE);
        pauseMenu.SetBackgroundAlpha(0.5f);
        
        pauseMenu.OnUpperButton(_ => UIManager.CloseMenu(GetFormName()));
        pauseMenu.OnLowerButton(_ => {
            UIManager.CloseMenu(GetFormName());
            GameManager.I().Quit();
        });
        
        pauseMenu.SetKeyboardBehaviour((key, form, isPressed) => {
            if (!isPressed) return;
            switch (key) {
                case Key.W: {
                    form.GetUpperButton().GetElement().GrabFocus();
                    return;
                }
                case Key.S: {
                    form.GetLowerButton().GetElement().GrabFocus();
                    return;
                }
                case Key.Space: {
                    foreach (ButtonElement button in form.GetButtons().Where(button => button.GetElement().HasFocus())) {
                        button.ForcePressed();
                        return;
                    }
                    break;
                }
                case Key.Escape: {
                    form.GetUpperButton().ForcePressed();
                    return;
                }
            }
        });
        
        return pauseMenu;
    }

    protected override string GetFormName() => "PauseMenu";
}