using System;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace TreeGo.Droid.Graphics
{
    /* Rishub Nagpal
     * June 30 2016
     * Uniform Wrapper
     */
    class GLUniform
    {
        private readonly OpenGL _gl;
        private readonly int _programid;
        private readonly string _name;
        private readonly int _location;
        private readonly int _size;
        private readonly ActiveUniformType _type;
        private bool _dataChanged;
        private bool _firstSet;
        private bool _disabled;
        private bool _shouldUpdate;
        private Object _data;
        public string Name { get { return _name; } }
        public int Location { get { return _location; } }
        public int Size { get { return _size; } }
        public Object Data { get { return _data; } }
        public ActiveUniformType Type { get { return _type; } }
        public readonly string TAG = "GLUniform";
        public GLUniform(OpenGL gl, int programid, string name, int size, ActiveUniformType type)
        {
            _gl = gl;
            _programid = programid;
            _name = name;
            _size = size;
            _type = type;
            _location = GL.GetUniformLocation(_programid, name);
            if(_location == -1) { throw new OpenGLException(String.Format("Can't get uniform {0} location", name)); }
            if(_size > 0)
            {
                setDefaults();
                _dataChanged = false;
                _firstSet = true;
                _disabled = false;
            }
            else
            {
                _gl.Log(TAG, String.Format("Uniform {0} has size 0, disabled", _name),OpenGL.LogCode.Warn);
                _disabled = true;
            }
        }
        public GLUniform(OpenGL gl, string name)
        {
            _gl = gl;
            _disabled = true;
            _gl.Log(TAG, String.Format("Fake uniform created {0}",name), OpenGL.LogCode.Warn);
        }
        private void checkTypeCompliancy<T>()
        {
            Type t = typeof(T);
            if (t == typeof(int)) { return; }
            if (t == typeof(float)) { return; } //scalars
            if (t == typeof(Half)) { return; }
            if (t == typeof(Vector2)) { return; } //floating point vectors
            if (t == typeof(Vector3)) { return; }
            if (t == typeof(Vector4)) { return; }
            if (t == typeof(Vector2h)) { return; } //half point vectors
            if (t == typeof(Vector3h)) { return; }
            if (t == typeof(Vector4h)) { return; }
            if (t == typeof(Vector2d)) { return; } //double vectors
            if (t == typeof(Vector3d)) { return; }
            if (t == typeof(Vector4d)) { return; }
            if (t == typeof(Matrix2)) { return; } //floating point matricies
            if (t == typeof(Matrix3)) { return; }
            if (t == typeof(Matrix4)) { return; }
            if (t == typeof(Matrix4d)) { return; } //double point Matrix
            throw new OpenGLException(String.Format("Type : {0} not valid uniform type", typeof(T).ToString()));
        }
        public void Use()
        {
            _shouldUpdate = true;
            update();
        }
        public void Unuse()
        {
            _shouldUpdate = false;
        }
        private void update()
        {
            if (_disabled) { return; }
            if (_firstSet) { _gl.Log(TAG, String.Format("Uniform value not set, using default"), OpenGL.LogCode.Warn); _firstSet = false; }
            if (_dataChanged) { applyData(); _dataChanged = false; }
        }
        private void applyData()
        {
            switch (_type)
            {
                case ActiveUniformType.Int:
                case ActiveUniformType.Sampler2D: { GL.Uniform1(_location, (int)_data); } break;
                case ActiveUniformType.Float: { GL.Uniform1(_location, (float)_data); } break;
                case ActiveUniformType.FloatVec2:
                case ActiveUniformType.BoolVec2:
                case ActiveUniformType.IntVec2: { Vector2 temp = (Vector2)_data; GL.Uniform2(_location, ref temp); } break;
                case ActiveUniformType.FloatVec3:
                case ActiveUniformType.BoolVec3:
                case ActiveUniformType.IntVec3: { Vector3 temp = (Vector3)_data; GL.Uniform3(_location, ref temp); } break;
                case ActiveUniformType.FloatVec4:
                case ActiveUniformType.BoolVec4:
                case ActiveUniformType.IntVec4: { Vector4 temp = (Vector4)_data; GL.Uniform4(_location, ref temp); } break;
                case ActiveUniformType.FloatMat2: { Matrix2 temp = (Matrix2)_data; GL.UniformMatrix2(_location, false, ref temp); } break;
                case ActiveUniformType.FloatMat3: { Matrix3 temp = (Matrix3)_data; GL.UniformMatrix3(_location, false, ref temp); } break;
                case ActiveUniformType.FloatMat4: { Matrix4 temp = (Matrix4)_data; GL.UniformMatrix4(_location, false, ref temp); } break;
                default: throw new OpenGLException("Invalid Uniform type");
            }
        }
        private void setDefaults()
        {
            switch (_type)
            {
                case ActiveUniformType.Int:
                case ActiveUniformType.Float: 
                case ActiveUniformType.Sampler2D: { _data = 0; } break;
                case ActiveUniformType.FloatVec2:
                case ActiveUniformType.BoolVec2:
                case ActiveUniformType.IntVec2: { _data = Vector2.Zero; } break;
                case ActiveUniformType.FloatVec3:
                case ActiveUniformType.BoolVec3:
                case ActiveUniformType.IntVec3: { _data = Vector3.Zero; } break;
                case ActiveUniformType.FloatVec4:
                case ActiveUniformType.BoolVec4:
                case ActiveUniformType.IntVec4: { _data = Vector4.Zero; } break;
                case ActiveUniformType.FloatMat2: { _data = Matrix2.Identity; } break;
                case ActiveUniformType.FloatMat3: { _data = Matrix3.Identity; } break;
                case ActiveUniformType.FloatMat4: { _data = Matrix4.Identity; } break;
                default: break;
            }
            _gl.RuntimeCheck(TAG);
        }
        public void SetData<T>(T data)
        {
            checkTypeCompliancy<T>();
            if (_disabled) { return; }
            
            if(_firstSet || _data != (object)data)
            {
                _data = data;
                _dataChanged = true;
                if (_shouldUpdate) { update(); }
            }
            _firstSet = false;
        }
    }
}