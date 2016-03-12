namespace PoliceStationArmory
{
    // System
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    // RPH
    using Rage;
    using Rage.Native;

    // PoliceStationArmoury
    using PoliceStationArmory.Types;
    using PoliceStationArmory.UI;

    internal class Armory
    {
        public Ped Cop { get; private set; }
        public Camera Cam { get; private set; }
        public bool IsPlayerUsingTheArmoury { get; private set; }
        public bool IsCopGivingAWeapon { get; private set; }

        public Armory()
        {
            userInterface = new UserInterface();
            userInterface.ItemSelected += OnItemSelected;

            Game.FrameRender += UIFrameRender;
            Game.RawFrameRender += UIRawFrameRender;
        }


        public void Update()
        {
            if (Cop.Exists())
            {
                if(canPlayerEnterTheArmory()/*Game.LocalPlayer.Character.IsInRangeOf2D(playerGetStuffPos.Position, 4.85f)*/)
                {
                    if(!IsPlayerUsingTheArmoury)
                        Game.DisplayHelp("Press ~INPUT_CONTEXT~ to enter the armory", 10);
                    else
                        Game.DisplayHelp("Press ~INPUT_CONTEXT~ to exit the armory", 10);

                    if ((Game.IsControlJustPressed(0, GameControl.Context) || Common.IsDisabledControlJustPressed(0, GameControl.Context)) && !IsCopGivingAWeapon)
                    {
                        if (!IsPlayerUsingTheArmoury)
                        {
                            Common.DisplayHud(false);
                            Common.DisplayRadar(false);
                            if (!Cam.Exists())
                                Cam = new Camera(false);
                            Cam.Position = camPos;
                            Cam.PointAtEntity(Cop, new Vector3(0f, 0.2f, -0.115f), true);
                            Camera tempCam = new Camera(false);
                            tempCam.Position = Common.GetGameplayCameraPosition();
                            Vector3 r = Common.GetGameplayCameraRotation();
                            tempCam.Rotation = new Rotator(r.X, r.Y, r.Z);
                            tempCam.Active = true;
                            Task playerTask = Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerGetStuffPos.Position, playerGetStuffPos.Heading, 1.0f, 0.0f, -1);
                            tempCam.Interpolate(Cam, 1525, true, true, true);
                            Cam.Active = true;
                            playerTask.WaitForCompletion();
                            Cop.Tasks.Clear();
                            Game.LocalPlayer.Character.IsPositionFrozen = true;
                            Cop.PlayAmbientSpeech(new string[] { Speech.GENERIC_HI, Speech.GENERIC_HOWS_IT_GOING }.GetRandomElement(), false);
                            Game.LocalPlayer.Character.PlayAmbientSpeech(new string[] { Speech.GENERIC_HI }.GetRandomElement(), false);
                            userInterface.CurrentMenu = UserInterface.ECurrentMenu.MainMenu;
                            Logger.LogTrivial("Player using the armory");
                            IsPlayerUsingTheArmoury = true;
                        }
                        else
                        {
                            IsPlayerUsingTheArmoury = false;
                            Logger.LogTrivial("Player exiting the armory");
                            Cop.PlayAmbientSpeech(Speech.GENERIC_BYE, false);
                            Game.LocalPlayer.Character.PlayAmbientSpeech(new string[] { Speech.GENERIC_BYE, Speech.GENERIC_THANKS }.GetRandomElement(), false);
                            Cam.PointAtEntity(Game.LocalPlayer.Character, Vector3.Zero, true);
                            Game.LocalPlayer.Character.IsPositionFrozen = false;
                            //Game.LocalPlayer.Character.Tasks.GoStraightToPosition(playerLeavesPos.Position, 1.0f, playerLeavesPos.Heading, 0.0f, -1).WaitForCompletion();
                            Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerLeavesPos.Position, playerLeavesPos.Heading, 1.0f, 0.0f, -1).WaitForCompletion(2250);

                            string scenario = new string[] { Scenario.WORLD_HUMAN_AA_COFFEE, Scenario.WORLD_HUMAN_CLIPBOARD, Scenario.WORLD_HUMAN_COP_IDLES }.GetRandomElement();
                            Scenario.StartInPlace(Cop, scenario, scenario == Scenario.WORLD_HUMAN_COP_IDLES ? true : false);
                            Logger.LogDebug("Cop playing scenario " + scenario);

                            Cam.Active = false;
                            Common.DisplayHud(true);
                            Common.DisplayRadar(true);
                        }
                    }
                }

                if (!Game.LocalPlayer.Character.IsInRangeOf2D(copSpawnPos.Position, 50f))
                {
                    Cop.Delete();
                    Logger.LogTrivial("Deleted cop");
                }
            }
            else
            {
                if (Game.LocalPlayer.Character.IsInRangeOf2D(copSpawnPos.Position, 20f))
                {
                    Cop = GetOrCreateCop();
                    string scenario = new string[] { Scenario.WORLD_HUMAN_AA_COFFEE, Scenario.WORLD_HUMAN_CLIPBOARD, Scenario.WORLD_HUMAN_COP_IDLES }.GetRandomElement();
                    Scenario.StartInPlace(Cop, scenario, scenario == Scenario.WORLD_HUMAN_COP_IDLES ? true : false);
                    Logger.LogDebug("Cop playing scenario " + scenario);
                }
            }
        }

        public Ped GetOrCreateCop()
        {
            Ped copCreatedByLSPDFR = World.GetClosestEntity(copSpawnPos.Position, 1.25f, GetEntitiesFlags.ConsiderHumanPeds | GetEntitiesFlags.ExcludePlayerPed) as Ped;
            if (!copCreatedByLSPDFR.Exists() || copCreatedByLSPDFR.IsDead)
            {
                Ped ped = new Ped("s_m_y_cop_01", copSpawnPos.Position, copSpawnPos.Heading);
                ped.IsPersistent = true;
                ped.Invincible = true;
                ped.BlockPermanentEvents = true;
                ped.Position = copSpawnPos.Position;
                ped.Heading = copSpawnPos.Heading;
                ped.Tasks.Clear();
                Logger.LogTrivial("No cop detected, creating new one");
                return ped;
            }
            else
            {
                copCreatedByLSPDFR.IsPersistent = true;
                copCreatedByLSPDFR.Invincible = true;
                copCreatedByLSPDFR.BlockPermanentEvents = true;
                copCreatedByLSPDFR.Position = copSpawnPos.Position;
                copCreatedByLSPDFR.Heading = copSpawnPos.Heading;
                copCreatedByLSPDFR.Tasks.Clear();
                Logger.LogTrivial("Cop detected, using it");
                return copCreatedByLSPDFR;
            }
        }

        private readonly Vector3[] enterCheckPos =
            {
                new Vector3(451.59f, -979.45f, 30.69f),
                new Vector3(451.59f, -981.02f, 30.69f),
                new Vector3(451.53f, -982.75f, 30.69f),
            };
        private const float checkRadius = 1.65f;
        private bool canPlayerEnterTheArmory()
        {
//#if DEBUG
//            foreach (Vector3 v3 in enterCheckPos)
//                Common.DrawMarker(MarkerType.VerticalCylinder, v3, Vector3.Zero, Vector3.Zero, new Vector3(checkRadius * 2), Color.FromArgb(180, Color.Red));
//#endif
            Ped player = Game.LocalPlayer.Character;
            return player.IsInRangeOf2D(enterCheckPos[0], checkRadius) ||
                   player.IsInRangeOf2D(enterCheckPos[1], checkRadius) ||
                   player.IsInRangeOf2D(enterCheckPos[2], checkRadius + 0.3f);
        }

        private void OnItemSelected(UserInterface.WeaponItem selectedItem)
        {
            GameFiber.StartNew(delegate
            {
                IsCopGivingAWeapon = true;
                NativeFunction.CallByName<uint>("SET_CURRENT_PED_WEAPON", Game.LocalPlayer.Character, (uint)EWeaponHash.Unarmed, true);
                ItemType type = GetTypeForWeapon(selectedItem.WeaponHash);
                if (type != ItemType.Misc)
                {
                    Task copAnimTask = Cop.PlayAnimation(type == ItemType.Handgun ? GiveHandgunAnimation : GiveRifleAnimation, -1, 1f, 0.5f, 0.0f);
                    Task playerAnimTask = Game.LocalPlayer.Character.PlayAnimation(type == ItemType.Handgun ? ReceiveHandgunAnimation : ReceiveRifleAnimation, -1, 1f, 0.5f, 0.0f);
                    GameFiber.Sleep(410);
                    Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.WeaponHash, 999, true);
                    Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                    if (Cop.IsAnySpeechPlaying)
                    {
                        GameFiber.StartNew(delegate
                        {
                            GameFiber.Sleep(900);
                            Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                        });
                    }
                    copAnimTask.WaitForCompletion();
                    Cop.Inventory.Weapons.Remove((WeaponHash)selectedItem.WeaponHash);
                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.WeaponHash, 999, true);
                }
                else
                {
                    if(selectedItem.MiscItem == MiscItems.Fire_Extinguisher || selectedItem.MiscItem == MiscItems.Nightstick)
                    {
                        Task copAnimTask = Cop.PlayAnimation(GiveHandgunAnimation, -1, 1f, 0.5f, 0.0f);
                        Task playerAnimTask = Game.LocalPlayer.Character.PlayAnimation(ReceiveHandgunAnimation, -1, 1f, 0.5f, 0.0f);
                        GameFiber.Sleep(410);
                        Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                        Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                        if (Cop.IsAnySpeechPlaying)
                        {
                            GameFiber.StartNew(delegate
                            {
                                GameFiber.Sleep(900);
                                Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                            });
                        }
                        copAnimTask.WaitForCompletion();
                        Cop.Inventory.Weapons.Remove((WeaponHash)selectedItem.MiscItem);
                        Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                    }
                    else if(selectedItem.MiscItem == MiscItems.Bulletproof_Vest)
                    {
                        Logger.LogTrivial("Bulletproof_Vest - NOT IMPLEMENTED");
                    }
                }
                IsCopGivingAWeapon = false;
            });
        }

        public void UIFrameRender(object sender, GraphicsEventArgs e)
        {
            if (IsPlayerUsingTheArmoury)
            {
                if (!userInterface.Visible)
                    userInterface.Visible = true;

                Common.DisableAllGameControls(GameControlGroup.MAX_INPUTGROUPS);

                UICommon.ProcessCursor();
            }
            else
            {
                if (userInterface.Visible)
                    userInterface.Visible = false;
            }

            userInterface.Process();
        }

        public void UIRawFrameRender(object sender, GraphicsEventArgs e)
        {
            userInterface.Draw(e);

            if (IsPlayerUsingTheArmoury)
                UICommon.DrawCursor(e);
        }

        public void CleanUp()
        {
            Game.FrameRender -= UIFrameRender;
            Game.RawFrameRender -= UIRawFrameRender;
            if (Cop.Exists())
            {
                Cop.Invincible = true;
                Cop.Dismiss();
            }
            if (Cam.Exists())
                Cam.Delete();
            Common.DisplayHud(true);
            Common.DisplayRadar(true);
            Game.LocalPlayer.Character.IsPositionFrozen = false;
            Game.LocalPlayer.Character.Tasks.ClearImmediately();
        }

        private ItemType GetTypeForWeapon(EWeaponHash hash)
        {
            switch (hash)                           // TODO: add flashlight
            {                
                // pistols    
                case EWeaponHash.Pistol:  
                case EWeaponHash.Combat_Pistol:         
                case EWeaponHash.AP_Pistol:             
                case EWeaponHash.Pistol_50:             
                case EWeaponHash.Heavy_Pistol:          
                case EWeaponHash.Stun_Gun:        
                // submachines
                case EWeaponHash.Micro_SMG:
                // throwables
                case EWeaponHash.Grenade:
                case EWeaponHash.Sticky_Bomb:
                case EWeaponHash.Smoke_Grenade:
                case EWeaponHash.BZ_Gas:
                case EWeaponHash.Flare:
                    return ItemType.Handgun;
                             


                case EWeaponHash.SMG:      
                case EWeaponHash.Assault_SMG:   
                // rifles  
                case EWeaponHash.Assault_Rifle:         
                case EWeaponHash.Carbine_Rifle:         
                case EWeaponHash.Advanced_Rifle:        
                case EWeaponHash.Bullpup_Rifle:    
                //mgs     
                case EWeaponHash.MG:                    
                case EWeaponHash.Combat_MG:        
                // shotguns    
                case EWeaponHash.Pump_Shotgun:          
                case EWeaponHash.Sawn_Off_Shotgun:      
                case EWeaponHash.Assault_Shotgun:       
                case EWeaponHash.Bullpup_Shotgun:       
                case EWeaponHash.Heavy_Shotgun:    
                // snipers  
                case EWeaponHash.Sniper_Rifle:          
                case EWeaponHash.Heavy_Sniper:          
                case EWeaponHash.Marksman_Rifle:
                    return ItemType.LongGun;

                case EWeaponHash.Fire_Extinguisher:
                // melee    
                case EWeaponHash.Nightstick:
                case EWeaponHash.Flashlight:
                    return ItemType.Misc;
            }
            return ItemType.Handgun;
        }

        private UserInterface userInterface;

        private readonly Animation GiveAmmoAnimation = new Animation("mp_cop_armoury", "ammo_on_counter_cop");
        private readonly Animation GiveHandgunAnimation = new Animation("mp_cop_armoury", "pistol_on_counter_cop");
        private readonly Animation GiveRifleAnimation = new Animation("mp_cop_armoury", "rifle_on_counter_cop");
        private readonly Animation GivePackageAnimation = new Animation("mp_cop_armoury", "package_from_counter_cop");

        private readonly Animation ReceiveAmmoAnimation = new Animation("mp_cop_armoury", "ammo_on_counter");
        private readonly Animation ReceiveHandgunAnimation = new Animation("mp_cop_armoury", "pistol_on_counter");
        private readonly Animation ReceiveRifleAnimation = new Animation("mp_cop_armoury", "rifle_on_counter");
        private readonly Animation ReceivePackageAnimation = new Animation("mp_cop_armoury", "package_from_counter");

        private readonly SpawnPoint copSpawnPos = new SpawnPoint(new Vector3(454.11f, -980.26f, 30.69f), 90f);
        private readonly SpawnPoint playerGetStuffPos = new SpawnPoint(new Vector3(452.26f, -980.0f, 30.69f), 263.19f);
        private readonly SpawnPoint playerLeavesPos = new SpawnPoint(new Vector3(449.24f, -982.84f, 30.69f), 90f);

        private readonly Vector3 camPos = new Vector3(452.53f, -981.61f, 31f);

        protected class UserInterface
        {
            public delegate void WeaponItemSelected(WeaponItem selectedItem);


            public event WeaponItemSelected ItemSelected = delegate { };
            
            public MenuItem HandgunsItem { get; }
            public MenuItem RifleItem { get; }
            public MenuItem ThrowableItem { get; }
            public MenuItem MiscItem { get; }

            public List<WeaponItem> HandgunWeaponItems { get; }
            public List<WeaponItem> LongGunsWeaponItems { get; }
            public List<WeaponItem> ThrowableWeaponItems { get; }
            public List<WeaponItem> MiscWeaponItems { get; }

            //private UIState _state;
            //public UIState State
            //{
            //    get
            //    {
            //        return _state;
            //    }
            //    set
            //    {
            //        if (_state == value)
            //            return;

            //        HandgunsItem.State = value;
            //        RifleItem.State = value;
            //        ThrowableItem.State = value;

            //        //foreach (WeaponItem item in HandgunWeaponItems)
            //        //{
            //        //    item.State = value;
            //        //}
            //        //foreach (WeaponItem item in RifleWeaponItems)
            //        //{
            //        //    item.State = value;
            //        //}
            //        //foreach (WeaponItem item in ThrowableWeaponItems)
            //        //{
            //        //    item.State = value;
            //        //}

            //        _state = value;
            //    }
            //}
            public bool Visible { get; set; }

            private ECurrentMenu _currentMenu = ECurrentMenu.MainMenu;
            public ECurrentMenu CurrentMenu
            {
                get
                {
                    return _currentMenu;
                }
                set
                {
                    switch (value)
                    {
                        case ECurrentMenu.MainMenu:
                            foreach (WeaponItem item in HandgunWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in LongGunsWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in ThrowableWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in MiscWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }

                            if (HandgunsItem.State != UIState.ComingIntoView || HandgunsItem.State != UIState.Showing)
                                HandgunsItem.State = UIState.ComingIntoView;

                            if (RifleItem.State != UIState.ComingIntoView || RifleItem.State != UIState.Showing)
                                RifleItem.State = UIState.ComingIntoView;

                            if (ThrowableItem.State != UIState.ComingIntoView || ThrowableItem.State != UIState.Showing)
                                ThrowableItem.State = UIState.ComingIntoView;

                            if (MiscItem.State != UIState.ComingIntoView || MiscItem.State != UIState.Showing)
                                MiscItem.State = UIState.ComingIntoView;
                            break;
                        case ECurrentMenu.HandgunsMenu:
                            foreach (WeaponItem item in LongGunsWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in ThrowableWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in MiscWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }

                            if (HandgunsItem.State != UIState.Hiding || HandgunsItem.State != UIState.Hidden)
                                HandgunsItem.State = UIState.Hiding;

                            if (RifleItem.State != UIState.Hiding || RifleItem.State != UIState.Hidden)
                                RifleItem.State = UIState.Hiding;

                            if (ThrowableItem.State != UIState.Hiding || ThrowableItem.State != UIState.Hidden)
                                ThrowableItem.State = UIState.Hiding;

                            if (MiscItem.State != UIState.Hiding || MiscItem.State != UIState.Hidden)
                                MiscItem.State = UIState.Hiding;

                            foreach (WeaponItem item in HandgunWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            break;
                        case ECurrentMenu.LongGunsMenu:
                            foreach (WeaponItem item in HandgunWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in ThrowableWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in MiscWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            if (HandgunsItem.State != UIState.Hiding || HandgunsItem.State != UIState.Hidden)
                                HandgunsItem.State = UIState.Hiding;

                            if (RifleItem.State != UIState.Hiding || RifleItem.State != UIState.Hidden)
                                RifleItem.State = UIState.Hiding;

                            if (ThrowableItem.State != UIState.Hiding || ThrowableItem.State != UIState.Hidden)
                                ThrowableItem.State = UIState.Hiding;

                            if (MiscItem.State != UIState.Hiding || MiscItem.State != UIState.Hidden)
                                MiscItem.State = UIState.Hiding;

                            foreach (WeaponItem item in LongGunsWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            break;
                        case ECurrentMenu.ThrowablesMenu:
                            foreach (WeaponItem item in HandgunWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in LongGunsWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in MiscWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }

                            if (HandgunsItem.State != UIState.Hiding || HandgunsItem.State != UIState.Hidden)
                                HandgunsItem.State = UIState.Hiding;

                            if (RifleItem.State != UIState.Hiding || RifleItem.State != UIState.Hidden)
                                RifleItem.State = UIState.Hiding;

                            if (ThrowableItem.State != UIState.Hiding || ThrowableItem.State != UIState.Hidden)
                                ThrowableItem.State = UIState.Hiding;

                            if (MiscItem.State != UIState.Hiding || MiscItem.State != UIState.Hidden)
                                MiscItem.State = UIState.Hiding;

                            foreach (WeaponItem item in ThrowableWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            break;
                        case ECurrentMenu.MiscMenu:
                            foreach (WeaponItem item in HandgunWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in LongGunsWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }
                            foreach (WeaponItem item in ThrowableWeaponItems)
                            {
                                if (item.State != UIState.Hiding || item.State != UIState.Hidden)
                                    item.State = UIState.Hiding;
                            }

                            if (HandgunsItem.State != UIState.Hiding || HandgunsItem.State != UIState.Hidden)
                                HandgunsItem.State = UIState.Hiding;

                            if (RifleItem.State != UIState.Hiding || RifleItem.State != UIState.Hidden)
                                RifleItem.State = UIState.Hiding;

                            if (ThrowableItem.State != UIState.Hiding || ThrowableItem.State != UIState.Hidden)
                                ThrowableItem.State = UIState.Hiding;

                            if (MiscItem.State != UIState.Hiding || MiscItem.State != UIState.Hidden)
                                MiscItem.State = UIState.Hiding;

                            foreach (WeaponItem item in MiscWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            break;
                    }
                    _currentMenu = value;
                }
            }

            public UserInterface()
            {
                HandgunsItem = new MenuItem("Handguns", Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\Handguns_Icon.png"));
                HandgunsItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.HandgunsMenu; };
                RifleItem = new MenuItem("Long guns", Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\Long_Guns_Icon.png"));
                RifleItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.LongGunsMenu; };
                ThrowableItem = new MenuItem("Throwables", Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\Throwables_Icon.png"));
                ThrowableItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.ThrowablesMenu; };
                MiscItem = new MenuItem("Misc", Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\Not_Added.png"));
                MiscItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.ThrowablesMenu; };

                HandgunWeaponItems = new List<WeaponItem>();
                LongGunsWeaponItems = new List<WeaponItem>();
                ThrowableWeaponItems = new List<WeaponItem>();
                MiscWeaponItems = new List<WeaponItem>();

                foreach (EWeaponHash hash in WeaponItem.GetAvalaibleHandgunWeapons())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForWeapon(hash, ItemType.Handgun);
                    item.BackgroundRectangle.Clicked += (s) => { invokeItemSelected(item); };
                    HandgunWeaponItems.Add(item);
                }

                foreach (EWeaponHash hash in WeaponItem.GetAvalaibleRifleWeapons())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForWeapon(hash, ItemType.LongGun);
                    item.BackgroundRectangle.Clicked += (s) => { invokeItemSelected(item); };
                    LongGunsWeaponItems.Add(item);
                }

                foreach (EWeaponHash hash in WeaponItem.GetAvalaibleThrowableWeapons())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForWeapon(hash, ItemType.Throwable);
                    item.BackgroundRectangle.Clicked += (s) => { invokeItemSelected(item); };
                    ThrowableWeaponItems.Add(item);
                }

                foreach (MiscItems mItem in WeaponItem.GetAvalaibleMiscItems())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForMiscItem(mItem);
                    item.BackgroundRectangle.Clicked += (s) => { invokeItemSelected(item); };
                    MiscWeaponItems.Add(item);
                }
                updateItemsPosition();
            }

            public void Process()
            {
                if (!Visible)
                    return;

                if(CurrentMenu != ECurrentMenu.MainMenu && Game.GetMouseState().IsRightButtonDown)
                {
                    CurrentMenu = ECurrentMenu.MainMenu;
                }

                HandgunsItem.Process();
                RifleItem.Process();
                ThrowableItem.Process();
                MiscItem.Process();

                foreach (WeaponItem item in HandgunWeaponItems)
                {
                    item.Process();
                }
                foreach (WeaponItem item in LongGunsWeaponItems)
                {
                    item.Process();
                }
                foreach (WeaponItem item in ThrowableWeaponItems)
                {
                    item.Process();
                }
                foreach (WeaponItem item in MiscWeaponItems)
                {
                    item.Process();
                }
            }

            public void Draw(GraphicsEventArgs e)
            {
                if (!Visible)
                    return;

                HandgunsItem.Draw(e);
                RifleItem.Draw(e);
                ThrowableItem.Draw(e);
                MiscItem.Draw(e);

                foreach (WeaponItem item in HandgunWeaponItems)
                {
                    item.Draw(e);
                }
                foreach (WeaponItem item in LongGunsWeaponItems)
                {
                    item.Draw(e);
                }
                foreach (WeaponItem item in ThrowableWeaponItems)
                {
                    item.Draw(e);
                }
                foreach (WeaponItem item in MiscWeaponItems)
                {
                    item.Draw(e);
                }
            }

            private void invokeItemSelected(WeaponItem item)
            {
                if (ItemSelected != null)
                    ItemSelected(item);
            }

            private void updateItemsPosition()
            {
                float x = Game.Resolution.Width - 600f;
                float y = 0f;
                float height = Game.Resolution.Height / 4;

                HandgunsItem.Texture.RectangleF = new RectangleF(x, y, HandgunsItem.Texture.Texture.Size.Width * 0.375f, HandgunsItem.Texture.Texture.Size.Height * 0.375f);
                HandgunsItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, HandgunsItem.Texture.Texture.Size.Height * 0.375f);
                HandgunsItem.Label.Position = new PointF(x + HandgunsItem.Texture.Texture.Size.Width * 0.4f, y + (HandgunsItem.Texture.Texture.Size.Height * 0.1125f));
                y += height;
                RifleItem.Texture.RectangleF = new RectangleF(x, y, RifleItem.Texture.Texture.Size.Width * 0.375f, RifleItem.Texture.Texture.Size.Height * 0.375f);
                RifleItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, RifleItem.Texture.Texture.Size.Height * 0.375f);
                RifleItem.Label.Position = new PointF(x + RifleItem.Texture.Texture.Size.Width * 0.4f, y + (RifleItem.Texture.Texture.Size.Height * 0.1125f));
                y += height;
                ThrowableItem.Texture.RectangleF = new RectangleF(x, y, ThrowableItem.Texture.Texture.Size.Width * 0.375f, ThrowableItem.Texture.Texture.Size.Height * 0.375f);
                ThrowableItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, ThrowableItem.Texture.Texture.Size.Height * 0.375f);
                ThrowableItem.Label.Position = new PointF(x + ThrowableItem.Texture.Texture.Size.Width * 0.4f, y + (ThrowableItem.Texture.Texture.Size.Height * 0.1125f));
                y += height;
                MiscItem.Texture.RectangleF = new RectangleF(x, y, MiscItem.Texture.Texture.Size.Width * 0.375f, MiscItem.Texture.Texture.Size.Height * 0.375f);
                MiscItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, MiscItem.Texture.Texture.Size.Height * 0.375f);
                MiscItem.Label.Position = new PointF(x + MiscItem.Texture.Texture.Size.Width * 0.4f, y + (MiscItem.Texture.Texture.Size.Height * 0.1125f));
                y += height;


                x = Game.Resolution.Width - 600f;
                y = 0f;

                foreach (WeaponItem item in HandgunWeaponItems)
                {
                    //Logger.LogTrivial("Hash: " + item.WeaponHash);
                    item.Texture.RectangleF = new RectangleF(x, y, item.Texture.Texture.Size.Width * 0.375f, item.Texture.Texture.Size.Height * 0.375f);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, item.Texture.Texture.Size.Height * 0.375f);
                    item.Label.Position = new PointF(x + item.Texture.Texture.Size.Width * 0.4f, y + (item.Texture.Texture.Size.Height * 0.095f/*0.1125f*/));
                    y += item.Texture.Texture.Size.Height * 0.375f;
                }


                x = Game.Resolution.Width - 600f;
                y = 0f;

                foreach (WeaponItem item in LongGunsWeaponItems)
                {
                    //Logger.LogTrivial("Hash: " + item.WeaponHash);
                    item.Texture.RectangleF = new RectangleF(x, y, item.Texture.Texture.Size.Width * 0.28125f, item.Texture.Texture.Size.Height * 0.28125f);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, item.Texture.Texture.Size.Height * 0.28125f);
                    item.Label.Position = new PointF(x + item.Texture.Texture.Size.Width * 0.4f, y + (item.Texture.Texture.Size.Height * 0.095f/*0.1125f*/));
                    y += item.Texture.Texture.Size.Height * 0.28125f;
                }


                x = Game.Resolution.Width - 600f;
                y = 0f;

                foreach (WeaponItem item in ThrowableWeaponItems)
                {
                    //Logger.LogTrivial("Hash: " + item.WeaponHash);
                    item.Texture.RectangleF = new RectangleF(x, y, item.Texture.Texture.Size.Width * 0.375f, item.Texture.Texture.Size.Height * 0.375f);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, item.Texture.Texture.Size.Height * 0.375f);
                    item.Label.Position = new PointF(x + item.Texture.Texture.Size.Width * 0.4f, y + (item.Texture.Texture.Size.Height * 0.095f));
                    y += item.Texture.Texture.Size.Height * 0.375f;
                }


                x = Game.Resolution.Width - 600f;
                y = 0f;

                foreach (WeaponItem item in MiscWeaponItems)
                {
                    //Logger.LogTrivial("Hash: " + item.WeaponHash);
                    item.Texture.RectangleF = new RectangleF(x, y, item.Texture.Texture.Size.Width * 0.375f, item.Texture.Texture.Size.Height * 0.375f);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, 1280, item.Texture.Texture.Size.Height * 0.375f);
                    item.Label.Position = new PointF(x + item.Texture.Texture.Size.Width * 0.4f, y + (item.Texture.Texture.Size.Height * 0.095f));
                    y += item.Texture.Texture.Size.Height * 0.375f;
                }
            }

            public enum ECurrentMenu
            {
                MainMenu,
                HandgunsMenu,
                LongGunsMenu,
                ThrowablesMenu,
                MiscMenu,
            }

            public class MenuItem
            {
                public UITexture Texture { get; }
                public UILabel Label { get; }
                public UIRectangle BackgroundRectangle { get; }

                private UIState _state;
                public UIState State
                {
                    get
                    {
                        return _state;
                    }
                    set
                    {
                        if (_state == value)
                            return;

                        Texture.State = value;
                        Label.State = value;
                        BackgroundRectangle.State = value;
                        _state = value;
                    }
                }

                public MenuItem(string label, Rage.Texture texture)
                {
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(label, "Arial", 22.5f, new PointF(), Color.White, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), Color.FromArgb(150, Color.DarkGray), Color.Black, UIRectangleType.FilledWithBorders, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle.Hovered += backRectHoveredEvent;
                }

                public void Process()
                {
                    BackgroundRectangle.Color = defaultBackRectColor;
                    BackgroundRectangle.Process();
                    Label.Process();
                    Texture.Process();
                }

                public void Draw(GraphicsEventArgs e)
                {
                    BackgroundRectangle.Draw(e);
                    Label.Draw(e);
                    Texture.Draw(e);
                }

                private Color defaultBackRectColor = Color.FromArgb(150, Color.DarkGray);
                private Color hoveredBackRectColor = ControlPaint.Light(Color.FromArgb(150, Color.DarkGray), 1.0f);
                private void backRectHoveredEvent(UIElementBase sender)
                {
                    BackgroundRectangle.Color = hoveredBackRectColor;
                }
            }

            public class WeaponItem
            {
                public ItemType Type { get; }
                public EWeaponHash WeaponHash { get; }
                public MiscItems MiscItem { get; }
                public UITexture Texture { get; }
                public UILabel Label { get; }
                public UIRectangle BackgroundRectangle { get; }

                private UIState _state;
                public UIState State
                {
                    get
                    {
                        return _state;
                    }
                    set
                    {
                        if (_state == value)
                            return;

                        Texture.State = value;
                        Label.State = value;
                        BackgroundRectangle.State = value;
                        _state = value;
                    }
                }

                public WeaponItem(EWeaponHash hash, ItemType type, Rage.Texture texture)
                {
                    Type = type;
                    WeaponHash = hash;
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(hash.ToString().Replace('_', ' '), "Arial", 22.5f, new PointF(), Color.White, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), Color.FromArgb(150, Color.DarkGray), Color.Black, UIRectangleType.FilledWithBorders, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle.Hovered += backRectHoveredEvent;
                }

                public WeaponItem(MiscItems item, Rage.Texture texture)
                {
                    Type = ItemType.Misc;
                    WeaponHash = 0;
                    MiscItem = item;
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(item.ToString().Replace('_', ' '), "Arial", 22.5f, new PointF(), Color.White, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), Color.FromArgb(150, Color.DarkGray), Color.Black, UIRectangleType.FilledWithBorders, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle.Hovered += backRectHoveredEvent;
                }

                public void Process()
                {
                    BackgroundRectangle.Color = defaultBackRectColor;
                    BackgroundRectangle.Process();
                    Label.Process();
                    Texture.Process();
                }

                public void Draw(GraphicsEventArgs e)
                {
                    BackgroundRectangle.Draw(e);
                    Label.Draw(e);
                    Texture.Draw(e);
                }

                private Color defaultBackRectColor = Color.FromArgb(150, Color.DarkGray);
                private Color hoveredBackRectColor = ControlPaint.Light(Color.FromArgb(150, Color.DarkGray), 1.0f);
                private void backRectHoveredEvent(UIElementBase sender)
                {
                    BackgroundRectangle.Color = hoveredBackRectColor;
                }

                public static WeaponItem GetWeaponItemForWeapon(EWeaponHash hash, ItemType type)
                {
                    Rage.Texture texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\" + hash + ".png");
                    if (texture == null)
                        texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\Not_Added.png");
                    return new WeaponItem(hash, type, texture);
                }
                public static WeaponItem GetWeaponItemForMiscItem(MiscItems item)
                {
                    Rage.Texture texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\" + item + ".png");
                    if(texture == null)
                        texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armory Resources\UI\Not_Added.png");
                    return new WeaponItem(item, texture);
                }

                //public static EWeaponHash[] GetAvalaibleWeapons()
                //{
                //    return new EWeaponHash[]
                //    {
                //        // melee     
                //        EWeaponHash.Nightstick,
                //        //EWeaponHash.Flashlight,   // doesn't have texture     

                //        // pistols  
                //        EWeaponHash.Pistol,
                //        EWeaponHash.Combat_Pistol,
                //        EWeaponHash.AP_Pistol,
                //        EWeaponHash.Pistol_50,
                //        EWeaponHash.Heavy_Pistol,
                //        EWeaponHash.Stun_Gun,              

                //        // submachines  done textures
                //        EWeaponHash.Micro_SMG,
                //        EWeaponHash.SMG,
                //        EWeaponHash.Assault_SMG,

                //        // rifles
                //        EWeaponHash.Assault_Rifle,
                //        EWeaponHash.Carbine_Rifle,
                //        EWeaponHash.Advanced_Rifle,
                //        EWeaponHash.Bullpup_Rifle,         

                //        //mgs
                //        EWeaponHash.MG,
                //        EWeaponHash.Combat_MG,             

                //        // shotguns  
                //        EWeaponHash.Pump_Shotgun,
                //        EWeaponHash.Sawn_Off_Shotgun,
                //        EWeaponHash.Assault_Shotgun,
                //        EWeaponHash.Bullpup_Shotgun,       
                //        //EWeaponHash.Heavy_Shotgun,       // doesn't have texture          

                //        // snipers     done textures except Marksman_Rifle
                //        EWeaponHash.Sniper_Rifle,
                //        EWeaponHash.Heavy_Sniper,          
                //        //EWeaponHash.Marksman_Rifle,     // doesn't have texture          

                //        // big guns
                //        //EWeaponHash.Grenade_Launcher,      
                //        //EWeaponHash.RPG,                   
                //        //EWeaponHash.Stinger,               
                //        //EWeaponHash.Minigun,               

                //        // throwables
                //        EWeaponHash.Grenade,
                //        EWeaponHash.Sticky_Bomb,
                //        EWeaponHash.Smoke_Grenade,     
                //        EWeaponHash.Flare,
                //        //EWeaponHash.BZ_Gas,                // doesn't have texture        
                //        //case EWeaponHash.Molotov,               
                //        EWeaponHash.Fire_Extinguisher,
                //    };
                //}

                public static EWeaponHash[] GetAvalaibleHandgunWeapons()
                {
                    return new EWeaponHash[]
{
                        // melee     
                        //EWeaponHash.Nightstick,
                        //EWeaponHash.Flashlight,   // doesn't have texture     

                        // pistols  
                        EWeaponHash.Pistol,
                        EWeaponHash.Combat_Pistol,
                        EWeaponHash.AP_Pistol,
                        EWeaponHash.Pistol_50,
                        EWeaponHash.Heavy_Pistol,
                        EWeaponHash.Stun_Gun,              

                        // submachines  
                        EWeaponHash.Micro_SMG,

                        //EWeaponHash.Fire_Extinguisher,
                    };
                }

                public static EWeaponHash[] GetAvalaibleRifleWeapons()
                {
                    return new EWeaponHash[]
                    {
                        // submachines
                        EWeaponHash.SMG,
                        EWeaponHash.Assault_SMG,

                        // rifles
                        EWeaponHash.Assault_Rifle,
                        EWeaponHash.Carbine_Rifle,
                        EWeaponHash.Advanced_Rifle,
                        EWeaponHash.Bullpup_Rifle,         

                        //mgs
                        EWeaponHash.MG,
                        EWeaponHash.Combat_MG,             

                        // shotguns  
                        EWeaponHash.Pump_Shotgun,
                        EWeaponHash.Sawn_Off_Shotgun,
                        EWeaponHash.Assault_Shotgun,
                        EWeaponHash.Bullpup_Shotgun,       
                        //EWeaponHash.Heavy_Shotgun,       // doesn't have texture          

                        // snipers  
                        EWeaponHash.Sniper_Rifle,
                        EWeaponHash.Heavy_Sniper,          
                        //EWeaponHash.Marksman_Rifle,     // doesn't have texture   
                    };
                }

                public static EWeaponHash[] GetAvalaibleThrowableWeapons()
                {
                    return new EWeaponHash[]
                    {
                        // throwables
                        EWeaponHash.Grenade,
                        EWeaponHash.Sticky_Bomb,
                        EWeaponHash.Smoke_Grenade,
                        EWeaponHash.Flare,
                    };
                }

                public static MiscItems[] GetAvalaibleMiscItems()
                {
                    return new MiscItems[]
                    {
                        // throwables
                        MiscItems.Bulletproof_Vest,
                        MiscItems.Nightstick,
                        MiscItems.Fire_Extinguisher,
                    };
                }
            }
        }

        protected enum MiscItems : uint
        {
            Bulletproof_Vest,
            Nightstick = EWeaponHash.Nightstick,
            Fire_Extinguisher = EWeaponHash.Fire_Extinguisher,

        }

        protected enum ItemType
        {
            Handgun,
            LongGun,
            Throwable,
            Misc,
        }
    }
}




//tempCam.Active = true;

//Camera tempCam2 = new Camera(false);
//tempCam2.Position = new Vector3(450.41f, -982.78f, 31f);
//tempCam2.SetRotationYaw(270.65f);
//tempCam.Active = true;
//Task playerTask = Game.LocalPlayer.Character.Tasks.GoStraightToPosition(playerGetStuffPos.Position, 1.0f, playerGetStuffPos.Heading, 0.0f, -1);
//Task playerTask = Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerGetStuffPos.Position, playerGetStuffPos.Heading, 1.0f, 0.0f, -1);
//tempCam.Interpolate(tempCam2, 500, true, true, true);
//tempCam2.Active = true;
//tempCam2.Interpolate(Cam, 1000, true, true, true);