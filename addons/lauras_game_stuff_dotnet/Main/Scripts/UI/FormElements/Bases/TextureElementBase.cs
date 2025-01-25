using System;
using Godot;

public abstract class TextureElementBase<T> : FormElement<T> where T : Control {
    
    public TextureElementBase(T element = null, Action<T> onReady = null) : base(element, onReady) { }
    public TextureElementBase(string path, Action<T> onReady = null) : base(path, onReady) { }

    public void SetTexture(Texture2D texture) {
        T element = GetElement();
        switch (element) {
            case TextureRect textureRect:
                textureRect.SetTexture(texture);
                break;
            case NinePatchRect ninePatchRect:
                ninePatchRect.SetTexture(texture);
                break;
            default:
                throw new NotSupportedException($"TextureElementBase does not support {element.GetType().Name}");
        }
    }
    
    public void SetTexture(string path) {
        Texture2D texture2D = ResourceLoader.Load<Texture2D>(path);
        SetTexture(texture2D);
    }
    
    public void SetAlpha(float alpha) {
        T element = GetElement();
        Color color = element.Modulate;
        color.A = Math.Clamp(alpha, 0.0f, 1.0f);
        switch (element) {
            case TextureRect textureRect:
                textureRect.SetModulate(color);
                return;
            case NinePatchRect ninePatchRect:
                ninePatchRect.SetModulate(color);
                break;
            default:
                throw new NotSupportedException($"TextureElementBase does not support {element.GetType().Name}");
        }
    }
}