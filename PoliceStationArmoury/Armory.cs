namespace PoliceStationArmory
{
    //
    // online character creation police room coords X:414.54  Y:-998.37  Z:-98.66
    //
    // police garage coords X:407.55  Y:-963.6  Z:-99
    //

    // System
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    using System.Text;

    // RPH
    using Rage;
    using Rage.Native;

    // PoliceStationArmoury
    using PoliceStationArmory.Types;
    using PoliceStationArmory.UI;

    internal class Armory
    {

        public const string MAIN_FOLDER     = @"Plugins\Police Station Armory Resources\";
        public const string UI_FOLDER       = MAIN_FOLDER + @"UI\";
        public const string LOADOUTS_FOLDER = MAIN_FOLDER + @"Loadouts\";

        public const string AUDIO_LIBRARY = "HUD_FRONTEND_DEFAULT_SOUNDSET";

        public const string AUDIO_UPDOWN = "NAV_UP_DOWN";
        //public const string AUDIO_LEFTRIGHT = "NAV_LEFT_RIGHT";
        public const string AUDIO_SELECT = "SELECT";
        public const string AUDIO_BACK = "BACK";
        public const string AUDIO_ERROR = "ERROR";

        public const int TEXTURE_WIDTH = 128, TEXTURE_HEIGHT = 64;

        public static Sound SoundInstance { get; private set; } = new Sound(-1);

        //public const float HANDGUNS_TEXTURES_MULTIPLIER     = 1.0f,
        //                   LONG_GUNS_TEXTURES_MULTIPLIER    = 1.0f,
        //                   THROWABLES_TEXTURES_MULTIPLIER   = 1.0f,
        //                   MISC_ITEMS_TEXTURES_MULTIPLIER   = 1.0f,
        //                   LOADOUTS_TEXTURES_MULTIPLIER     = 1.0f;

        private static Rage.Texture _missingTexture;
        public static Rage.Texture MissingTexture
        {
            get
            {
                if (_missingTexture == null)
                    _missingTexture = Game.CreateTextureFromFile(UI_FOLDER + "Missing_Texture.png");
                return _missingTexture;
            }
        }

        public static readonly Color BackgroundRectangleColor           = Color.FromArgb(140, Color.DarkGray),
                                     HoveredBackgroundRectangleColor    = Color.FromArgb(160, 224, 224, 224),
                                     SelectedBackgroundRectangleColor   = Color.FromArgb(160, Color.WhiteSmoke),
                                     BorderRectangleColor               = Color.FromArgb(180, Color.Black),

                                     LabelTextColor                     = Color.White,

                                     HelpTextBackgroundRectangleColor   = Color.FromArgb(195, Color.Black),
                                     HelpTextLabelTextColor             = Color.White,

                                     ScrollBarBackgroundColor           = Color.DarkGray,
                                     ScrollBarColor                     = Color.Black;      


        public Ped Cop { get; private set; }
        public Camera Cam { get; private set; }
        public bool IsPlayerUsingTheArmoury { get; private set; }
        public bool IsCopGivingAWeapon { get; private set; }
        public bool IsOnArmory { get; private set; }


        public Armory()
        {
            userInterface = new UserInterface();
            userInterface.WeaponItemSelected += OnWeaponItemSelected;
            userInterface.LoadoutItemSelected += OnLoadoutItemSelected;

            Game.FrameRender += UIFrameRender;
            Game.RawFrameRender += UIRawFrameRender;
        }


        public void Update()
        {
            if (Cop.Exists())
            {
                if (!Cop.Position.IsInRangeOf(copSpawnPos.Position, 0.25f))
                    Cop.Position = copSpawnPos.Position;

                if(canPlayerEnterTheArmory())
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
                            Game.LocalPlayer.Character.Tasks.Clear();
                            Game.LocalPlayer.Character.IsPositionFrozen = true;
                            Cop.PlayAmbientSpeech(new string[] { Speech.GENERIC_HI, Speech.GENERIC_HOWS_IT_GOING }.GetRandomElement(), false);
                            Game.LocalPlayer.Character.PlayAmbientSpeech(new string[] { Speech.GENERIC_HI }.GetRandomElement(), false);
                            userInterface.CurrentMenu = UserInterface.ECurrentMenu.MainMenu;
                            Logger.LogTrivial("Player using the armory");
                            IsOnArmory = true;
                            IsPlayerUsingTheArmoury = true;
                        }
                        else
                        {
                            IsPlayerUsingTheArmoury = false;
                            IsOnArmory = false;
                            Logger.LogTrivial("Player exiting the armory");
                            Cop.PlayAmbientSpeech(Speech.GENERIC_BYE, false);
                            Game.LocalPlayer.Character.PlayAmbientSpeech(new string[] { Speech.GENERIC_BYE, Speech.GENERIC_THANKS }.GetRandomElement(), false);
                            Cam.PointAtEntity(Game.LocalPlayer.Character, Vector3.Zero, true);
                            Game.LocalPlayer.Character.IsPositionFrozen = false;
                            //Game.LocalPlayer.Character.Tasks.GoStraightToPosition(playerLeavesPos.Position, 1.0f, playerLeavesPos.Heading, 0.0f, -1).WaitForCompletion();
                            Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerLeavesPos.Position, playerLeavesPos.Heading, 1.0f, 0.0f, -1).WaitForCompletion(2250);
                            Common.EnableGameControlsGroup(GameControlsGroup.MAX_INPUTGROUPS);

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

            if (Settings.OnTheFlyModeEnabled)
            {
                if (!IsOnArmory)
                {
                    if (Game.IsKeyDown(Settings.OpenCloseOnTheFlyMenuKey))
                    {
                        if (!IsPlayerUsingTheArmoury)
                        {
                            userInterface.CurrentMenu = UserInterface.ECurrentMenu.MainMenu;
                            IsOnArmory = false;
                            IsPlayerUsingTheArmoury = true;
                        }
                        else
                        {
                            IsOnArmory = false;
                            IsPlayerUsingTheArmoury = false;
                        }
                    }
                }
            }
        }

        public Ped GetOrCreateCop()
        {
            Ped copAlreadyCreated = World.GetClosestEntity(copSpawnPos.Position, 1.25f, GetEntitiesFlags.ConsiderHumanPeds | GetEntitiesFlags.ExcludePlayerPed) as Ped;
            if (!copAlreadyCreated.Exists() || copAlreadyCreated.IsDead)
            {
                Ped ped = new Ped("s_m_y_cop_01", copSpawnPos.Position, copSpawnPos.Heading);
                ped.IsPersistent = true;
                ped.IsInvincible = true;
                ped.BlockPermanentEvents = true;
                ped.Position = copSpawnPos.Position;
                ped.Heading = copSpawnPos.Heading;
                ped.Tasks.Clear();
                Logger.LogTrivial("No cop detected, creating new one");
                return ped;
            }
            else
            {
                copAlreadyCreated.IsPersistent = true;
                copAlreadyCreated.IsInvincible = true;
                copAlreadyCreated.BlockPermanentEvents = true;
                copAlreadyCreated.Position = copSpawnPos.Position;
                copAlreadyCreated.Heading = copSpawnPos.Heading;
                copAlreadyCreated.Tasks.Clear();
                Logger.LogTrivial("Cop detected, using it");
                return copAlreadyCreated;
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
#if DEBUG
            if(Game.IsKeyDownRightNow(Keys.RControlKey))
                foreach (Vector3 v3 in enterCheckPos)
                    Common.DrawMarker(MarkerType.VerticalCylinder, v3, Vector3.Zero, Vector3.Zero, new Vector3(checkRadius * 2), Color.FromArgb(180, Color.Red));
#endif
            Ped player = Game.LocalPlayer.Character;
            return player.IsInRangeOf(enterCheckPos[0], checkRadius) ||
                   player.IsInRangeOf(enterCheckPos[1], checkRadius) ||
                   player.IsInRangeOf(enterCheckPos[2], checkRadius + 0.3f);
        }

        private void OnWeaponItemSelected(UserInterface.WeaponItem selectedItem)
        {
            if (!IsOnArmory)
            {
                GameFiber.StartNew(delegate
                {
                    SoundInstance.PlayFrontend(AUDIO_SELECT, AUDIO_LIBRARY);

                    ItemType type = selectedItem.Type;
                    if (type != ItemType.Misc)
                    {
                        Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.WeaponHash, 999, true);
                    }
                    else
                    {
                        if (selectedItem.MiscItem == MiscItems.Fire_Extinguisher || selectedItem.MiscItem == MiscItems.Nightstick || selectedItem.MiscItem == MiscItems.Flashlight)
                        {
                            Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                        }
                        else if (selectedItem.MiscItem == MiscItems.Bulletproof_Vest)
                        {
                            Game.LocalPlayer.Character.Armor = 250;
                        }
                        else if (selectedItem.MiscItem == MiscItems.Refill_Ammo)
                        {
                            foreach (WeaponDescriptor w in Game.LocalPlayer.Character.Inventory.Weapons)
                            {
                                w.Ammo += 500;
                            }
                        }
                    }
                });
                return;
            }

            if (IsCopGivingAWeapon)
            {
                SoundInstance.PlayFrontend(AUDIO_ERROR, AUDIO_LIBRARY);
                return;
            }

            GameFiber.StartNew(delegate
            {
                IsCopGivingAWeapon = true;
                SoundInstance.PlayFrontend(AUDIO_SELECT, AUDIO_LIBRARY);
                NativeFunction.CallByName<uint>("SET_CURRENT_PED_WEAPON", Game.LocalPlayer.Character, (uint)EWeaponHash.Unarmed, true);
                //ItemType type = GetTypeForWeapon(selectedItem.WeaponHash);
                ItemType type = selectedItem.Type;
                if (type != ItemType.Misc)
                {
                    AnimationTask copAnimTask =  type == ItemType.LongGun ? GiveRifleAnimation.PlayOnPed(Cop) : GiveHandgunAnimation.PlayOnPed(Cop);
                    AnimationTask playerAnimTask = type == ItemType.LongGun ? ReceiveRifleAnimation.PlayOnPed(Game.LocalPlayer.Character) : ReceiveHandgunAnimation.PlayOnPed(Game.LocalPlayer.Character);
                    GameFiber.Sleep(420);
                    Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.WeaponHash, 999, true);
                    Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                    if (Cop.IsAnySpeechPlaying)
                    {
                        GameFiber.StartNew(delegate
                        {
                            GameFiber.Sleep(1000);
                            Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                        });
                    }
                    copAnimTask.WaitForCompletion();
                    Cop.Inventory.Weapons.Remove((WeaponHash)selectedItem.WeaponHash);
                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.WeaponHash, 999, true);
                }
                else
                {
                    if (selectedItem.MiscItem == MiscItems.Fire_Extinguisher || selectedItem.MiscItem == MiscItems.Nightstick || selectedItem.MiscItem == MiscItems.Flashlight)
                    {
                        AnimationTask copAnimTask = GiveHandgunAnimation.PlayOnPed(Cop);
                        AnimationTask playerAnimTask = ReceiveHandgunAnimation.PlayOnPed(Game.LocalPlayer.Character);
                        GameFiber.Sleep(420);
                        Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                        Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                        if (Cop.IsAnySpeechPlaying)
                        {
                            GameFiber.StartNew(delegate
                            {
                                GameFiber.Sleep(1000);
                                Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                            });
                        }
                        copAnimTask.WaitForCompletion();
                        Cop.Inventory.Weapons.Remove((WeaponHash)selectedItem.MiscItem);
                        Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                    }
                    else if (selectedItem.MiscItem == MiscItems.Bulletproof_Vest)
                    {
                        AnimationTask copAnimTask = GiveAmmoAnimation.PlayOnPed(Cop);
                        AnimationTask playerAnimTask = ReceivePackageAnimation.PlayOnPed(Game.LocalPlayer.Character);
                        GameFiber.Sleep(500);
                        Rage.Object tempObj = new Rage.Object("prop_armour_pickup", Vector3.Zero);
                        tempObj.AttachTo(Cop, Cop.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, 0f), new Rotator(0f, 180f, 0f));
                        //Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                        Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                        if (Cop.IsAnySpeechPlaying)
                        {
                            GameFiber.StartNew(delegate
                            {
                                GameFiber.Sleep(1000);
                                Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                            });
                        }
                        GameFiber.Sleep(3500);
                        tempObj.AttachTo(Game.LocalPlayer.Character, Game.LocalPlayer.Character.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, 0f), new Rotator(0f, 0f, 0f));
                        copAnimTask.WaitForCompletion();
                        tempObj.Detach();
                        Vector3 pos = tempObj.Position;
                        Rotator rot = tempObj.Rotation;
                        tempObj.Delete();
                        Common.CreatePickup(PickupType.Armour, pos, rot, 250);
                        //Game.LocalPlayer.Character.Armor = 250;
                        //Cop.Inventory.Weapons.Remove((WeaponHash)selectedItem.MiscItem);
                        //Game.LocalPlayer.Character.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                    }
                    else if (selectedItem.MiscItem == MiscItems.Refill_Ammo)
                    {
                        // prop_ld_ammo_pack_01, prop_ld_ammo_pack_02, prop_ld_ammo_pack_03
                        AnimationTask copAnimTask = GiveAmmoAnimation.PlayOnPed(Cop);
                        AnimationTask playerAnimTask = ReceiveAmmoAnimation.PlayOnPed(Game.LocalPlayer.Character);
                        GameFiber.Sleep(500);
                        Rage.Object tempObj = new Rage.Object("prop_ld_ammo_pack_0" + Globals.Random.Next(1, 4), Vector3.Zero);
                        tempObj.AttachTo(Cop, Cop.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, 0f), new Rotator(0f, 180f, 0f));
                        //Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                        Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                        if (Cop.IsAnySpeechPlaying)
                        {
                            GameFiber.StartNew(delegate
                            {
                                GameFiber.Sleep(1000);
                                Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                            });
                        }
                        //GameFiber.Sleep(3500);
                        //tempObj.AttachTo(Game.LocalPlayer.Character, Game.LocalPlayer.Character.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, 0f), new Rotator(0f, 0f, 0f));
                        //copAnimTask.WaitForCompletion();
                        //tempObj.Detach();
                        //Vector3 pos = tempObj.Position;
                        //Rotator rot = tempObj.Rotation;
                        while (true)
                        {
                            GameFiber.Yield();
                            if (playerAnimTask.CurrentTime >= 7.5f || !playerAnimTask.IsActive)
                                break;
                        }
                        tempObj.Delete();
                        foreach (WeaponDescriptor w in Game.LocalPlayer.Character.Inventory.Weapons)
                        {
                            w.Ammo += 250;
                        }
                    }
                }
                IsCopGivingAWeapon = false;
            });
        }

        private void OnLoadoutItemSelected(UserInterface.LoadoutItem selectedItem)
        {
            if (!IsOnArmory)
            {
                GameFiber.StartNew(delegate
                {
                    SoundInstance.PlayFrontend(AUDIO_SELECT, AUDIO_LIBRARY);

                    NativeFunction.CallByName<uint>("SET_CURRENT_PED_WEAPON", Game.LocalPlayer.Character, (uint)EWeaponHash.Unarmed, true);

                    selectedItem.Loadout.GiveToPed(Game.LocalPlayer.Character);
                });
                return;
            }

            if (IsCopGivingAWeapon)
            {
                SoundInstance.PlayFrontend(AUDIO_ERROR, AUDIO_LIBRARY);
                return;
            }

            GameFiber.StartNew(delegate
            {
                IsCopGivingAWeapon = true;
                SoundInstance.PlayFrontend(AUDIO_SELECT, AUDIO_LIBRARY);
                NativeFunction.CallByName<uint>("SET_CURRENT_PED_WEAPON", Game.LocalPlayer.Character, (uint)EWeaponHash.Unarmed, true);
                AnimationTask copAnimTask = GiveAmmoAnimation.PlayOnPed(Cop);
                AnimationTask playerAnimTask = ReceivePackageAnimation.PlayOnPed(Game.LocalPlayer.Character);
                GameFiber.Sleep(500);
                Rage.Object tempObj = new Rage.Object("prop_big_bag_01", Vector3.Zero);
                tempObj.AttachTo(Cop, Cop.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, 0.2f), new Rotator(0f, 180f, 0f));
                //Cop.Inventory.GiveNewWeapon((WeaponHash)selectedItem.MiscItem, 999, true);
                Cop.PlayAmbientSpeech(Speech.CHAT_STATE, false);
                if (Cop.IsAnySpeechPlaying)
                {
                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Sleep(1000);
                        Game.LocalPlayer.Character.PlayAmbientSpeech(Speech.CHAT_RESP, false);
                    });
                }
                GameFiber.Sleep(3500);
                tempObj.AttachTo(Game.LocalPlayer.Character, Game.LocalPlayer.Character.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, -0.2f/*285*/), new Rotator(0f, 0f, 0f));
                copAnimTask.WaitForCompletion();
                tempObj.Detach();
                Game.LocalPlayer.Character.IsPositionFrozen = false;
                Game.LocalPlayer.Character.Tasks.AchieveHeading(Game.LocalPlayer.Character.GetHeadingTowards(tempObj)).WaitForCompletion(3000);
                Game.LocalPlayer.Character.IsPositionFrozen = true;
                TendToDeadIdleLoopAnimation.PlayOnPed(Game.LocalPlayer.Character);
                GameFiber.Sleep(2750);
                selectedItem.Loadout.GiveToPed(Game.LocalPlayer.Character);
                Game.LocalPlayer.Character.Tasks.Clear();
                Game.LocalPlayer.Character.IsPositionFrozen = false;
                Game.LocalPlayer.Character.Tasks.FollowNavigationMeshToPosition(playerGetStuffPos.Position, playerGetStuffPos.Heading, 1.0f, 0.0f, -1).WaitForCompletion();
                Game.LocalPlayer.Character.IsPositionFrozen = true;
                //Game.LocalPlayer.Character.Tasks.AchieveHeading(playerGetStuffPos.Heading).WaitForCompletion(1000);

                AnimationTask playerAnimTask2 = GiveAmmoAnimation.PlayOnPed(Game.LocalPlayer.Character);
                AnimationTask copAnimTask2 = ReceivePackageAnimation.PlayOnPed(Cop);
                GameFiber.Sleep(500);
                tempObj.AttachTo(Game.LocalPlayer.Character, Game.LocalPlayer.Character.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, 0.2f), new Rotator(0f, 180f, 0f));
                GameFiber.Sleep(3500);
                tempObj.AttachTo(Cop, Cop.GetBoneIndex(PedBoneId.RightPhHand), new Vector3(0f, 0f, -0.2f/*285*/), new Rotator(0f, 0f, 0f));
                playerAnimTask2.WaitForCompletion();
                tempObj.Detach();
                tempObj.Delete();
                //tempObj.Dismiss();

                IsCopGivingAWeapon = false;
            });
        }

        public void UIFrameRender(object sender, GraphicsEventArgs e)
        {
            if (IsPlayerUsingTheArmoury)
            {
                if (!userInterface.Visible)
                    userInterface.Visible = true;

                Common.DisableGameControlsGroup(GameControlsGroup.MAX_INPUTGROUPS);

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
            userInterface.Draw(e.Graphics);

            if (IsPlayerUsingTheArmoury)
                UICommon.DrawCursor(e.Graphics);
        }

        public void CleanUp()
        {
            Game.FrameRender -= UIFrameRender;
            Game.RawFrameRender -= UIRawFrameRender;
            if (Cop.Exists())
            {
                Cop.IsInvincible = true;
                Cop.Dismiss();
            }
            if (Cam.Exists())
                Cam.Delete();
            userInterface = null;
            Common.DisplayHud(true);
            Common.DisplayRadar(true);
            Camera.DeleteAllCameras();
            Common.EnableGameControlsGroup(GameControlsGroup.MAX_INPUTGROUPS);
            Game.LocalPlayer.Character.IsPositionFrozen = false;
            Game.LocalPlayer.Character.Tasks.ClearImmediately();
        }

        private UserInterface userInterface;

        private readonly Animation GiveAmmoAnimation        = new Animation("mp_cop_armoury",   "ammo_on_counter_cop",        AnimationFlags.None, 1f, 0.5f, 0.0f);
        private readonly Animation GiveHandgunAnimation     = new Animation("mp_cop_armoury",   "pistol_on_counter_cop",      AnimationFlags.None, 1f, 0.5f, 0.0f);
        private readonly Animation GiveRifleAnimation       = new Animation("mp_cop_armoury",   "rifle_on_counter_cop",       AnimationFlags.None, 1f, 0.5f, 0.0f);
        private readonly Animation GivePackageAnimation     = new Animation("mp_cop_armoury",   "package_from_counter",       AnimationFlags.None, 1f, 0.5f, 0.0f);

        private readonly Animation ReceiveAmmoAnimation     = new Animation("mp_cop_armoury",   "ammo_on_counter",            AnimationFlags.None, 1f, 0.5f, 0.0f);
        private readonly Animation ReceiveHandgunAnimation  = new Animation("mp_cop_armoury",   "pistol_on_counter",          AnimationFlags.None, 1f, 0.5f, 0.0f);
        private readonly Animation ReceiveRifleAnimation    = new Animation("mp_cop_armoury",   "rifle_on_counter",           AnimationFlags.None, 1f, 0.5f, 0.0f);
        private readonly Animation ReceivePackageAnimation  = new Animation("mp_cop_armoury",   "package_from_counter_cop",   AnimationFlags.None, 1f, 0.5f, 0.0f);

        private readonly Animation TendToDeadIdleLoopAnimation = new Animation("amb@medic@standing@tendtodead@idle_a", "idle_a", AnimationFlags.Loop, 2.0f, 0.5f, 0.0f);   // TODO: improve searching anim for loadouts 


        private readonly SpawnPoint copSpawnPos = new SpawnPoint(new Vector3(454.11f, -980.26f, 30.69f), 90f);
        private readonly SpawnPoint playerGetStuffPos = new SpawnPoint(new Vector3(452.26f, -980.0f, 30.69f), 263.19f);
        private readonly SpawnPoint playerLeavesPos = new SpawnPoint(new Vector3(449.24f, -982.84f, 30.69f), 90f);

        private readonly Vector3 camPos = new Vector3(452.53f, -981.61f, 31f);

        protected class UserInterface
        {
            public delegate void WeaponItemSelectedEventHandler(WeaponItem selectedItem);
            public delegate void LoadoutItemSelectedEventHandler(LoadoutItem selectedItem);


            public event WeaponItemSelectedEventHandler WeaponItemSelected = delegate { };
            public event LoadoutItemSelectedEventHandler LoadoutItemSelected = delegate { };

            public MenuItem HandgunsItem { get; }
            public MenuItem RifleItem { get; }
            public MenuItem ThrowableItem { get; }
            public MenuItem MiscItem { get; }
            public MenuItem PredefinedLoadoutItem { get; }
            public List<MenuItem> MainMenuItems { get; }

            public List<WeaponItem> HandgunWeaponItems { get; }
            public List<WeaponItem> LongGunsWeaponItems { get; }
            public List<WeaponItem> ThrowableWeaponItems { get; }
            public List<WeaponItem> MiscWeaponItems { get; }
            public List<LoadoutItem> PredefinedLoadoutItems { get; }

            public List<Item> AllItems { get; }
            
            public ScrollBar ScrollBarInstance { get; }

            public bool Visible { get; set; }

            private Item _selectedItem;
            public Item SelectedItem
            {
                get
                {
                    return _selectedItem;
                }
                set
                {
                    if (value == null)
                        return;

                    if(_selectedItem != null)
                        _selectedItem.Selected = false;

                    _selectedItem = value;
                    _selectedItem.Selected = true;

                    SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler SelectedItemChanged;

            public event EventHandler MenuChanged;

            private ECurrentMenu _currentMenu = ECurrentMenu.MainMenu;
            public ECurrentMenu CurrentMenu
            {
                get
                {
                    return _currentMenu;
                }
                set
                {
                    //if (value == _currentMenu)
                    //    return;

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
                            foreach (LoadoutItem item in PredefinedLoadoutItems)
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

                            if (PredefinedLoadoutItem.State != UIState.ComingIntoView || PredefinedLoadoutItem.State != UIState.Showing)
                                PredefinedLoadoutItem.State = UIState.ComingIntoView;
                            ScrollBarInstance.Visible = false;
                            SelectedItem = MainMenuItems[0];
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
                            foreach (LoadoutItem item in PredefinedLoadoutItems)
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

                            if (PredefinedLoadoutItem.State != UIState.Hiding || PredefinedLoadoutItem.State != UIState.Hidden)
                                PredefinedLoadoutItem.State = UIState.Hiding;

                            foreach (WeaponItem item in HandgunWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            ScrollBarInstance.Visible = true;
                            SelectedItem = HandgunWeaponItems[0];
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
                            foreach (LoadoutItem item in PredefinedLoadoutItems)
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

                            if (PredefinedLoadoutItem.State != UIState.Hiding || PredefinedLoadoutItem.State != UIState.Hidden)
                                PredefinedLoadoutItem.State = UIState.Hiding;

                            foreach (WeaponItem item in LongGunsWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            ScrollBarInstance.Visible = true;
                            SelectedItem = LongGunsWeaponItems[0];
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
                            foreach (LoadoutItem item in PredefinedLoadoutItems)
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

                            if (PredefinedLoadoutItem.State != UIState.Hiding || PredefinedLoadoutItem.State != UIState.Hidden)
                                PredefinedLoadoutItem.State = UIState.Hiding;

                            foreach (WeaponItem item in ThrowableWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            ScrollBarInstance.Visible = true;
                            SelectedItem = ThrowableWeaponItems[0];
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
                            foreach (LoadoutItem item in PredefinedLoadoutItems)
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

                            if (PredefinedLoadoutItem.State != UIState.Hiding || PredefinedLoadoutItem.State != UIState.Hidden)
                                PredefinedLoadoutItem.State = UIState.Hiding;

                            foreach (WeaponItem item in MiscWeaponItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            ScrollBarInstance.Visible = true;
                            SelectedItem = MiscWeaponItems[0];
                            break;
                        case ECurrentMenu.PredefinedLoadoutsMenu:
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

                            if (HandgunsItem.State != UIState.Hiding || HandgunsItem.State != UIState.Hidden)
                                HandgunsItem.State = UIState.Hiding;

                            if (RifleItem.State != UIState.Hiding || RifleItem.State != UIState.Hidden)
                                RifleItem.State = UIState.Hiding;

                            if (ThrowableItem.State != UIState.Hiding || ThrowableItem.State != UIState.Hidden)
                                ThrowableItem.State = UIState.Hiding;

                            if (MiscItem.State != UIState.Hiding || MiscItem.State != UIState.Hidden)
                                MiscItem.State = UIState.Hiding;

                            if (PredefinedLoadoutItem.State != UIState.Hiding || PredefinedLoadoutItem.State != UIState.Hidden)
                                PredefinedLoadoutItem.State = UIState.Hiding;

                            foreach (LoadoutItem item in PredefinedLoadoutItems)
                            {
                                if (item.State != UIState.ComingIntoView || item.State != UIState.Showing)
                                    item.State = UIState.ComingIntoView;
                            }
                            ScrollBarInstance.Visible = true;
                            SelectedItem = PredefinedLoadoutItems[0];
                            break;
                    }
                    _currentMenu = value;
                    ScrollBarInstance.SetValue(0.0f);
                    calculateItemsPosition();

                    MenuChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public UserInterface()
            {
                HandgunsItem = new MenuItem("Handguns", Game.CreateTextureFromFile(UI_FOLDER + "Handguns_Icon.png"));
                HandgunsItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.HandgunsMenu; };
                RifleItem = new MenuItem("Long guns", Game.CreateTextureFromFile(UI_FOLDER + "Long_Guns_Icon.png"));
                RifleItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.LongGunsMenu; };
                ThrowableItem = new MenuItem("Throwables", Game.CreateTextureFromFile(UI_FOLDER + "Throwables_Icon.png"));
                ThrowableItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.ThrowablesMenu; };
                MiscItem = new MenuItem("Misc", Game.CreateTextureFromFile(UI_FOLDER + "Misc_Icon.png")); 
                MiscItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.MiscMenu; };
                PredefinedLoadoutItem = new MenuItem("Loadouts", Game.CreateTextureFromFile(UI_FOLDER + "Loadouts_Icon.png")); 
                PredefinedLoadoutItem.BackgroundRectangle.Clicked += (s) => { CurrentMenu = ECurrentMenu.PredefinedLoadoutsMenu; };

                MainMenuItems = new List<MenuItem> { HandgunsItem, RifleItem, ThrowableItem, MiscItem, PredefinedLoadoutItem };

                HandgunWeaponItems = new List<WeaponItem>();
                LongGunsWeaponItems = new List<WeaponItem>();
                ThrowableWeaponItems = new List<WeaponItem>();
                MiscWeaponItems = new List<WeaponItem>();
                PredefinedLoadoutItems = new List<LoadoutItem>();

                foreach (EWeaponHash hash in WeaponItem.GetAvalaibleHandgunWeapons())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForWeapon(hash, ItemType.Handgun);
                    item.BackgroundRectangle.Clicked += (s) => { invokeWeaponItemSelected(item); };
                    HandgunWeaponItems.Add(item);
                }

                foreach (EWeaponHash hash in WeaponItem.GetAvalaibleLongWeapons())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForWeapon(hash, ItemType.LongGun);
                    item.BackgroundRectangle.Clicked += (s) => { invokeWeaponItemSelected(item); };
                    LongGunsWeaponItems.Add(item);
                }

                foreach (EWeaponHash hash in WeaponItem.GetAvalaibleThrowableWeapons())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForWeapon(hash, ItemType.Throwable);
                    item.BackgroundRectangle.Clicked += (s) => { invokeWeaponItemSelected(item); };
                    ThrowableWeaponItems.Add(item);
                }

                foreach (MiscItems mItem in WeaponItem.GetAvalaibleMiscItems())
                {
                    WeaponItem item = WeaponItem.GetWeaponItemForMiscItem(mItem);
                    item.BackgroundRectangle.Clicked += (s) => { invokeWeaponItemSelected(item); };
                    MiscWeaponItems.Add(item);
                }

                foreach (string file in Directory.GetFiles(LOADOUTS_FOLDER, "*.xml", SearchOption.TopDirectoryOnly))
                {
                    Loadout loadout = Loadout.GetFromXML(Path.GetFullPath(file));
                    LoadoutItem item = new LoadoutItem(loadout);
                    item.BackgroundRectangle.Clicked += (s) => { invokeLoadoutItemSelected(item); };
                    PredefinedLoadoutItems.Add(item);
                }

                ScrollBarInstance = new ScrollBar();
                ScrollBarInstance.ValueChanged += (s, e) => { calculateItemsPosition(); };

                AllItems = new List<Item>();
                AllItems.AddRange(MainMenuItems);
                AllItems.AddRange(HandgunWeaponItems);
                AllItems.AddRange(LongGunsWeaponItems);
                AllItems.AddRange(ThrowableWeaponItems);
                AllItems.AddRange(MiscWeaponItems);
                AllItems.AddRange(PredefinedLoadoutItems);
                

                calculateItemsPosition();
                CurrentMenu = ECurrentMenu.MainMenu;
                hideAll();
            }

            public void Process()
            {
                if (!Visible)
                    return;

                if (CurrentMenu != ECurrentMenu.MainMenu && (Game.GetMouseState().IsRightButtonDown || Common.IsDisabledControlJustPressed(0, GameControl.FrontendCancel)))
                {
                    SoundInstance.PlayFrontend(AUDIO_BACK, AUDIO_LIBRARY);
                    CurrentMenu = ECurrentMenu.MainMenu;
                }

                HandgunsItem.Process();
                RifleItem.Process();
                ThrowableItem.Process();
                MiscItem.Process();
                PredefinedLoadoutItem.Process();

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
                foreach (LoadoutItem item in PredefinedLoadoutItems)
                {
                    item.Process();
                }


                if (Common.IsDisabledControlPressed(0, GameControl.FrontendUp))
                {
                    if (!inputUpPressed)
                    {
                        inputCounter = 0;
                        inputUpPressed = true;
                        InputGoUp();
                    }
                    else
                    {
                        inputCounter++;
                        if(inputCounter >= 80)
                        {
                            InputGoUp();
                            inputCounter -= 10;
                        }
                    }
                }
                else if (Common.IsDisabledControlPressed(0, GameControl.FrontendDown))
                {
                    if (!inputDownPressed)
                    {
                        inputCounter = 0;
                        inputDownPressed = true;
                        InputGoDown();
                    }
                    else
                    {
                        inputCounter++;
                        if (inputCounter >= 80)
                        {
                            InputGoDown();
                            inputCounter -= 10;
                        }
                    }
                }
                else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept))
                {
                    InputSelect();
                }
                else
                {
                    inputUpPressed = false;
                    inputDownPressed = false;
                }
                // TODO: improve scroll bar movement with keyboard and controller input

                ScrollBarInstance.Process();
            }
            int inputCounter = 0;
            bool inputUpPressed = false, inputDownPressed = false;

            public void Draw(Rage.Graphics g)
            {
                if (!Visible)
                    return;

                HandgunsItem.Draw(g);
                RifleItem.Draw(g);
                ThrowableItem.Draw(g);
                MiscItem.Draw(g);
                PredefinedLoadoutItem.Draw(g);

                foreach (WeaponItem item in HandgunWeaponItems)
                {
                    item.Draw(g);
                }
                foreach (WeaponItem item in LongGunsWeaponItems)
                {
                    item.Draw(g);
                }
                foreach (WeaponItem item in ThrowableWeaponItems)
                {
                    item.Draw(g);
                }
                foreach (WeaponItem item in MiscWeaponItems)
                {
                    item.Draw(g);
                }
                foreach (LoadoutItem item in PredefinedLoadoutItems)
                {
                    item.Draw(g);
                }
                foreach (LoadoutItem item in PredefinedLoadoutItems)
                {
                    item.DrawHelpText(g);
                }


                ScrollBarInstance.Draw(g);
            }

            private void InputGoDown()
            {
                int currentIndex, newIndex;

                SoundInstance.PlayFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
                switch (CurrentMenu)
                {
                    case ECurrentMenu.MainMenu:
                        if(!MainMenuItems.Contains(SelectedItem))
                        {
                            SelectedItem = MainMenuItems[0];
                            break;
                        }

                        currentIndex = MainMenuItems.IndexOf((MenuItem)SelectedItem);
                        newIndex = currentIndex + 1;
                        if (newIndex >= MainMenuItems.Count)
                            newIndex = 0;
                        SelectedItem = MainMenuItems[newIndex];
                        break;
                    case ECurrentMenu.HandgunsMenu:
                        if (!HandgunWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = HandgunWeaponItems[0];
                            break;
                        }

                        currentIndex = HandgunWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex + 1;
                        if (newIndex >= HandgunWeaponItems.Count)
                            newIndex = 0;
                        SelectedItem = HandgunWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.LongGunsMenu:
                        if (!LongGunsWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = LongGunsWeaponItems[0];
                            break;
                        }

                        currentIndex = LongGunsWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex + 1;
                        if (newIndex >= LongGunsWeaponItems.Count)
                            newIndex = 0;
                        SelectedItem = LongGunsWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.ThrowablesMenu:
                        if (!ThrowableWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = ThrowableWeaponItems[0];
                            break;
                        }

                        currentIndex = ThrowableWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex + 1;
                        if (newIndex >= ThrowableWeaponItems.Count)
                            newIndex = 0;
                        SelectedItem = ThrowableWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.MiscMenu:
                        if (!MiscWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = MiscWeaponItems[0];
                            break;
                        }

                        currentIndex = MiscWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex + 1;
                        if (newIndex >= MiscWeaponItems.Count)
                            newIndex = 0;
                        SelectedItem = MiscWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.PredefinedLoadoutsMenu:
                        if (!PredefinedLoadoutItems.Contains(SelectedItem))
                        {
                            SelectedItem = PredefinedLoadoutItems[0];
                            break;
                        }

                        currentIndex = PredefinedLoadoutItems.IndexOf((LoadoutItem)SelectedItem);
                        newIndex = currentIndex + 1;
                        if (newIndex >= PredefinedLoadoutItems.Count)
                            newIndex = 0;
                        SelectedItem = PredefinedLoadoutItems[newIndex];
                        break;
                }

                //ScrollBarInstance.Scroll(SelectedItem.BackgroundRectangle.RectangleF.Y - ScrollBarInstance.Value);
                if (SelectedItem.BackgroundRectangle.RectangleF.Y + TEXTURE_HEIGHT > Game.Resolution.Height)
                {
                    ScrollBarInstance.Scroll((SelectedItem.BackgroundRectangle.RectangleF.Y + TEXTURE_HEIGHT) * 0.75f);
                }
                else if (SelectedItem.BackgroundRectangle.RectangleF.Y < 0.0f)
                {
                    ScrollBarInstance.Scroll((SelectedItem.BackgroundRectangle.RectangleF.Y) * 1.25f);
                }
            }

            private void InputGoUp()
            {
                int currentIndex, newIndex;

                SoundInstance.PlayFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
                switch (CurrentMenu)
                {
                    case ECurrentMenu.MainMenu:
                        if (!MainMenuItems.Contains(SelectedItem))
                        {
                            SelectedItem = MainMenuItems[0];
                            break;
                        }

                        currentIndex = MainMenuItems.IndexOf((MenuItem)SelectedItem);
                        newIndex = currentIndex - 1;
                        if (newIndex < 0)
                            newIndex = MainMenuItems.Count - 1;
                        SelectedItem = MainMenuItems[newIndex];
                        break;
                    case ECurrentMenu.HandgunsMenu:
                        if (!HandgunWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = HandgunWeaponItems[0];
                            break;
                        }

                        currentIndex = HandgunWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex - 1;
                        
                        if (newIndex < 0)
                            newIndex = HandgunWeaponItems.Count - 1;

                        SelectedItem = HandgunWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.LongGunsMenu:
                        if (!LongGunsWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = LongGunsWeaponItems[0];
                            break;
                        }

                        currentIndex = LongGunsWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex - 1;
                        if (newIndex < 0)
                            newIndex = LongGunsWeaponItems.Count - 1;
                        SelectedItem = LongGunsWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.ThrowablesMenu:
                        if (!ThrowableWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = ThrowableWeaponItems[0];
                            break;
                        }

                        currentIndex = ThrowableWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex - 1;
                        if (newIndex < 0)
                            newIndex = ThrowableWeaponItems.Count - 1;
                        SelectedItem = ThrowableWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.MiscMenu:
                        if (!MiscWeaponItems.Contains(SelectedItem))
                        {
                            SelectedItem = MiscWeaponItems[0];
                            break;
                        }

                        currentIndex = MiscWeaponItems.IndexOf((WeaponItem)SelectedItem);
                        newIndex = currentIndex - 1;
                        if (newIndex < 0)
                            newIndex = MiscWeaponItems.Count - 1;
                        SelectedItem = MiscWeaponItems[newIndex];
                        break;
                    case ECurrentMenu.PredefinedLoadoutsMenu:
                        if (!PredefinedLoadoutItems.Contains(SelectedItem))
                        {
                            SelectedItem = PredefinedLoadoutItems[0];
                            break;
                        }

                        currentIndex = PredefinedLoadoutItems.IndexOf((LoadoutItem)SelectedItem);
                        newIndex = currentIndex - 1;
                        if (newIndex < 0)
                            newIndex = PredefinedLoadoutItems.Count - 1;
                        SelectedItem = PredefinedLoadoutItems[newIndex];
                        break;
                }

                //ScrollBarInstance.Scroll(SelectedItem.BackgroundRectangle.RectangleF.Y - ScrollBarInstance.Value);
                if (SelectedItem.BackgroundRectangle.RectangleF.Y + TEXTURE_HEIGHT > Game.Resolution.Height)
                {
                    ScrollBarInstance.Scroll((SelectedItem.BackgroundRectangle.RectangleF.Y + TEXTURE_HEIGHT) * 0.75f);
                }
                else if (SelectedItem.BackgroundRectangle.RectangleF.Y < 0.0f)
                {
                    ScrollBarInstance.Scroll((SelectedItem.BackgroundRectangle.RectangleF.Y) * 1.25f);
                }
            }

            private void InputSelect()
            {
                SoundInstance.PlayFrontend(AUDIO_SELECT, AUDIO_LIBRARY);
                SelectedItem?.BackgroundRectangle.RaiseClicked();
            }

            private void invokeLoadoutItemSelected(LoadoutItem item)
            {
                if (ScrollBarInstance.IsClicked)
                    return;

                if (LoadoutItemSelected != null)
                    LoadoutItemSelected(item);
            }

            private void invokeWeaponItemSelected(WeaponItem item)
            {
                if (ScrollBarInstance.IsClicked)
                    return;

                if (WeaponItemSelected != null)
                    WeaponItemSelected(item);
            }


            private void calculateItemsPosition()   
            {
                float resWidth = Game.Resolution.Width;
                float resHeight = Game.Resolution.Height;

                float x = resWidth - 600f;
                float y = 0f;
                float height = resHeight / 5;

                const float RECTANGLE_WIDTH = 584.0f;
                const float MAIN_MENU_RECT_WIDTH = RECTANGLE_WIDTH + 27.0f;


                ScrollBarInstance.BackgroundRectangle.RectangleF = new RectangleF(resWidth - 15f, 0, 20f, resHeight);
                ScrollBarInstance.Rectangle.RectangleF = new RectangleF(resWidth - 15f, ScrollBarInstance.Value, 20f, 50f);


                float mainMenuYAddition = (resHeight / 6) - TEXTURE_HEIGHT;

                y = mainMenuYAddition * 1;
                HandgunsItem.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH, TEXTURE_HEIGHT);
                HandgunsItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, MAIN_MENU_RECT_WIDTH, TEXTURE_HEIGHT);
                HandgunsItem.Label.Position = new PointF(x + TEXTURE_WIDTH, y + TEXTURE_HEIGHT * 0.25f);
                y = mainMenuYAddition * 2;
                RifleItem.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH, TEXTURE_HEIGHT);
                RifleItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, MAIN_MENU_RECT_WIDTH, TEXTURE_HEIGHT);
                RifleItem.Label.Position = new PointF(x + TEXTURE_WIDTH, y + TEXTURE_HEIGHT * 0.25f);
                y = mainMenuYAddition * 3;
                ThrowableItem.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH, TEXTURE_HEIGHT);
                ThrowableItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, MAIN_MENU_RECT_WIDTH, TEXTURE_HEIGHT);
                ThrowableItem.Label.Position = new PointF(x + TEXTURE_WIDTH, y + TEXTURE_HEIGHT * 0.25f);
                y = mainMenuYAddition * 4;
                MiscItem.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH, TEXTURE_HEIGHT);
                MiscItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, MAIN_MENU_RECT_WIDTH, TEXTURE_HEIGHT);
                MiscItem.Label.Position = new PointF(x + TEXTURE_WIDTH, y + TEXTURE_HEIGHT * 0.25f);
                y = mainMenuYAddition * 5;
                PredefinedLoadoutItem.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH, TEXTURE_HEIGHT);
                PredefinedLoadoutItem.BackgroundRectangle.RectangleF = new RectangleF(x, y, MAIN_MENU_RECT_WIDTH, TEXTURE_HEIGHT);
                PredefinedLoadoutItem.Label.Position = new PointF(x + TEXTURE_WIDTH, y + TEXTURE_HEIGHT * 0.25f);



                x = resWidth - 600f;
                y = -ScrollBarInstance.Value;

                foreach (WeaponItem item in HandgunWeaponItems)
                {
                    item.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH /* * HANDGUNS_TEXTURES_MULTIPLIER */, TEXTURE_HEIGHT /* * HANDGUNS_TEXTURES_MULTIPLIER */);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, RECTANGLE_WIDTH, TEXTURE_HEIGHT /* * HANDGUNS_TEXTURES_MULTIPLIER */);
                    item.Label.Position = new PointF(x + TEXTURE_WIDTH /* * HANDGUNS_TEXTURES_MULTIPLIER */, y + TEXTURE_HEIGHT /* * HANDGUNS_TEXTURES_MULTIPLIER */ * 0.25f);
                    y += TEXTURE_HEIGHT /* * HANDGUNS_TEXTURES_MULTIPLIER */;
                }


                x = resWidth - 600f;
                y = -ScrollBarInstance.Value;

                foreach (WeaponItem item in LongGunsWeaponItems)
                {
                    item.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH /* * LONG_GUNS_TEXTURES_MULTIPLIER*/, TEXTURE_HEIGHT /* * LONG_GUNS_TEXTURES_MULTIPLIER*/);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, RECTANGLE_WIDTH, TEXTURE_HEIGHT /* * LONG_GUNS_TEXTURES_MULTIPLIER*/);
                    item.Label.Position = new PointF(x + TEXTURE_WIDTH /* * LONG_GUNS_TEXTURES_MULTIPLIER*/, y + TEXTURE_HEIGHT /* * LONG_GUNS_TEXTURES_MULTIPLIER*/ * 0.25f);
                    y += TEXTURE_HEIGHT /* * LONG_GUNS_TEXTURES_MULTIPLIER*/;
                }


                x = resWidth - 600f;
                y = -ScrollBarInstance.Value;

                foreach (WeaponItem item in ThrowableWeaponItems)
                {
                    item.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH /* * THROWABLES_TEXTURES_MULTIPLIER */, TEXTURE_HEIGHT /* * THROWABLES_TEXTURES_MULTIPLIER */);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, RECTANGLE_WIDTH, TEXTURE_HEIGHT /* * THROWABLES_TEXTURES_MULTIPLIER */);
                    item.Label.Position = new PointF(x + TEXTURE_WIDTH /* * THROWABLES_TEXTURES_MULTIPLIER */, y + TEXTURE_HEIGHT /* * THROWABLES_TEXTURES_MULTIPLIER */ * 0.25f);
                    y += TEXTURE_HEIGHT /* * THROWABLES_TEXTURES_MULTIPLIER */;
                }


                x = resWidth - 600f;
                y = -ScrollBarInstance.Value;

                foreach (WeaponItem item in MiscWeaponItems)
                {
                    item.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH /* * MISC_ITEMS_TEXTURES_MULTIPLIER*/, TEXTURE_HEIGHT /* * MISC_ITEMS_TEXTURES_MULTIPLIER*/);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, RECTANGLE_WIDTH, TEXTURE_HEIGHT /* * MISC_ITEMS_TEXTURES_MULTIPLIER*/);
                    item.Label.Position = new PointF(x + TEXTURE_WIDTH /* * MISC_ITEMS_TEXTURES_MULTIPLIER*/, y + TEXTURE_HEIGHT /* * MISC_ITEMS_TEXTURES_MULTIPLIER*/ * 0.25f);
                    y += TEXTURE_HEIGHT /* * MISC_ITEMS_TEXTURES_MULTIPLIER*/;
                }


                x = resWidth - 600f;
                y = -ScrollBarInstance.Value;

                foreach (LoadoutItem item in PredefinedLoadoutItems)
                {
                    item.Texture.RectangleF = new RectangleF(x, y, TEXTURE_WIDTH /* * LOADOUTS_TEXTURES_MULTIPLIER*/, TEXTURE_HEIGHT /* * LOADOUTS_TEXTURES_MULTIPLIER*/);
                    item.BackgroundRectangle.RectangleF = new RectangleF(x, y, RECTANGLE_WIDTH, TEXTURE_HEIGHT /* * LOADOUTS_TEXTURES_MULTIPLIER*/);
                    item.Label.Position = new PointF(x + TEXTURE_WIDTH /* * LOADOUTS_TEXTURES_MULTIPLIER*/, y + TEXTURE_HEIGHT /* * LOADOUTS_TEXTURES_MULTIPLIER*/ * 0.25f);
                    y += TEXTURE_HEIGHT /* * LOADOUTS_TEXTURES_MULTIPLIER*/;
                }
                
                const int timesToUpdate = 3;

                for (int i = 0; i < timesToUpdate; i++)
                {
                    HandgunsItem.Process();             // update items position before display
                    RifleItem.Process();
                    ThrowableItem.Process();
                    MiscItem.Process();
                    PredefinedLoadoutItem.Process();

                    foreach (WeaponItem item in HandgunWeaponItems)
                        item.Process();
                    foreach (WeaponItem item in LongGunsWeaponItems)
                        item.Process();
                    foreach (WeaponItem item in ThrowableWeaponItems)
                        item.Process();
                    foreach (WeaponItem item in MiscWeaponItems)
                        item.Process();
                    foreach (LoadoutItem item in PredefinedLoadoutItems)
                        item.Process();
                }
            }

            private void hideAll()
            {
                foreach (WeaponItem item in HandgunWeaponItems)
                    item.State = UIState.Hidden;
                foreach (WeaponItem item in LongGunsWeaponItems)
                    item.State = UIState.Hidden;
                foreach (WeaponItem item in ThrowableWeaponItems)
                    item.State = UIState.Hidden;
                foreach (WeaponItem item in MiscWeaponItems)
                    item.State = UIState.Hidden;
                foreach (LoadoutItem item in PredefinedLoadoutItems)
                    item.State = UIState.Hidden;

                HandgunsItem.State = UIState.Hidden;
                RifleItem.State = UIState.Hidden;
                ThrowableItem.State = UIState.Hidden;
                MiscItem.State = UIState.Hidden;
                PredefinedLoadoutItem.State = UIState.Hidden;
            }

            public enum ECurrentMenu
            {
                MainMenu,
                HandgunsMenu,
                LongGunsMenu,
                ThrowablesMenu,
                MiscMenu,                   
                PredefinedLoadoutsMenu,
            }

            public sealed class ScrollBar
            {
                public float MaxScrollBarValue { get { return Game.Resolution.Height - 50.0f; } } 
                public float MinScrollBarValue { get { return 0.0f; } }

                public float Value { get; private set; }

                public UIRectangle BackgroundRectangle { get; }
                public UIRectangle Rectangle { get; }

                bool _isClicked = false;
                public bool IsClicked { get { return _isClicked; } }

                public event EventHandler ValueChanged;

                private bool _visible;
                public bool Visible
                {
                    get
                    {
                        return _visible;
                    }
                    set
                    {
                        _visible = value;
                        BackgroundRectangle.State = _visible ? UIState.ComingIntoView : UIState.Hiding;
                        Rectangle.State = _visible ? UIState.ComingIntoView : UIState.Hiding;
                    }
                }

                public ScrollBar()
                {
                    Value = 0.0f;
                    BackgroundRectangle = new UIRectangle(new RectangleF(Game.Resolution.Width - 15f, 0, 20f, Game.Resolution.Height), ScrollBarBackgroundColor, UIRectangleType.Filled, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Rectangle = new UIRectangle(new RectangleF(Game.Resolution.Width - 15f, 0, 20f, 50f), ScrollBarColor, UIRectangleType.Filled, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Rectangle.Clicked += rectClicked;
                }


                private void rectClicked(UIElementBase sender)
                {
                    _isClicked = true;
                    float yRect = Rectangle.RectangleF.Y;
                    float yMouse = UICommon.GetCursorPosition().Y;
                    yOffset = yRect - yMouse;
                }

                float previousScrollBarValue = 0.0f;
                float yOffset = 0.0f;
                public void Process()
                {
                    if (_isClicked)
                    {
                        MouseState mouseState = Game.GetMouseState();


                        float newYValue = mouseState.Y + yOffset > MaxScrollBarValue ? MaxScrollBarValue :
                                          mouseState.Y + yOffset < MinScrollBarValue ? MinScrollBarValue :
                                          mouseState.Y + yOffset;

                        SetValue(newYValue);

                        if (!mouseState.IsLeftButtonDown)
                        {
                            yOffset = 0.0f;
                            _isClicked = false;
                        }
                    }
                    else
                    {
                        MouseState mouseState = Game.GetMouseState();
                        float delta = -mouseState.MouseWheelDelta;
                        const float multiplier = 9.225f;

                        float newYValue = Rectangle.RectangleF.Y + delta * multiplier > MaxScrollBarValue ? MaxScrollBarValue :
                                          Rectangle.RectangleF.Y + delta * multiplier < MinScrollBarValue ? MinScrollBarValue :
                                          Rectangle.RectangleF.Y + delta * multiplier;

                        SetValue(newYValue);
                    }

                    if (lerping)
                    {
                        float newLerpValue = MathHelper.Lerp(initScrollValue, toScroll, lerpPercent);
                        lerpPercent += 0.05f;

                        float newYValue = newLerpValue > MaxScrollBarValue ? MaxScrollBarValue :
                                          newLerpValue < MinScrollBarValue ? MinScrollBarValue :
                                          newLerpValue;

                        SetValue(newYValue);

                        if (lerpPercent >= 1.0f)
                        {
                            initScrollValue = 0.0f;
                            toScroll = 0.0f;
                            lerpPercent = 0.0f;
                            lerping = false;
                        }
                    }

                    BackgroundRectangle.Process();
                    Rectangle.Process();
                }

                public void Draw(Rage.Graphics g)
                {
                    if (!Visible)
                        return;

                    BackgroundRectangle.Draw(g);
                    Rectangle.Draw(g);
                }

                bool lerping = false;

                float lerpPercent = 0.0f;
                float initScrollValue = 0.0f;
                float toScroll = 0.0f;
                public void Scroll(float valueToScroll)
                {
                    if (valueToScroll == 0.0f)
                        return;

                    initScrollValue = Value;
                    toScroll = Value + valueToScroll;
                    lerpPercent = 0.0f;
                    lerping = true;
                }

                public void SetValue(float value)
                {
                    previousScrollBarValue = Value;

                    Rectangle.RectangleF = new RectangleF(Rectangle.RectangleF.X, value, Rectangle.RectangleF.Width, Rectangle.RectangleF.Height);
                    Value = value;

                    if (Value != previousScrollBarValue)
                        ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            

            public abstract class Item
            {
                public abstract bool Selected { get; set; }
                public abstract bool Hovered { get; }
                public abstract void Process();
                public abstract void Draw(Rage.Graphics g);
                public abstract UIRectangle BackgroundRectangle { get; }
            }

            public class MenuItem : Item
            {
                public UITexture Texture { get; }
                public UILabel Label { get; }
                public override UIRectangle BackgroundRectangle { get; }

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

                public override bool Hovered
                {
                    get
                    {
                        return BackgroundRectangle.MouseState == UIMouseState.ElementHovered;
                    }
                }

                public override bool Selected { get; set; }

                public MenuItem(string label, Rage.Texture texture)
                {
                    if (texture == null)
                        texture = MissingTexture;
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(label, "Arial", 22.5f, new PointF(), LabelTextColor, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), BackgroundRectangleColor, BorderRectangleColor, /*UIRectangleType.FilledWithBorders*/UIRectangleType.Filled, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    //BackgroundRectangle.Hovered += backRectHoveredEvent;
                }

                public override void Process()
                {
                    BackgroundRectangle.Color = Selected ? SelectedBackgroundRectangleColor : Hovered ? HoveredBackgroundRectangleColor : BackgroundRectangleColor;

                    BackgroundRectangle.Process();
                    Label.Process();
                    Texture.Process();
                }

                public override void Draw(Rage.Graphics g)
                {
                    BackgroundRectangle.Draw(g);
                    Label.Draw(g);
                    Texture.Draw(g);
                }
                
                //private void backRectHoveredEvent(UIElementBase sender)
                //{
                //    BackgroundRectangle.Color = HoveredBackgroundRectangleColor;
                //}
            }
            public class WeaponItem : Item
            {
                public ItemType Type { get; }
                public EWeaponHash WeaponHash { get; }
                public MiscItems MiscItem { get; }
                public UITexture Texture { get; }
                public UILabel Label { get; }
                public override UIRectangle BackgroundRectangle { get; }

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

                public override bool Hovered
                {
                    get
                    {
                        return BackgroundRectangle.MouseState == UIMouseState.ElementHovered;
                    }
                }

                public override bool Selected { get; set; }

                public WeaponItem(EWeaponHash hash, ItemType type, Rage.Texture texture)
                {
                    Type = type;
                    WeaponHash = hash;
                    if (texture == null)
                        texture = MissingTexture;
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(hash.ToString().Replace('_', ' '), "Arial", 22.5f, new PointF(), LabelTextColor, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), BackgroundRectangleColor, BorderRectangleColor, UIRectangleType.Filled, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    //BackgroundRectangle.Hovered += backRectHoveredEvent;
                    Logger.LogDebug("Created new WeaponItem - Hash: " + hash + "  Type: " + type);
                }

                public WeaponItem(MiscItems item, Rage.Texture texture)
                {
                    Type = ItemType.Misc;
                    WeaponHash = 0;
                    MiscItem = item;
                    if (texture == null)
                        texture = MissingTexture;
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(item.ToString().Replace('_', ' '), "Arial", 22.5f, new PointF(), LabelTextColor, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), BackgroundRectangleColor, BorderRectangleColor, UIRectangleType.Filled, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    //BackgroundRectangle.Hovered += backRectHoveredEvent;
                    Logger.LogDebug("Created new WeaponItem - MiscItems: " + MiscItem + "  Type: " + Type);
                }

                public override void Process()
                {
                    BackgroundRectangle.Color = Selected ? SelectedBackgroundRectangleColor : Hovered ? HoveredBackgroundRectangleColor : BackgroundRectangleColor;
                    BackgroundRectangle.Process();
                    Label.Process();
                    Texture.Process();

                }

                public override void Draw(Rage.Graphics g)
                {
                    BackgroundRectangle.Draw(g);
                    Label.Draw(g);
                    Texture.Draw(g);
                }


                //private void backRectHoveredEvent(UIElementBase sender)
                //{
                //    BackgroundRectangle.Color = HoveredBackgroundRectangleColor;
                //}

                public static WeaponItem GetWeaponItemForWeapon(EWeaponHash hash, ItemType type)
                {
                    Rage.Texture texture = Game.CreateTextureFromFile(UI_FOLDER + hash + ".png");
                    return new WeaponItem(hash, type, texture);
                }
                public static WeaponItem GetWeaponItemForMiscItem(MiscItems item)
                {
                    Rage.Texture texture = Game.CreateTextureFromFile(UI_FOLDER + item + ".png");
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
                        // pistols  
                        EWeaponHash.Pistol,
                        EWeaponHash.Combat_Pistol,
                        EWeaponHash.AP_Pistol,
                        EWeaponHash.Pistol_50,
                        EWeaponHash.Heavy_Pistol,
                        EWeaponHash.Heavy_Revolver,
                        EWeaponHash.Machine_Pistol,
                        EWeaponHash.Flare_Gun,
                        EWeaponHash.Stun_Gun,              

                        // submachines  
                        EWeaponHash.Micro_SMG,
                    };
                }

                public static EWeaponHash[] GetAvalaibleLongWeapons()
                {
                    return new EWeaponHash[]
                    {
                        // submachines
                        EWeaponHash.SMG,
                        EWeaponHash.Assault_SMG,
                        EWeaponHash.Combat_PDW,

                        // rifles
                        EWeaponHash.Assault_Rifle,
                        EWeaponHash.Carbine_Rifle,
                        EWeaponHash.Advanced_Rifle,
                        EWeaponHash.Bullpup_Rifle,         
                        EWeaponHash.Compact_Rifle,
                        EWeaponHash.Special_Carbine,

                        //mgs
                        EWeaponHash.MG,
                        EWeaponHash.Combat_MG,             

                        // shotguns  
                        EWeaponHash.Pump_Shotgun,
                        EWeaponHash.Sawn_Off_Shotgun,
                        EWeaponHash.Assault_Shotgun,
                        EWeaponHash.Bullpup_Shotgun,
                        EWeaponHash.Heavy_Shotgun,                
                        EWeaponHash.Double_Barrel_Shotgun,

                        // snipers  
                        EWeaponHash.Sniper_Rifle,
                        EWeaponHash.Heavy_Sniper,
                        EWeaponHash.Marksman_Rifle,        
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
                        MiscItems.Flashlight,
                        MiscItems.Refill_Ammo,  
                    };
                }
            }
            public class LoadoutItem : Item
            {
                public UITexture Texture { get; }
                public UILabel Label { get; }
                public override UIRectangle BackgroundRectangle { get; }
                public Loadout Loadout { get; }

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

                public override bool Hovered
                {
                    get
                    {
                        return BackgroundRectangle.MouseState == UIMouseState.ElementHovered;
                    }
                }

                public override bool Selected { get; set; }

                public LoadoutItem(Loadout loadout)
                {
                    Rage.Texture texture = Game.CreateTextureFromFile(UI_FOLDER + loadout.TextureFileName + ".png");
                    if(texture == null)
                        texture = MissingTexture;

                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 0.0225f, 0.04725f);
                    Label = new UILabel(loadout.Name, "Arial", 22.5f, new PointF(), LabelTextColor, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle = new UIRectangle(new RectangleF(), BackgroundRectangleColor, BorderRectangleColor, UIRectangleType.Filled, UIScreenBorder.Right, 0.0225f, 0.04725f);
                    BackgroundRectangle.Hovered += backRectHoveredEvent;
                    Loadout = loadout;
                }

                public override void Process()
                {
                    BackgroundRectangle.Color = Selected ? SelectedBackgroundRectangleColor : Hovered ? HoveredBackgroundRectangleColor : BackgroundRectangleColor;
                    HelpText.State = UI.UIState.Hidden;
                    HelpRectangle.State = UI.UIState.Hidden;
                    BackgroundRectangle.Process();
                    Label.Process();
                    Texture.Process();
                    HelpRectangle.Process();
                    HelpText.Process();
                }

                public override void Draw(Rage.Graphics g)
                {
                    BackgroundRectangle.Draw(g);
                    Label.Draw(g);
                    Texture.Draw(g);
                }

                public void DrawHelpText(Rage.Graphics g)
                {
                    HelpRectangle.Draw(g);
                    HelpText.Draw(g);
                }


                private UILabel HelpText = new UI.UILabel("", "", 13f, PointF.Empty, HelpTextLabelTextColor, UI.UIScreenBorder.Top, 0.035f, 0.0475f);
                private UIRectangle HelpRectangle = new UI.UIRectangle(new RectangleF(), HelpTextBackgroundRectangleColor, UI.UIRectangleType.Filled, UI.UIScreenBorder.Top, 0.035f, 0.0475f);
                
                private void backRectHoveredEvent(UIElementBase sender)
                {
                    Vector2 mousePos = UI.UICommon.GetCursorPosition();
                    HelpText.Text = Loadout.Description;
                    HelpText.Position = new PointF(mousePos.X + 0.5f, mousePos.Y + 30f);
                    HelpRectangle.RectangleF = new RectangleF(new PointF(HelpText.Position.X - 0.15f, HelpText.Position.Y), HelpText.Measure() + new SizeF(11f, 7.585f));
                    if (HelpText.State != UI.UIState.Showing)
                        HelpText.State = UI.UIState.Showing;
                    if (HelpRectangle.State != UI.UIState.Showing)
                        HelpRectangle.State = UI.UIState.Showing;
                }
            }
        }

        protected enum MiscItems : uint
        {
            Bulletproof_Vest,
            Refill_Ammo,
            Nightstick = EWeaponHash.Nightstick,
            Fire_Extinguisher = EWeaponHash.Fire_Extinguisher,
            Flashlight = EWeaponHash.Flashlight,
        }

        protected enum ItemType
        {
            Handgun,
            LongGun,
            Throwable,
            Misc,      
        }
    }


    [Serializable]
    public class Item
    {
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

        [XmlElement]
        public ItemType Type;
        [XmlElement]
        public WeaponComponent[] Components;
    }

    [Serializable]
    public class Loadout
    {
        [XmlElement]
        public List<Item> Items;
        [XmlElement]
        public string TextureFileName;
        [XmlElement]
        public string Name;
        
        [XmlIgnore]
        private string _description;
        [XmlIgnore]
        public string Description
        {
            get
            {
                if (String.IsNullOrEmpty(_description))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Item item in Items)
                    {
                        sb.AppendLine("-" + item.Type);
                        foreach (WeaponComponent comp in item.Components)
                        {
                            sb.AppendLine("  -" + comp);
                        }
                        sb.AppendLine();
                    }

                    return _description = sb.ToString();
                }
                return _description;
            }
            private set
            {
                _description = value;
            }
        }

        public Loadout()
        {
            Items = new List<Item>();
        }

        public void GiveToPed(Ped ped)
        {
            foreach (Item item in Items)
            {
                WeaponAsset wAsset = new WeaponAsset((uint)item.Type);
                if (wAsset.IsValid)
                {
                    ped.Inventory.GiveNewWeapon(wAsset, 999, false);
                    foreach (WeaponComponent comp in item.Components)
                    {
                        ped.Inventory.AddComponentToWeapon(wAsset, comp.ToString());
                    }
                }
                else
                {
                    switch (item.Type)
                    {
                        case Item.ItemType.Bulletproof_Vest:
                            ped.Armor = 250;
                            break;
                    }
                }
            }
        }

        public static Loadout GetFromXML(string xmlFilePath)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(Loadout));
            // If the XML document has been altered with unknown 
            // nodes or attributes, handles them with the 
            // UnknownNode and UnknownAttribute events.
            //serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            //serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(xmlFilePath, FileMode.Open);
            // Declares an object variable of the type to be deserialized.
            Loadout loadout;
            // Uses the Deserialize method to restore the object's state 
            // with data from the XML document. */
            loadout = (Loadout)serializer.Deserialize(fs);
            foreach (Item item in loadout.Items)
            {
                if (item.Components == null)
                {
                    item.Components = new WeaponComponent[] { };
                }
            }
#if DEBUG
            Logger.LogTrivial("Loaded Loadout from " + xmlFilePath);
            Logger.LogTrivial("  -Name:               " + loadout.Name);
            Logger.LogTrivial("  -Texture Filename:   " + loadout.TextureFileName);
            StringBuilder sb = new StringBuilder("  -Items:      ");
            foreach (Item item in loadout.Items)
            {
                sb.Append(item.Type + "(");
                foreach (WeaponComponent c in item.Components)
                    sb.Append(c + ",");
                sb.Append(")" + ",");
            }
            Logger.LogTrivial(sb);
#endif
            return loadout;
        }


        public static void WriteToXML(string xmlFilePath, Loadout loadout)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(Loadout));
            TextWriter writer = new StreamWriter(xmlFilePath);

            // Serializes the purchase order, and closes the TextWriter.
            serializer.Serialize(writer, loadout);
            writer.Close();
            writer.Dispose();
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