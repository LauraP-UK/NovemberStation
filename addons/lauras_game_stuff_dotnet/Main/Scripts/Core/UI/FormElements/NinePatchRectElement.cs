
using System;
using Godot;

public class NinePatchRectElement : TextureElementBase<NinePatchRect> {
    
    public NinePatchRectElement(NinePatchRect element = null, Action<NinePatchRect> onReady = null) : base(element, onReady) { }
    public NinePatchRectElement(string path, Action<NinePatchRect> onReady = null) : base(path, onReady) { }

    public override void SetTexture(Texture2D texture) {
        throw new NotImplementedException();
    }
}