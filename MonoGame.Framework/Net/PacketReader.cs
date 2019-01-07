#region License
// /*
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright © 2009 The MonoGame Team
// 
// All rights reserved.
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
// U.S. copyright law.
// 
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// */
#endregion License

#region Using clause
using System;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utilities.IO;
#endregion Using clause

namespace Microsoft.Xna.Framework.Net
{
    public class PacketReader : BinaryReader
    {
        // Read comments within the PacketWriter
        #region Constructors
        public PacketReader() : this(0)
        {
        }


        public PacketReader(int capacity) : base(RecyclableMemoryManager.Instance.GetMemoryStream(null, capacity))
        {

        }
        #endregion

        #region Methods
        internal byte[] Data
        {
            get
            {
                MemoryStream stream = (MemoryStream)this.BaseStream;
                return stream.GetBuffer();
            }
            set
            {
                MemoryStream ms = (MemoryStream)this.BaseStream;
                ms.Write(value, 0, value.Length);
            }
        }

        public Color ReadColor()
        {
            Color newColor = Color.Transparent;
            newColor.PackedValue = this.ReadUInt32();
            return newColor;
        }

        public override double ReadDouble()
        {
            return this.ReadDouble();
        }

        public Matrix ReadMatrix()
        {
            Matrix matrix = new Matrix
            {
                M11 = this.ReadSingle(),
                M12 = this.ReadSingle(),
                M13 = this.ReadSingle(),
                M14 = this.ReadSingle(),

                M21 = this.ReadSingle(),
                M22 = this.ReadSingle(),
                M23 = this.ReadSingle(),
                M24 = this.ReadSingle(),

                M31 = this.ReadSingle(),
                M32 = this.ReadSingle(),
                M33 = this.ReadSingle(),
                M34 = this.ReadSingle(),

                M41 = this.ReadSingle(),
                M42 = this.ReadSingle(),
                M43 = this.ReadSingle(),
                M44 = this.ReadSingle()
            };

            return matrix;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion quat = new Quaternion
            {
                X = this.ReadSingle(),
                Y = this.ReadSingle(),
                Z = this.ReadSingle(),
                W = this.ReadSingle()
            };

            return quat;

        }

        //		public override float ReadSingle()
        //		{
        //			return this.ReadSingle();
        //		}

        public Vector2 ReadVector2()
        {
            Vector2 vect = new Vector2
            {
                X = this.ReadSingle(),
                Y = this.ReadSingle()
            };

            return vect;
        }

        public Vector3 ReadVector3()
        {
            Vector3 vect = new Vector3
            {
                X = this.ReadSingle(),
                Y = this.ReadSingle(),
                Z = this.ReadSingle()
            };

            return vect;
        }

        public Vector4 ReadVector4()
        {
            Vector4 vect = new Vector4
            {
                X = this.ReadSingle(),
                Y = this.ReadSingle(),
                Z = this.ReadSingle(),
                W = this.ReadSingle()
            };

            return vect;
        }

        internal void Reset(int size)
        {
            MemoryStream ms = (MemoryStream)BaseStream;
            ms.SetLength(size);
            ms.Position = 0;
        }
        #endregion

        #region Properties
        public int Length => (int)BaseStream.Length;

        public int Position
        {
            get => (int)BaseStream.Position;
            set
            {
                if (BaseStream.Position != value)
                    BaseStream.Position = value;
            }
        }
        #endregion
    }
}