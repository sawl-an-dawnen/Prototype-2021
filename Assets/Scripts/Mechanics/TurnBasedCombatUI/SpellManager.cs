using System;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager: MonoBehaviour
{
    [SerializeField] Sprite[] spellIcons;
    [SerializeField] SpellSprites[] sprites;
    public enum SpellIndex {
        SLAM = 0,
        FIREBALL = 1,
        DODGE = 2,
        ELECTROCUTE = 3,
        THROWKNIFE = 4,
        STUN = 5
    }

    public SpellSprites GetRuneSprites(Rune element)
    {
        return sprites[(int)element];
    }
    
    [Serializable]
    public class SpellSprites
    {
        public Sprite regular;
        public Sprite hover;
        public Sprite selected;
        public Sprite disabled;
    }
}
