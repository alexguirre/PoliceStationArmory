//namespace PoliceStationArmory.UI
//{
//    using Rage;
//    using Rage.Native;
//    using System;
//    using System.Drawing;

//    internal class Scaleform
//    {
//        private int _handle;
//        private string _scaleformID;

//        public int Handle { get { return _handle; } }

//        public Scaleform()
//        {
//        }

//        public Scaleform(int handle)
//        {
//            this._handle = handle;
//        }

//        public bool Load(string scaleformID)
//        {
//            int handle = NativeFunction.CallByName<int>("REQUEST_SCALEFORM_MOVIE", scaleformID);

//            if (handle == 0) return false;

//            this._handle = handle;
//            this._scaleformID = scaleformID;

//            return true;
//        }


//        public void SetAsNoLongerNeeded()
//        {
//            unsafe
//            {
//                int handle = Handle;
//                NativeFunction.CallByName<uint>("SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED", new IntPtr(&handle));
//                _handle = 0;
//            }
//        }

//        public void Render2D()
//        {
//            const ulong DrawScaleformMovieDefault = 0x0df606929c105be1;
//            NativeFunction.CallByHash<uint>(DrawScaleformMovieDefault, this.Handle, 255, 255, 255, 255);
//        }
//        public void Render2DScreenSpace(PointF location, PointF size)
//        {
//            float x = location.X / 1280.0f;
//            float y = location.Y / 720.0f;
//            float width = size.X / 1280.0f;
//            float height = size.Y / 720.0f;

//            NativeFunction.CallByName<uint>("DRAW_SCALEFORM_MOVIE", this._handle, x + (width / 2.0f), y + (height / 2.0f), width, height, 255, 255, 255, 255);
//        }


//        public void Render3D(Vector3 position, Rotator rotation, Vector3 scale)
//        {
//            NativeFunction.CallByHash<uint>(0x1ce592fdc749d6f5, this._handle, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 2.0f, 2.0f, 1.0f, scale.X, scale.Y, scale.Z, 2);
//        }
//        public void Render3DAdditive(Vector3 position, Rotator rotation, Vector3 scale)
//        {
//            NativeFunction.CallByHash<uint>(0x87d51d72255d4e78, this._handle, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 2.0f, 2.0f, 1.0f, scale.X, scale.Y, scale.Z, 2);
//        }


//        public void CallFunction(string function, params object[] arguments)
//        {
//            NativeFunction.CallByHash<uint>(_PUSH_SCALEFORM_MOVIE_FUNCTION, this._handle, function);

//            foreach (object obj in arguments)
//            {
//                if (obj.GetType() == typeof(int))
//                {
//                    NativeFunction.CallByHash<uint>(_PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)obj);
//                }
//                else if (obj.GetType() == typeof(string))
//                {
//                    NativeFunction.CallByHash<uint>(_BEGIN_TEXT_COMPONENT, "STRING");
//                    NativeFunction.CallByHash<uint>(_ADD_TEXT_COMPONENT_STRING, (string)obj);
//                    NativeFunction.CallByHash<uint>(_END_TEXT_COMPONENT);
//                }
//                else if (obj.GetType() == typeof(char))
//                {
//                    NativeFunction.CallByHash<uint>(_BEGIN_TEXT_COMPONENT, "STRING");
//                    NativeFunction.CallByHash<uint>(_ADD_TEXT_COMPONENT_STRING, ((char)obj).ToString());
//                    NativeFunction.CallByHash<uint>(_END_TEXT_COMPONENT);
//                }
//                else if (obj.GetType() == typeof(float))
//                {
//                    NativeFunction.CallByHash<uint>(_PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT, (float)obj);
//                }
//                else if (obj.GetType() == typeof(double))
//                {
//                    NativeFunction.CallByHash<uint>(_PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT, (float)((double)obj));
//                }
//                else if (obj.GetType() == typeof(bool))
//                {
//                    NativeFunction.CallByHash<uint>(_PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL, (bool)obj);
//                }
//                else if (obj.GetType() == typeof(ScaleformArgumentTXD))
//                {
//                    NativeFunction.CallByHash<uint>(_PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING, ((ScaleformArgumentTXD)obj).TXD);
//                }
//                else
//                {
//                    Game.LogTrivial(String.Format("Unknown argument type {0} passed to scaleform with handle {1}.", obj.GetType().Name, this._handle));
//                }
//            }

//            NativeFunction.CallByHash<uint>(_POP_SCALEFORM_MOVIE_FUNCTION_VOID);
//        }



//        public const ulong _PUSH_SCALEFORM_MOVIE_FUNCTION = 0xf6e48914c7a8694e;
//        public const ulong _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT = 0xc3d0841a0cc546a6;
//        public const ulong _BEGIN_TEXT_COMPONENT = 0x80338406f3475e55;
//        public const ulong _ADD_TEXT_COMPONENT_STRING = 0x6c188be134e074aa;
//        public const ulong _END_TEXT_COMPONENT = 0x362e2d3fe93a9959;
//        public const ulong _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT = 0xd69736aae04db51a;
//        public const ulong _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL = 0xc58424ba936eb458;
//        public const ulong _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING = 0xba7148484bd90365;
//        public const ulong _POP_SCALEFORM_MOVIE_FUNCTION_VOID = 0xc6796a8ffa375e53;
//    }

//    internal class ScaleformArgumentTXD
//    {
//        private string _txd;
//        public string TXD { get { return this._txd; } }

//        public ScaleformArgumentTXD(string txd)
//        {
//            this._txd = txd;
//        }
//    }
//}
