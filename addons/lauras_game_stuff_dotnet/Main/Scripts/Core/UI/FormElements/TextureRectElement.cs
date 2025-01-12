
using System;
using Godot;

public class TextureRectElement : TextureElementBase<TextureRect> {

    public TextureRectElement(TextureRect element = null, Action<TextureRect> onReady = null) : base(element, onReady) { }
    public TextureRectElement(string path, Action<TextureRect> onReady = null) : base(path, onReady) {}
    
    public override void SetTexture(Texture2D texture) => GetElement().Texture = texture;
}