namespace Police_Station_Armory_Loadouts_Creator.Types
{
    // System
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;

    public enum ItemType : uint
    {
        Bulletproof_Vest,
        Nightstick = EWeaponHash.Nightstick,
        Fire_Extinguisher = EWeaponHash.Fire_Extinguisher,
        SMG = EWeaponHash.SMG,
        Assault_SMG = EWeaponHash.Assault_SMG,
        Assault_Rifle = EWeaponHash.Assault_Rifle,
        Carbine_Rifle = EWeaponHash.Carbine_Rifle,
        Advanced_Rifle = EWeaponHash.Advanced_Rifle,
        Bullpup_Rifle = EWeaponHash.Bullpup_Rifle,
        MG = EWeaponHash.MG,
        Combat_MG = EWeaponHash.Combat_MG,
        Pump_Shotgun = EWeaponHash.Pump_Shotgun,
        Sawn_Off_Shotgun = EWeaponHash.Sawn_Off_Shotgun,
        Assault_Shotgun = EWeaponHash.Assault_Shotgun,
        Bullpup_Shotgun = EWeaponHash.Bullpup_Shotgun,
        Sniper_Rifle = EWeaponHash.Sniper_Rifle,
        Heavy_Sniper = EWeaponHash.Heavy_Sniper,
        Pistol = EWeaponHash.Pistol,
        Combat_Pistol = EWeaponHash.Combat_Pistol,
        AP_Pistol = EWeaponHash.AP_Pistol,
        Pistol_50 = EWeaponHash.Pistol_50,
        Heavy_Pistol = EWeaponHash.Heavy_Pistol,
        Stun_Gun = EWeaponHash.Stun_Gun,
        Micro_SMG = EWeaponHash.Micro_SMG,
        Flare_Gun = EWeaponHash.Flare_Gun,
        Grenade = EWeaponHash.Grenade,
        Sticky_Bomb = EWeaponHash.Sticky_Bomb,
        Smoke_Grenade = EWeaponHash.Smoke_Grenade,
        Flare = EWeaponHash.Flare,
    }

    [Serializable]
    public class Item
    {

        [XmlElement]
        public ItemType Type;
        [XmlElement]
        public WeaponComponent[] Components;
    }
}
