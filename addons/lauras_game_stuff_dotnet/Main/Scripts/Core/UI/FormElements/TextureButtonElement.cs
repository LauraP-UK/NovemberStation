
using Godot;

public class TextureButtonElement : ButtonElementBase<TextureButton> {
    public TextureButtonElement(TextureButton element = null) => SetElement(element);
    public void SetTextureHover(Texture2D texture) => GetElement().SetTextureHover(texture);
    public void SetTextureNormal(Texture2D texture) => GetElement().SetTextureNormal(texture);
    public void SetTexturePressed(Texture2D texture) => GetElement().SetTexturePressed(texture);
    public void SetTextureFocused(Texture2D texture) => GetElement().SetTextureFocused(texture);
    public void SetTextureDisabled(Texture2D texture) => GetElement().SetTextureDisabled(texture);
    
    public void LoadTextureHover(string path) {
        Texture2D loadTexture = LoadTexture(path);
        if (loadTexture == null) {
            GD.PrintErr($"ERROR: TextureButtonElement.LoadTextureHover() : Failed to load texture at path '{path}'");
            return;
        }
        SetTextureHover(loadTexture);
    }
    
    public void LoadTextureNormal(string path) {
        Texture2D loadTexture = LoadTexture(path);
        if (loadTexture == null) {
            GD.PrintErr($"ERROR: TextureButtonElement.LoadTextureNormal() : Failed to load texture at path '{path}'");
            return;
        }
        SetTextureNormal(loadTexture);
    }
    
    public void LoadTexturePressed(string path) {
        Texture2D loadTexture = LoadTexture(path);
        if (loadTexture == null) {
            GD.PrintErr($"ERROR: TextureButtonElement.LoadTexturePressed() : Failed to load texture at path '{path}'");
            return;
        }
        SetTexturePressed(loadTexture);
    }
    
    public void LoadTextureFocused(string path) {
        Texture2D loadTexture = LoadTexture(path);
        if (loadTexture == null) {
            GD.PrintErr($"ERROR: TextureButtonElement.LoadTextureFocused() : Failed to load texture at path '{path}'");
            return;
        }
        SetTextureFocused(loadTexture);
    }
    
    public void LoadTextureDisabled(string path) {
        Texture2D loadTexture = LoadTexture(path);
        if (loadTexture == null) {
            GD.PrintErr($"ERROR: TextureButtonElement.LoadTextureDisabled() : Failed to load texture at path '{path}'");
            return;
        }
        SetTextureDisabled(loadTexture);
    }
    
    private Texture2D LoadTexture(string path) {
        // Load the texture from the path
        return null; // Replace this with the actual loading code
    }
}