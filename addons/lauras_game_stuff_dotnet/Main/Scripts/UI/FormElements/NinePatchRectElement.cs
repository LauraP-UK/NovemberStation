using System;
using Godot;

public class NinePatchRectElement : TextureElementBase<NinePatchRect> {
    
    public NinePatchRectElement(NinePatchRect element = null, Action<NinePatchRect> onReady = null) : base(element, onReady) { }
    public NinePatchRectElement(string path, Action<NinePatchRect> onReady = null) : base(path, onReady) { }
    
    public void SetDrawCenter(bool value) => GetElement().DrawCenter = value;
    
    public void SetRegionRect(Rect2 rect) => GetElement().RegionRect = rect;
    public void SetRegionRect(float x, float y, float width, float height) => GetElement().RegionRect = new Rect2(x, y, width, height);
    public void SetRegionToTexture() {
        if (GetElement().Texture == null) {
            GD.PrintErr("WARN: NinePatchRectElement.SetRegionToTexture() : Texture is null!");
            return;
        }
        GetElement().RegionRect = new Rect2(0, 0, GetElement().Texture.GetWidth(), GetElement().Texture.GetHeight());
    }

    public void SetPatchMarginLeft(int value) => GetElement().PatchMarginLeft = value;
    public void SetPatchMarginTop(int value) => GetElement().PatchMarginTop = value;
    public void SetPatchMarginRight(int value) => GetElement().PatchMarginRight = value;
    public void SetPatchMarginBottom(int value) => GetElement().PatchMarginBottom = value;
    public void SetPatchMargins(int value) => SetPatchMargins(value, value, value, value);
    public void SetPatchMargins(int left, int top, int right, int bottom) {
        SetPatchMarginLeft(left);
        SetPatchMarginTop(top);
        SetPatchMarginRight(right);
        SetPatchMarginBottom(bottom);
    }
    
    public void SetHStretchMode(NinePatchRect.AxisStretchMode mode) => GetElement().AxisStretchHorizontal = mode;
    public void SetVStretchMode(NinePatchRect.AxisStretchMode mode) => GetElement().AxisStretchVertical = mode;
}