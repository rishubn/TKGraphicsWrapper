using System;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.ES20;
using System.Collections.Generic;
namespace TreeGo.Droid.Graphics
{
    /* Rishub Nagpal
     * June 23 2016
     * VertexSpecification - Parses vertex struct and generates vertex attributes to link to shader
     */
    sealed class VertexSpecification<TVertex>
        where TVertex : struct
    {
        private readonly VertexAttribute[] _attributes;
        public VertexAttribute[] Attributes { get { return _attributes; } }
        private GLProgram _program;
        private readonly OpenGL _gl;
        public static readonly string TAG = "GLVertexSpecification";
        public VertexSpecification(GLProgram program)
        {
            _program = program;
            _gl = program._gl;
            Type t = typeof(TVertex);
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _attributes = new VertexAttribute[fields.Length];
            int i = 0;
            foreach (FieldInfo f in fields)
            {
                string name = f.Name;
                int size = getSizeOfVector(f.FieldType);
                VertexAttribPointerType type = VertexAttribPointerType.Float;
                int stride = Marshal.SizeOf<TVertex>();
                IntPtr offset = Marshal.OffsetOf<TVertex>(f.Name);
                bool normalize = false;
                if(f.GetCustomAttribute<NormalizeAttribute>() != null)
                {
                    normalize = true;
                }  
                _attributes[i] = new VertexAttribute(name, size, type, stride, offset.ToInt32(), normalize);
                _gl.Log(TAG, _attributes[i].ToString());
                i++;
            }
        }
        private int getSizeOfVector(Type vector)
        {
            if(vector == typeof(OpenTK.Graphics.Color4)) { return 4; }
            if(vector == typeof(Vector2)) { return 2; }
            if(vector == typeof(Vector3)) { return 3; }
            if(vector == typeof(Vector4)) { return 4; }
            if (vector == typeof(Vector2d)) { return 2; }
            if (vector == typeof(Vector3d)) { return 3; }
            if (vector == typeof(Vector4d)) { return 4; }
            if (vector == typeof(Vector2h)) { return 2; }
            if (vector == typeof(Vector3h)) { return 3; }
            if (vector == typeof(Vector4h)) { return 4; }
            throw new OpenGLException("Type not implemented");
        }
        public void Use()
        {
            foreach (VertexAttribute va in _attributes)
            {
                va.Use(_program);
            }
        }
        public void Unuse()
        {
            foreach(VertexAttribute va in _attributes)
            {
                va.Unuse(_program);
            }
        }
    }
    [AttributeUsage(AttributeTargets.Field)]
    sealed class NormalizeAttribute : Attribute
    {
        public NormalizeAttribute() { }
    }
    struct VertexAttribute
    {
        private readonly string _name;
        private readonly int _size;
        private readonly VertexAttribPointerType _type;
        private readonly bool _normalize;
        private readonly int _stride;
        private readonly int _offset;
        public string Name { get { return _name; } }
        public int Size { get { return _size; } }
        public VertexAttribPointerType Type { get { return _type; } }
        public bool Normalize { get { return _normalize; } }
        public int Stride { get { return _stride; } }
        public int Offset { get { return _offset; } }
        public static readonly string TAG = "GLVertexAttribute";
        public VertexAttribute(string name, int size, VertexAttribPointerType type,
            int stride, int offset, bool normalize = false)
        {
            this._name = name;
            this._size = size;
            this._type = type;
            this._stride = stride;
            this._offset = offset;
            this._normalize = normalize;
        }

        public void Use(GLProgram program)
        {
            int location = program.Attributes[_name].Location;
            if(location == -1) { return; }
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, _size, _type, _normalize, _stride, _offset);
            program._gl.RuntimeCheck(TAG);
        }
        public void Unuse(GLProgram program)
        {
            int location = program.Attributes[_name].Location;
            GL.DisableVertexAttribArray(location);
            program._gl.RuntimeCheck(TAG);
        }
        public override string ToString()
        {
            return "{name:" + this._name
                + ", size:" + this._size
                + ", type:" + this._type
                + ", normalize:" + this._normalize
                + ", stride:" + this._stride
                + ", offset:" + this._offset
                + "}";
        }
    }
}