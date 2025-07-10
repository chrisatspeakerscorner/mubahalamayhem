using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteLibrary", menuName = "ScriptableObjects/SpriteLibrary", order = 1)]
public class SpriteLibrary : ScriptableObject
{
    public List<Sprite> Sprites; 
}
