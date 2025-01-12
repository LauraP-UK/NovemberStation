
using System;
using Godot;

public abstract class TextureElementBase<T> : FormElement<T> where T : Control {
    
    public TextureElementBase(T element = null, Action<T> onReady = null) : base(element, onReady) { }
    public TextureElementBase(string path, Action<T> onReady = null) : base(path, onReady) { }
    
    public abstract void SetTexture(Texture2D texture);
}