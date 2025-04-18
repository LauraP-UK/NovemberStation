﻿using System;
using System.Collections.Generic;
using Godot;

public interface IInventory {
    public string GetName();
    public AddItemFailCause AddItem(IObjectBase item);
    public AddItemFailCause AddItem(Node3D node);
    public AddItemFailCause AddItem(string objectMetaTag, string jsonData);
    public AddItemFailCause CanAddItem(IObjectBase item);
    public AddItemFailCause CanAddItem(string jsonData);
    public void RemoveItem(string objectMetaTag, string jsonData);
    public bool UpdateItem(string updatedJson);
    public int CountItemType(string objectMetaTag);
    public bool HasItem(string objectMetaTag);
    public bool HasItem(ItemType itemType);
    public bool IsEmpty();
    public List<(string,Guid)> GetContents();
    public List<(string,Guid)> GetContentsOfType(string type);
    public List<(string,Guid)> GetContentsOfType(ItemType type);
    public string GetViaGUID(Guid id);
    public void ClearContents();
    public T GetAs<T>() where T : IInventory;
    public string Serialise();
    public void Deserialise(string json);
    public Dictionary<string, string> SerialiseToDict();
    public void DeserialiseFromDict(Dictionary<string, string> data);
}