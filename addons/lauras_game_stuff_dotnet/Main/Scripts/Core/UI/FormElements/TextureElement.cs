
using System;
using Godot;

public class TextureElement : FormElement<TextureRect> {

    public TextureElement(TextureRect element = null) : base(element) { }
    public TextureElement(string path, Action<TextureRect> initialiser = null) : base(path, initialiser) {}
    
    public void SetTexture(Texture2D texture) => GetElement().Texture = texture;
}