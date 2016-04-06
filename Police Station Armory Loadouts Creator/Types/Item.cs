namespace Police_Station_Armory_Loadouts_Creator.Types
{
    // System
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections.Generic;


    public enum ItemType : uint
    {
        Bulletproof_Vest,
        Nightstick            = EWeaponHash.Nightstick,
        Fire_Extinguisher     = EWeaponHash.Fire_Extinguisher,
        SMG                   = EWeaponHash.SMG,
        Assault_SMG           = EWeaponHash.Assault_SMG,
        Assault_Rifle         = EWeaponHash.Assault_Rifle,
        Carbine_Rifle         = EWeaponHash.Carbine_Rifle,
        Advanced_Rifle        = EWeaponHash.Advanced_Rifle,
        Bullpup_Rifle         = EWeaponHash.Bullpup_Rifle,
        MG                    = EWeaponHash.MG,
        Combat_MG             = EWeaponHash.Combat_MG,
        Pump_Shotgun          = EWeaponHash.Pump_Shotgun,
        Sawn_Off_Shotgun      = EWeaponHash.Sawn_Off_Shotgun,
        Assault_Shotgun       = EWeaponHash.Assault_Shotgun,
        Bullpup_Shotgun       = EWeaponHash.Bullpup_Shotgun,
        Sniper_Rifle          = EWeaponHash.Sniper_Rifle,
        Heavy_Sniper          = EWeaponHash.Heavy_Sniper,
        Pistol                = EWeaponHash.Pistol,
        Combat_Pistol         = EWeaponHash.Combat_Pistol,
        AP_Pistol             = EWeaponHash.AP_Pistol,
        Pistol_50             = EWeaponHash.Pistol_50,
        Heavy_Pistol          = EWeaponHash.Heavy_Pistol,
        Stun_Gun              = EWeaponHash.Stun_Gun,
        Micro_SMG             = EWeaponHash.Micro_SMG,
        Flare_Gun             = EWeaponHash.Flare_Gun,
        Grenade               = EWeaponHash.Grenade,
        Sticky_Bomb           = EWeaponHash.Sticky_Bomb,
        Smoke_Grenade         = EWeaponHash.Smoke_Grenade,
        Flare                 = EWeaponHash.Flare,
        Bat                   = EWeaponHash.Bat,
        Bottle                = EWeaponHash.Bottle,
        BZ_Gas                = EWeaponHash.BZ_Gas,
        Combat_PDW            = EWeaponHash.Combat_PDW,
        Flashlight            = EWeaponHash.Flashlight,
        Golf_Club             = EWeaponHash.Golf_Club,
        Grenade_Launcher      = EWeaponHash.Grenade_Launcher,
        Gusenberg             = EWeaponHash.Gusenberg,
        Hammer                = EWeaponHash.Hammer,
        Heavy_Revolver        = EWeaponHash.Heavy_Revolver,
        Heavy_Shotgun         = EWeaponHash.Heavy_Shotgun,
        Knife                 = EWeaponHash.Knife,
        Knuckle_Dusters       = EWeaponHash.Knuckle_Dusters,
        Machete               = EWeaponHash.Machete,
        Machine_Pistol        = EWeaponHash.Machine_Pistol,
        Marksman_Pistol       = EWeaponHash.Marksman_Pistol,
        Marksman_Rifle        = EWeaponHash.Marksman_Rifle,
        Minigun               = EWeaponHash.Minigun,
        Molotov               = EWeaponHash.Molotov,
        Musket                = EWeaponHash.Musket,
        Petrol_Can            = EWeaponHash.Petrol_Can,
        Proximity_Mine        = EWeaponHash.Proximity_Mine,
        Railgun               = EWeaponHash.Railgun,
        RPG                   = EWeaponHash.RPG,
        SNS_Pistol            = EWeaponHash.SNS_Pistol,
        Switchblade           = EWeaponHash.Switchblade,
        Stinger               = EWeaponHash.Stinger,
        Vintage_Pistol        = EWeaponHash.Vintage_Pistol,
        Compact_Rifle         = EWeaponHash.Compact_Rifle,
        Double_Barrel_Shotgun = EWeaponHash.Double_Barrel_Shotgun,
        Special_Carbine       = EWeaponHash.Special_Carbine,
    }


    [Serializable]
    public class Item
    {
        [XmlElement]
        public ItemType Type;
        [XmlElement]
        public WeaponComponent[] Components;
    }




    internal static class ItemTypeExtensions
    {
        private static SortedDictionary<ItemType, WeaponComponent[]> weaponComponentsDictionary;

        static ItemTypeExtensions()
        {
            #region WeaponComponentsDictionary Initialization
            weaponComponentsDictionary = new SortedDictionary<ItemType, WeaponComponent[]>();
            //weaponComponentsDictionary.Add(ItemType.Bulletproof_Vest, new WeaponComponent[0]);
            //weaponComponentsDictionary.Add(ItemType.Nightstick, new WeaponComponent[0]);
            //weaponComponentsDictionary.Add(ItemType.Fire_Extinguisher, new WeaponComponent[0]);
            weaponComponentsDictionary.Add(ItemType.SMG, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_SMG_CLIP_01,
   WeaponComponent.COMPONENT_SMG_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_MACRO_02,
   WeaponComponent.COMPONENT_AT_PI_SUPP,
   WeaponComponent.COMPONENT_SMG_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Assault_SMG, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_ASSAULTSMG_CLIP_01,
   WeaponComponent.COMPONENT_ASSAULTSMG_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_MACRO,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_ASSAULTSMG_VARMOD_LOWRIDER,
            });
            weaponComponentsDictionary.Add(ItemType.Assault_Rifle, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_ASSAULTRIFLE_CLIP_01,
   WeaponComponent.COMPONENT_ASSAULTRIFLE_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_MACRO,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
   WeaponComponent.COMPONENT_ASSAULTRIFLE_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Carbine_Rifle, new WeaponComponent[] 
            {
   WeaponComponent.COMPONENT_CARBINERIFLE_CLIP_01,
   WeaponComponent.COMPONENT_CARBINERIFLE_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_RAILCOVER_01,
   WeaponComponent.COMPONENT_AT_SCOPE_MEDIUM,
   WeaponComponent.COMPONENT_AT_AR_SUPP,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
   WeaponComponent.COMPONENT_CARBINERIFLE_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Advanced_Rifle, new WeaponComponent[] 
            {
   WeaponComponent.COMPONENT_ADVANCEDRIFLE_CLIP_01,
   WeaponComponent.COMPONENT_ADVANCEDRIFLE_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_SMALL,
   WeaponComponent.COMPONENT_AT_AR_SUPP,
   WeaponComponent.COMPONENT_ADVANCEDRIFLE_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Bullpup_Rifle, new WeaponComponent[] 
            {
   WeaponComponent.COMPONENT_BULLPUPRIFLE_CLIP_01,
   WeaponComponent.COMPONENT_BULLPUPRIFLE_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_SMALL,
   WeaponComponent.COMPONENT_AT_AR_SUPP,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
            });
            weaponComponentsDictionary.Add(ItemType.MG, new WeaponComponent[] 
            {
   WeaponComponent.COMPONENT_MG_CLIP_01,
   WeaponComponent.COMPONENT_MG_CLIP_02,
   WeaponComponent.COMPONENT_AT_SCOPE_SMALL_02,
   WeaponComponent.COMPONENT_MG_VARMOD_LOWRIDER,
            });
            weaponComponentsDictionary.Add(ItemType.Combat_MG, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_COMBATMG_CLIP_01,
   WeaponComponent.COMPONENT_COMBATMG_CLIP_02,
   WeaponComponent.COMPONENT_AT_SCOPE_MEDIUM,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
   WeaponComponent.COMPONENT_COMBATMG_VARMOD_LOWRIDER,
            });
            weaponComponentsDictionary.Add(ItemType.Pump_Shotgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_PUMPSHOTGUN_CLIP_01,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SR_SUPP,
   WeaponComponent.COMPONENT_PUMPSHOTGUN_VARMOD_LOWRIDER,
            });
            weaponComponentsDictionary.Add(ItemType.Sawn_Off_Shotgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_SAWNOFFSHOTGUN_CLIP_01,
   WeaponComponent.COMPONENT_SAWNOFFSHOTGUN_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Assault_Shotgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_ASSAULTSHOTGUN_CLIP_01,
   WeaponComponent.COMPONENT_ASSAULTSHOTGUN_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_AR_SUPP,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
            });
            weaponComponentsDictionary.Add(ItemType.Bullpup_Shotgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_BULLPUPSHOTGUN_CLIP_01,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
            });
            weaponComponentsDictionary.Add(ItemType.Sniper_Rifle, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_SNIPERRIFLE_CLIP_01,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_AT_SCOPE_LARGE,
   WeaponComponent.COMPONENT_AT_SCOPE_MAX,
   WeaponComponent.COMPONENT_SNIPERRIFLE_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Heavy_Sniper, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_HEAVYSNIPER_CLIP_01,
   WeaponComponent.COMPONENT_AT_SCOPE_LARGE,
   WeaponComponent.COMPONENT_AT_SCOPE_MAX,
            });
            weaponComponentsDictionary.Add(ItemType.Grenade_Launcher, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_GRENADELAUNCHER_CLIP_01,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
   WeaponComponent.COMPONENT_AT_SCOPE_SMALL,
            });
            weaponComponentsDictionary.Add(ItemType.RPG, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_RPG_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Stinger, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_RPG_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Minigun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_MINIGUN_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Combat_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_COMBATPISTOL_CLIP_01,
   WeaponComponent.COMPONENT_COMBATPISTOL_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_FLSH,
   WeaponComponent.COMPONENT_AT_PI_SUPP,
   WeaponComponent.COMPONENT_COMBATPISTOL_VARMOD_LOWRIDER,
            });
            weaponComponentsDictionary.Add(ItemType.AP_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_APPISTOL_CLIP_01,
   WeaponComponent.COMPONENT_APPISTOL_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_FLSH,
   WeaponComponent.COMPONENT_AT_PI_SUPP,
   WeaponComponent.COMPONENT_APPISTOL_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Pistol_50, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_PISTOL50_CLIP_01,
   WeaponComponent.COMPONENT_PISTOL50_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_FLSH,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_PISTOL50_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_PISTOL_CLIP_01,
   WeaponComponent.COMPONENT_PISTOL_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_FLSH,
   WeaponComponent.COMPONENT_AT_PI_SUPP_02,
   WeaponComponent.COMPONENT_PISTOL_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Heavy_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_HEAVYPISTOL_CLIP_01,
   WeaponComponent.COMPONENT_HEAVYPISTOL_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_FLSH,
   WeaponComponent.COMPONENT_AT_PI_SUPP,
            });
            weaponComponentsDictionary.Add(ItemType.Micro_SMG, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_MICROSMG_CLIP_01,
   WeaponComponent.COMPONENT_MICROSMG_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_MACRO,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_MICROSMG_VARMOD_LUXE,
            });
            weaponComponentsDictionary.Add(ItemType.Flare_Gun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_FLAREGUN_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Combat_PDW, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_COMBATPDW_CLIP_01,
   WeaponComponent.COMPONENT_COMBATPDW_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
   WeaponComponent.COMPONENT_AT_SCOPE_SMALL,
            });
            weaponComponentsDictionary.Add(ItemType.Flashlight, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_FLASHLIGHT_LIGHT,
            });
            weaponComponentsDictionary.Add(ItemType.Gusenberg, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_GUSENBERG_CLIP_01,
   WeaponComponent.COMPONENT_GUSENBERG_CLIP_02,
            });
            weaponComponentsDictionary.Add(ItemType.Heavy_Revolver, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_REVOLVER_CLIP_01,
   WeaponComponent.COMPONENT_REVOLVER_VARMOD_BOSS,
   WeaponComponent.COMPONENT_REVOLVER_VARMOD_GOON,
            });
            weaponComponentsDictionary.Add(ItemType.Heavy_Shotgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_HEAVYSHOTGUN_CLIP_01,
   WeaponComponent.COMPONENT_HEAVYSHOTGUN_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
            });
            weaponComponentsDictionary.Add(ItemType.Knuckle_Dusters, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_BASE,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_PIMP,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_BALLAS,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_DOLLAR,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_DIAMOND,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_HATE,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_LOVE,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_PLAYER,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_KING,
   WeaponComponent.COMPONENT_KNUCKLE_VARMOD_VAGOS,
            });
            weaponComponentsDictionary.Add(ItemType.Machine_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_MACHINEPISTOL_CLIP_01,
   WeaponComponent.COMPONENT_MACHINEPISTOL_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_SUPP,
            });
            weaponComponentsDictionary.Add(ItemType.Marksman_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_MARKSMANPISTOL_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Marksman_Rifle, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_MARKSMANRIFLE_CLIP_01,
   WeaponComponent.COMPONENT_MARKSMANRIFLE_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_LARGE_FIXED_ZOOM,
   WeaponComponent.COMPONENT_AT_AR_SUPP,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
            });
            weaponComponentsDictionary.Add(ItemType.Musket, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_MUSKET_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Railgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_RAILGUN_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.SNS_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_SNSPISTOL_CLIP_01,
   WeaponComponent.COMPONENT_SNSPISTOL_CLIP_02,
            });
            weaponComponentsDictionary.Add(ItemType.Switchblade, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_SWITCHBLADE_VARMOD_BASE,
   WeaponComponent.COMPONENT_SWITCHBLADE_VARMOD_VAR1,
   WeaponComponent.COMPONENT_SWITCHBLADE_VARMOD_VAR2,
            });
            weaponComponentsDictionary.Add(ItemType.Vintage_Pistol, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_VINTAGEPISTOL_CLIP_01,
   WeaponComponent.COMPONENT_VINTAGEPISTOL_CLIP_02,
   WeaponComponent.COMPONENT_AT_PI_SUPP,
            });
            weaponComponentsDictionary.Add(ItemType.Compact_Rifle, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_COMPACTRIFLE_CLIP_01,
   WeaponComponent.COMPONENT_COMPACTRIFLE_CLIP_02,
            });
            weaponComponentsDictionary.Add(ItemType.Double_Barrel_Shotgun, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_DBSHOTGUN_CLIP_01,
            });
            weaponComponentsDictionary.Add(ItemType.Special_Carbine, new WeaponComponent[]
            {
   WeaponComponent.COMPONENT_SPECIALCARBINE_CLIP_01,
   WeaponComponent.COMPONENT_SPECIALCARBINE_CLIP_02,
   WeaponComponent.COMPONENT_AT_AR_FLSH,
   WeaponComponent.COMPONENT_AT_SCOPE_MEDIUM,
   WeaponComponent.COMPONENT_AT_AR_SUPP_02,
   WeaponComponent.COMPONENT_AT_AR_AFGRIP,
            });
            #endregion
        }

        public static WeaponComponent[] GetAvalaibleComponents(this ItemType item)
        {
            if (weaponComponentsDictionary.ContainsKey(item))
            {
                return weaponComponentsDictionary[item];
            }
            return new WeaponComponent[0];
        }
    }
}
