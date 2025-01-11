
using System;
using Godot;

public class TextureElement : FormElement<TextureRect> {

    public TextureElement(TextureRect element = null, Action<TextureRect> onReady = null) : base(element, onReady) { }
    public TextureElement(string path, Action<TextureRect> onReady = null) : base(path, onReady) {}
    
    public void SetTexture(Texture2D texture) => GetElement().Texture = texture;
}