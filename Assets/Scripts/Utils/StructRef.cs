using System;
using UnityEngine;

[Serializable]
public class StructRef<T>
{
    public T value;
}

[Serializable]
public class ColorRef : StructRef<Color> { }