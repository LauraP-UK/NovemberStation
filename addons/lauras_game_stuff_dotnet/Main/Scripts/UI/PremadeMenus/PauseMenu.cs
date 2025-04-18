using System.Linq;
using Godot;

public class PauseMenu : PreMadeMenu<BinaryChoiceForm> {
    
    protected override FormBase Build() {
        BinaryChoiceForm pauseMenu = new(GetFormName());
        pauseMenu.SetTitle("Pause Menu");
        pauseMenu.SetDescription("Do you want to quit?");
        pauseMenu.SetUpperText("Resume");
        pauseMenu.SetLowerText("Quit");
        
        pauseMenu.SetPauseGame(true);
        
        pauseMenu.SetBackgroundType(BinaryChoiceForm.BackgroundType.IMAGE);
        pauseMenu.SetBackgroundAlpha(0.5f);
        pauseMenu.SetListener(FormListener.Default(pauseMenu));
        
        pauseMenu.OnUpperButton(_ => Close());
        pauseMenu.OnLowerButton(_ => {
            Close();
            GameManager.Quit();
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