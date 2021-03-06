using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace MonoGame.Framework.Graphics
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class EffectParameter
    {
        /// <summary>
        /// The next state key used when an effect parameter
        /// is updated by any of the 'set' methods.
        /// </summary>
        internal static ulong NextStateKey { get; private set; }

        internal EffectParameter(
            EffectParameterClass @class,
            EffectParameterType type,
            string name,
            int rowCount,
            int columnCount,
            string semantic,
            EffectAnnotationCollection annotations,
            EffectParameterCollection elements,
            EffectParameterCollection structMembers,
            object? data)
        {
            ParameterClass = @class;
            ParameterType = type;

            Name = name;
            Semantic = semantic;
            Annotations = annotations;

            RowCount = rowCount;
            ColumnCount = columnCount;

            Elements = elements;
            StructureMembers = structMembers;

            Data = data;
            StateKey = unchecked(NextStateKey++);
        }

        internal EffectParameter(EffectParameter cloneSource)
        {
            // Share all the immutable types.
            ParameterClass = cloneSource.ParameterClass;
            ParameterType = cloneSource.ParameterType;
            Name = cloneSource.Name;
            Semantic = cloneSource.Semantic;
            Annotations = cloneSource.Annotations;
            RowCount = cloneSource.RowCount;
            ColumnCount = cloneSource.ColumnCount;

            // Clone the mutable types.
            Elements = cloneSource.Elements.Clone();
            StructureMembers = cloneSource.StructureMembers.Clone();

            // The data is mutable, so we have to clone it.
            if (cloneSource.Data is Array array)
                Data = array.Clone();

            StateKey = unchecked(NextStateKey++);
        }

        public string Name { get; private set; }
        public string Semantic { get; private set; }

        public EffectParameterClass ParameterClass { get; private set; }
        public EffectParameterType ParameterType { get; private set; }

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public EffectParameterCollection Elements { get; private set; }
        public EffectParameterCollection StructureMembers { get; private set; }
        public EffectAnnotationCollection Annotations { get; private set; }

        // TODO: Using object adds alot of boxing/unboxing overhead
        // and garbage generation.  We should consider a templated
        // type implementation to fix this!

        internal object? Data { get; private set; }

        /// <summary>
        /// The current state key which is used to detect
        /// if the parameter value has been changed.
        /// </summary>
        internal ulong StateKey { get; private set; }

        internal string DebuggerDisplay
        {
            get
            {
                string semanticStr = string.Empty;
                if (!string.IsNullOrEmpty(Semantic))
                    semanticStr = string.Concat("<", Semantic, "> ");

                return string.Concat(
                    "[", ParameterClass, " ", ParameterType, "]",
                    " ", semanticStr, Name, " : ", GetDataValueString());
            }
        }

        private string GetDataValueString()
        {
            string valueStr;

            if (Data == null)
            {
                if (Elements == null)
                    valueStr = "null";
                else
                    valueStr = string.Join(", ", Elements.Select(e => e.GetDataValueString()));
            }
            else
            {
                switch (ParameterClass)
                {
                    // Object types are stored directly in the Data property.
                    // Display Data's string value.
                    case EffectParameterClass.Object:
                        valueStr = Data.ToString();
                        break;

                    // Matrix types are stored in a float[16] which we don't really have room for.
                    // Display "...".
                    case EffectParameterClass.Matrix:
                        valueStr = "...";
                        break;

                    // Scalar types are stored as a float[1].
                    // Display the first (and only) element's string value.                    
                    case EffectParameterClass.Scalar:
                        valueStr = (Data as Array).GetValue(0).ToString();
                        break;

                    // Vector types are stored as an Array<Type>.
                    // Display the string value of each array element.
                    case EffectParameterClass.Vector:
                        var array = Data as Array;
                        var arrayStr = new string[array.Length];
                        var idx = 0;
                        foreach (var e in array)
                        {
                            arrayStr[idx] = array.GetValue(idx).ToString();
                            idx++;
                        }
                        valueStr = string.Join(" ", arrayStr);
                        break;

                    // Handle additional cases here...
                    default:
                        valueStr = Data.ToString();
                        break;
                }
            }

            return string.Concat("{", valueStr, "}");
        }

        public bool GetValueBoolean()
        {
            if (ParameterClass != EffectParameterClass.Scalar ||
                ParameterType != EffectParameterType.Bool)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes even booleans into a float.
            return ((float[])Data)[0] != 0f;
#else
            return ((int[])Data)[0] != 0;
#endif
        }

        /*
        public bool[] GetValueBooleanArray ()
        {
            throw new NotImplementedException();
        }
        */

        public int GetValueInt32()
        {
            if (ParameterClass != EffectParameterClass.Scalar ||
                ParameterType != EffectParameterType.Int32)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes integers into a float.
            return (int)((float[])Data)[0];
#else
            return ((int[])Data)[0];
#endif
        }

        public int[] GetValueInt32Array()
        {
            if (Elements != null && Elements.Count > 0)
            {
                var ret = new int[RowCount * ColumnCount * Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    var elmArray = Elements[i].GetValueInt32Array();

                    for (int j = 0; j < elmArray.Length; j++)
                        ret[RowCount * ColumnCount * i + j] = elmArray[j];
                }
                return ret;
            }

            switch (ParameterClass)
            {
                case EffectParameterClass.Scalar:
                    return new int[] { GetValueInt32() };

                default:
                    throw new NotImplementedException();
            }
        }

        public Matrix4x4 GetValueMatrix()
        {
            if (ParameterClass != EffectParameterClass.Matrix ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (RowCount != 4 || ColumnCount != 4)
                throw new InvalidCastException();

            var floatData = (float[])Data;
            return new Matrix4x4(
                floatData[0], floatData[4], floatData[8], floatData[12],
                floatData[1], floatData[5], floatData[9], floatData[13],
                floatData[2], floatData[6], floatData[10], floatData[14],
                floatData[3], floatData[7], floatData[11], floatData[15]);
        }

        public Matrix4x4[] GetValueMatrixArray(int count)
        {
            if (ParameterClass != EffectParameterClass.Matrix ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var ret = new Matrix4x4[count];
            for (var i = 0; i < count; i++)
                ret[i] = Elements[i].GetValueMatrix();

            return ret;
        }

        public Quaternion GetValueQuaternion()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var vecInfo = (float[])Data;
            return new Quaternion(vecInfo[0], vecInfo[1], vecInfo[2], vecInfo[3]);
        }

        public Quaternion[] GetValueQuaternionArray()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (Elements != null && Elements.Count > 0)
            {
                var result = new Quaternion[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    float[] v = Elements[i].GetValueSingleArray();
                    result[i] = new Quaternion(v[0], v[1], v[2], v[3]);
                }
                return result;
            }

            return null;
        }

        public float GetValueSingle()
        {
            // TODO: Should this fetch int and bool as a float?
            if (ParameterClass != EffectParameterClass.Scalar ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            return ((float[])Data)[0];
        }

        public float[] GetValueSingleArray()
        {
            if (Elements != null && Elements.Count > 0)
            {
                var ret = new float[RowCount * ColumnCount * Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    var elmArray = Elements[i].GetValueSingleArray();
                    for (var j = 0; j < elmArray.Length; j++)
                        ret[RowCount * ColumnCount * i + j] = elmArray[j];
                }
                return ret;
            }

            switch (ParameterClass)
            {
                case EffectParameterClass.Scalar:
                    return new float[] { GetValueSingle() };

                case EffectParameterClass.Vector:
                case EffectParameterClass.Matrix:
                    if (Data is Matrix4x4 matrix)
                    {
                        var array = new float[4 * 4];
                        matrix.CopyTo(array);
                        return array;
                    }
                    else
                    {
                        return (float[])Data;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public string GetValueString()
        {
            if (ParameterClass != EffectParameterClass.Object ||
                ParameterType != EffectParameterType.String)
                throw new InvalidCastException();

            return ((string[])Data)[0];
        }

        public Texture2D GetValueTexture2D()
        {
            if (ParameterClass != EffectParameterClass.Object ||
                ParameterType != EffectParameterType.Texture2D)
                throw new InvalidCastException();

            return (Texture2D)Data;
        }

        public Texture3D GetValueTexture3D()
        {
            if (ParameterClass != EffectParameterClass.Object ||
                ParameterType != EffectParameterType.Texture3D)
                throw new InvalidCastException();

            return (Texture3D)Data;
        }

        public TextureCube GetValueTextureCube()
        {
            if (ParameterClass != EffectParameterClass.Object ||
                ParameterType != EffectParameterType.TextureCube)
                throw new InvalidCastException();

            return (TextureCube)Data;
        }

        public Vector2 GetValueVector2()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            float[] vecInfo = (float[])Data;
            return new Vector2(vecInfo[0], vecInfo[1]);
        }

        public Vector2[] GetValueVector2Array()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (Elements != null && Elements.Count > 0)
            {
                var result = new Vector2[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    float[] v = Elements[i].GetValueSingleArray();
                    result[i] = new Vector2(v[0], v[1]);
                }
                return result;
            }

            return null;
        }

        public Vector3 GetValueVector3()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            float[] vecInfo = (float[])Data;
            return new Vector3(vecInfo[0], vecInfo[1], vecInfo[2]);
        }

        public Vector3[] GetValueVector3Array()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (Elements != null && Elements.Count > 0)
            {
                var result = new Vector3[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    float[] v = Elements[i].GetValueSingleArray();
                    result[i] = new Vector3(v[0], v[1], v[2]);
                }
                return result;
            }
            return null;
        }


        public Vector4 GetValueVector4()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            float[] vecInfo = (float[])Data;
            return new Vector4(vecInfo[0], vecInfo[1], vecInfo[2], vecInfo[3]);
        }

        public Vector4[] GetValueVector4Array()
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (Elements != null && Elements.Count > 0)
            {
                var result = new Vector4[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    float[] v = Elements[i].GetValueSingleArray();
                    result[i] = new Vector4(v[0], v[1], v[2], v[3]);
                }
                return result;
            }
            return null;
        }

        public void SetValue(bool value)
        {
            if (ParameterClass != EffectParameterClass.Scalar ||
                ParameterType != EffectParameterType.Bool)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes even booleans into a float.
            ((float[])Data)[0] = value ? 1 : 0;
#else
            ((int[])Data)[0] = value ? 1 : 0;
#endif

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<bool> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(int value)
        {
            if (ParameterClass != EffectParameterClass.Scalar ||
                ParameterType != EffectParameterType.Int32)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes integers into a float.
            ((float[])Data)[0] = value;
#else
            ((int[])Data)[0] = value;
#endif
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<int> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(in Matrix4x4 value)
        {
            if (ParameterClass != EffectParameterClass.Matrix ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;

            // HLSL expects matrices to be transposed by default.
            // These unrolled loops do the transpose during assignment.
            if (RowCount == 4 && ColumnCount == 4)
            {
                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;
                fData[3] = value.M41;

                fData[4] = value.M12;
                fData[5] = value.M22;
                fData[6] = value.M32;
                fData[7] = value.M42;

                fData[8] = value.M13;
                fData[9] = value.M23;
                fData[10] = value.M33;
                fData[11] = value.M43;

                fData[12] = value.M14;
                fData[13] = value.M24;
                fData[14] = value.M34;
                fData[15] = value.M44;
            }
            else if (RowCount == 4 && ColumnCount == 3)
            {
                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;
                fData[3] = value.M41;

                fData[4] = value.M12;
                fData[5] = value.M22;
                fData[6] = value.M32;
                fData[7] = value.M42;

                fData[8] = value.M13;
                fData[9] = value.M23;
                fData[10] = value.M33;
                fData[11] = value.M43;
            }
            else if (RowCount == 3 && ColumnCount == 4)
            {
                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;

                fData[3] = value.M12;
                fData[4] = value.M22;
                fData[5] = value.M32;

                fData[6] = value.M13;
                fData[7] = value.M23;
                fData[8] = value.M33;

                fData[9] = value.M14;
                fData[10] = value.M24;
                fData[11] = value.M34;
            }
            else if (RowCount == 3 && ColumnCount == 3)
            {
                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;

                fData[3] = value.M12;
                fData[4] = value.M22;
                fData[5] = value.M32;

                fData[6] = value.M13;
                fData[7] = value.M23;
                fData[8] = value.M33;
            }
            else if (RowCount == 3 && ColumnCount == 2)
            {
                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;

                fData[3] = value.M12;
                fData[4] = value.M22;
                fData[5] = value.M32;
            }

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<Matrix4x4> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValueTranspose(in Matrix4x4 value)
        {
            if (ParameterClass != EffectParameterClass.Matrix ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;

            // HLSL expects matrices to be transposed by default, so copying them straight
            // from the in-memory version effectively transposes them back to row-major.
            if (RowCount == 4 && ColumnCount == 4)
            {
                value.CopyTo(fData);
            }
            else if (RowCount == 4 && ColumnCount == 3)
            {
                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;

                fData[3] = value.M21;
                fData[4] = value.M22;
                fData[5] = value.M23;

                fData[6] = value.M31;
                fData[7] = value.M32;
                fData[8] = value.M33;

                fData[9] = value.M41;
                fData[10] = value.M42;
                fData[11] = value.M43;
            }
            else if (RowCount == 3 && ColumnCount == 4)
            {
                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;
                fData[3] = value.M14;

                fData[4] = value.M21;
                fData[5] = value.M22;
                fData[6] = value.M23;
                fData[7] = value.M24;

                fData[8] = value.M31;
                fData[9] = value.M32;
                fData[10] = value.M33;
                fData[11] = value.M34;
            }
            else if (RowCount == 3 && ColumnCount == 3)
            {
                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;

                fData[3] = value.M21;
                fData[4] = value.M22;
                fData[5] = value.M23;

                fData[6] = value.M31;
                fData[7] = value.M32;
                fData[8] = value.M33;
            }
            else if (RowCount == 3 && ColumnCount == 2)
            {
                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;

                fData[3] = value.M21;
                fData[4] = value.M22;
                fData[5] = value.M23;
            }

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValueTranspose(ReadOnlySpan<Matrix4x4> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValueTranspose(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(Quaternion value)
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            fData[2] = value.Z;
            fData[3] = value.W;

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<Quaternion> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(float value)
        {
            if (ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            ((float[])Data)[0] = value;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(float[] value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(Texture value)
        {
            if (ParameterType != EffectParameterType.Texture &&
                ParameterType != EffectParameterType.Texture1D &&
                ParameterType != EffectParameterType.Texture2D &&
                ParameterType != EffectParameterType.Texture3D &&
                ParameterType != EffectParameterType.TextureCube)
                throw new InvalidCastException();

            Data = value;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(Vector2 value)
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<Vector2> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(Vector3 value)
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            fData[2] = value.Z;

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<Vector3> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(Vector4 value)
        {
            if (ParameterClass != EffectParameterClass.Vector ||
                ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            float[] fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            fData[2] = value.Z;
            fData[3] = value.W;

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(ReadOnlySpan<Vector4> value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }
    }
}