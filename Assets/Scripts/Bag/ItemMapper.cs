using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMapper : MonoBehaviour
{
    [Header("Other")]
    public InventoryItem non;
    public InventoryItem news1;
    public InventoryItem news2;
    public InventoryItem news3;
    public InventoryItem helmet;
    public InventoryItem boots;
    public InventoryItem axe;
    public InventoryItem sword;
    public InventoryItem crowbar;

    [Header("Head")]
    public InventoryItem SimpleHelmet;
    public InventoryItem AdvancedHelmet;
    public InventoryItem EmeraldCrown;
    public InventoryItem SapphireCrown;
    public InventoryItem RubyCrown;
    public InventoryItem AmethystCrown;

    [Header("Body")]
    public InventoryItem BasicArmor;
    public InventoryItem AdvancedArmor;
    public InventoryItem MageRobe;
    public InventoryItem ShadowRobe;
    public InventoryItem VampireCloak;
    public InventoryItem TrimmedRobe;
    public InventoryItem FairyRobe;

    [Header("Belt")]
    public InventoryItem ClothBelt;
    public InventoryItem GreenBelt;
    public InventoryItem TricolorBelt;
    public InventoryItem AquaBelt;
    public InventoryItem AbyssBelt;

    [Header("Relic")]
    public InventoryItem SilverChalice;
    public InventoryItem GoldenGrail;
    public InventoryItem SapphireBox;
    public InventoryItem RubyPedestal;
    public InventoryItem DevilHorn;
    public InventoryItem DragonEgg;

    [Header("Main Hand")]
    public InventoryItem Broomstick;
    public InventoryItem CrimsonStaff;
    public InventoryItem VineStaff;
    public InventoryItem TwistedStaff;
    public InventoryItem AuraStaff;

    [Header("Off Hand")]
    public InventoryItem WaterSpellbook;
    public InventoryItem ThunderSpellbook;
    public InventoryItem EarthSpellbook;
    public InventoryItem FireSpellbook;

    [Header("Neck")]
    public InventoryItem TigerFangNecklace;
    public InventoryItem EmeraldNecklace;
    public InventoryItem SapphireNecklace;
    public InventoryItem LavaNecklace;
    public InventoryItem DemonNecklace;

    [Header("Ring")]
    public InventoryItem SilverRing;
    public InventoryItem GoldenRing;
    public InventoryItem DragonRing;
    public InventoryItem MeowMeowRing;
    public InventoryItem DemonRing;

    public InventoryItem GetInventory(string itemName)
    {
        switch (itemName)
        {
            case "News1":
                return news1;
            case "News2":
                return news2;
            case "News3":
                return news3;
            case "Crowbar":
                return crowbar;
            case "Helmet":
                return helmet;
            case "Boots":
                return boots;
            case "Axe":
                return axe;
            case "Sword":
                return sword;
            case "Simple Helmet":
                return SimpleHelmet;

            case "Advanced Helmet":
                return AdvancedHelmet;

            case "Emerald Crown":
                return EmeraldCrown;

            case "Sapphire Crown":
                return SapphireCrown;

            case "Ruby Crown":
                return RubyCrown;

            case "Amethyst Crown":
                return AmethystCrown;

            case "Basic Armor":
                return BasicArmor;

            case "Advanced Armor":
                return AdvancedArmor;

            case "Mage Robe":
                return MageRobe;

            case "Shadow Robe":
                return ShadowRobe;

            case "Vampire Cloak":
                return VampireCloak;

            case "Trimmed Robe":
                return TrimmedRobe;

            case "Fairy Robe":
                return FairyRobe;

            case "Cloth Belt":
                return ClothBelt;

            case "Green Belt":
                return GreenBelt;

            case "Tricolor Belt":
                return TricolorBelt;

            case "Aqua Belt":
                return AquaBelt;

            case "Abyss Belt":
                return AbyssBelt;

            case "Silver Chalice":
                return SilverChalice;

            case "Golden Grail":
                return GoldenGrail;

            case "Sapphire Box":
                return SapphireBox;

            case "Ruby Pedestal":
                return RubyPedestal;

            case "Devil Horn":
                return DevilHorn;

            case "Dragon Egg":
                return DragonEgg;

            case "Broomstick":
                return Broomstick;

            case "Crimson Staff":
                return CrimsonStaff;

            case "Vine Staff":
                return VineStaff;

            case "Twisted Staff":
                return TwistedStaff;

            case "Aura Staff":
                return AuraStaff;

            case "Water Spellbook":
                return WaterSpellbook;

            case "Thunder Spellbook":
                return ThunderSpellbook;

            case "Earth Spellbook":
                return EarthSpellbook;

            case "Fire Spellbook":
                return FireSpellbook;

            case "Tiger Fang Necklace":
                return TigerFangNecklace;

            case "Emerald Necklace":
                return EmeraldNecklace;

            case "Sapphire Necklace":
                return SapphireNecklace;

            case "Lava Necklace":
                return LavaNecklace;

            case "Demon Necklace":
                return DemonNecklace;

            case "Silver Ring":
                return SilverRing;

            case "Golden Ring":
                return GoldenRing;

            case "Dragon Ring":
                return DragonRing;

            case "Meow Meow Ring":
                return MeowMeowRing;

            case "Demon Ring":
                return DemonRing;
            default:
                Debug.LogError("Unrecognized item name: " + itemName);
                return non;
        }
    }
}
