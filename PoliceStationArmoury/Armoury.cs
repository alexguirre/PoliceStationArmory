namespace PoliceStationArmoury
{
    // System
    using System.Drawing;
    using System.Windows.Forms;

    // RPH
    using Rage;

    // PoliceStationArmoury
    using PoliceStationArmoury.Types;
    using PoliceStationArmoury.UI;

    internal class Armoury
    {
        public Ped Cop { get; private set; }

        public Armoury()
        {
            Game.FrameRender += UIFrameRender;
            Game.RawFrameRender += UIRawFrameRender;
        }


        public void Update()
        {
            if (Cop.Exists())
            {
                if (Game.IsKeyDown(Keys.Y))
                {
                    Game.DisplayNotification("activated");
                    Cop.PlayAnimation(GiveHandgunAnimation, -1, 0.5f, 0.5f, 0.0f);
                }

                if (!Game.LocalPlayer.Character.IsInRangeOf2D(copSpawnPos.Position, 50f))
                {
                    Cop.Delete();
                }
            }
            else
            {
                if (Game.LocalPlayer.Character.IsInRangeOf2D(copSpawnPos.Position, 20f))
                {
                    Cop = GetOrCreateCop();
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

        public void UIFrameRender(object sender, GraphicsEventArgs e)
        {
            userInterface.Process();
        }

        public void UIRawFrameRender(object sender, GraphicsEventArgs e)
        {
            userInterface.Draw(e);
        }

        private UserInterface userInterface;

        private readonly Animation GiveAmmoAnimation = new Animation("mp_cop_armoury", "ammo_on_counter_cop");
        private readonly Animation GiveHandgunAnimation = new Animation("mp_cop_armoury", "pistol_on_counter_cop");
        private readonly Animation GiveRifleAnimation = new Animation("mp_cop_armoury", "rifle_on_counter_cop");
        private readonly Animation GivePackageAnimation = new Animation("mp_cop_armoury", "package_from_counter_cop");

        private readonly SpawnPoint copSpawnPos = new SpawnPoint(new Vector3(454.11f, -979.99f, 30.69f), 90f);


        protected class UserInterface
        {
            public delegate void WeaponItemSelected(WeaponItem selectedItem);


            public event WeaponItemSelected ItemSelected = delegate { };

            public UILabel label = new UILabel("Some text", "Times New Roman", 20f, new PointF(50f, 50f), Color.Red, UIScreenBorder.Left, 3f, 4.375f);

            public UserInterface()
            {

            }

            public void Process()
            {
                
            }

            public void Draw(GraphicsEventArgs e)
            {

            }


            private void invokeItemSelected(WeaponItem item)
            {
                if (ItemSelected != null)
                    ItemSelected(item);
            }


            public class WeaponItem
            {
                public EWeaponHash WeaponHash { get; }
                public UITexture Texture { get; }

                public WeaponItem(EWeaponHash hash, Rage.Texture texture)
                {
                    WeaponHash = hash;
                    Texture = new UITexture(texture, new RectangleF(), UIScreenBorder.Right, 2.85f, 3.825f);
                }

                public static WeaponItem GetWeaponItemForWeapon(EWeaponHash hash)
                {
                    Rage.Texture texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\" + hash);
                    return new WeaponItem(hash, texture);
                    //Rage.Texture texture = null;
                    //switch (hash)                           // TODO: complete textures switch statement and create the textures
                    //{
                    //    // melee      done textures except Flashlight
                    //    case EWeaponHash.Nightstick:            texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\" + hash); break;
                    //    case EWeaponHash.Flashlight:            texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // pistols    done textures
                    //    case EWeaponHash.Pistol:                texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Combat_Pistol:         texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.AP_Pistol:             texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Pistol_50:             texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Heavy_Pistol:          texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Stun_Gun:              texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // submachines  done textures
                    //    case EWeaponHash.Micro_SMG:             texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.SMG:                   texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Assault_SMG:           texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // rifles   done textures
                    //    case EWeaponHash.Assault_Rifle:         texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Carbine_Rifle:         texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Advanced_Rifle:        texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Bullpup_Rifle:         texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    //mgs     done textures
                    //    case EWeaponHash.MG:                    texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Combat_MG:             texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // shotguns    done textures except Heavy_Shotgun
                    //    case EWeaponHash.Pump_Shotgun:          texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Sawn_Off_Shotgun:      texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Assault_Shotgun:       texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Bullpup_Shotgun:       texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Heavy_Shotgun:         texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // snipers     done textures except Marksman_Rifle
                    //    case EWeaponHash.Sniper_Rifle:          texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Heavy_Sniper:          texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Marksman_Rifle:        texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // big guns
                    //    //case EWeaponHash.Grenade_Launcher:      texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    //case EWeaponHash.RPG:                   texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    //case EWeaponHash.Stinger:               texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    //case EWeaponHash.Minigun:               texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;

                    //    // throwables
                    //    case EWeaponHash.Grenade:               texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Sticky_Bomb:           texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Smoke_Grenade:         texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.BZ_Gas:                texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    //case EWeaponHash.Molotov:               texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    case EWeaponHash.Fire_Extinguisher:     texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    //case EWeaponHash.Petrol_Can:            texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    //case EWeaponHash.Flare:                 texture = Game.CreateTextureFromFile(@"Plugins\Police Station Armoury Resources\UI\"); break;
                    //    default:
                    //        return null;
                    //}
                    //return new WeaponItem(hash, texture);
                }

                public static EWeaponHash[] GetAvalaibleWeapons()
                {
                    return new EWeaponHash[] 
                    {
                        // melee     
                        EWeaponHash.Nightstick,
                        //EWeaponHash.Flashlight,   // doesn't have texture     

                        // pistols  
                        EWeaponHash.Pistol,                
                        EWeaponHash.Combat_Pistol,         
                        EWeaponHash.AP_Pistol,             
                        EWeaponHash.Pistol_50,             
                        EWeaponHash.Heavy_Pistol,          
                        EWeaponHash.Stun_Gun,              

                        // submachines  done textures
                        EWeaponHash.Micro_SMG,             
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

                        // snipers     done textures except Marksman_Rifle
                        EWeaponHash.Sniper_Rifle,          
                        EWeaponHash.Heavy_Sniper,          
                        //EWeaponHash.Marksman_Rifle,     // doesn't have texture          

                        // big guns
                        //EWeaponHash.Grenade_Launcher,      
                        //EWeaponHash.RPG,                   
                        //EWeaponHash.Stinger,               
                        //EWeaponHash.Minigun,               

                        // throwables
                        EWeaponHash.Grenade,               
                        EWeaponHash.Sticky_Bomb,           
                        EWeaponHash.Smoke_Grenade,         
                        //EWeaponHash.BZ_Gas,                // doesn't have texture        
                        //case EWeaponHash.Molotov,               
                        EWeaponHash.Fire_Extinguisher, 
                    };
                }
            }
        }
    }
}
