using System.Collections.Generic;
using Godot;

public static class Toast {

    public static void Info(Player viewer, string message) {
        Info(new List<Player>{viewer}, message);
    }

    public static void Info(ICollection<Player> viewers, string message) {
        Pop(viewers, message, 5000L, "res://Main/Textures/UI/Symbols/WarningIcon.png");
    }

    public static void Warn(Player viewer, string message) {
        Warn(new List<Player>{viewer}, message);
    }

    public static void Warn(ICollection<Player> viewers, string message) {
        Pop(viewers, message, 5000L, "res://Main/Textures/UI/Symbols/WarningIconUpsidedown.png");
    }

    public static void Error(Player viewer, string message) {
        Error(new List<Player>{viewer}, message);
    }

    public static void Error(ICollection<Player> viewers, string message) {
        Pop(viewers, message, 5000L, "res://Main/Textures/UI/Symbols/WarningIconUpsidedown.png", Colors.DarkRed);
    }
    
    private static void Pop(ICollection<Player> viewers, string message, long duration, string iconPath, Color color = default) {
        ToastMessage tMessage = new("ToastMessage");
        tMessage.SetText(message);
        tMessage.SetIcon(iconPath);
        if (color != default) tMessage.SetBGColour(color);
        
        foreach (Player viewer in viewers)
            viewer.GetController<PlayerController>().GetToastUI().GetForm().DisplayMessage(tMessage, duration);
    }
    
}