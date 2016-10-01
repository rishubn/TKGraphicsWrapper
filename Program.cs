using System;
using System.Collections.Generic;

using System.Text;
using OpenTK.Graphics.ES20;

namespace Graphics
{
    /* Rishub Nagpal
     * June 9 2016
     * OpenGL program wrapper
     */
    sealed class GLProgram : IDisposable
    {

        private readonly int _id = -1;
        public int ID { get { return this._id; } }
        public Dictionary<string, GLAttribute> Attributes = new Dictionary<string, GLAttribute>();
        public Dictionary<string, GLUniform> Uniforms = new Dictionary<string, GLUniform>();
        public readonly OpenGL _gl;
        public static readonly string TAG = "GLProgram";
        public GLProgram(OpenGL gl)
        {
            _gl = gl;
            _id = GL.CreateProgram();
            if(_id == 0)
            {
                throw new OpenGLException("GL.Program failed");
            } 
        }
        public GLProgram(OpenGL gl, params GLShader[] shaders) : this(gl)
        {
            Attach(shaders);
            Link();
            GetAttributes();
            GetUniforms();
            DeleteShaders(shaders);
        }
        
        public void Attach(GLShader[] shaders)
        {
            foreach(GLShader s in shaders)
            {
                GL.AttachShader(_id, s.ID);
                _gl.RuntimeCheck(TAG);
            } 
        }
        public void GetAttributes()
        {
            int attribcount = 0;
            GL.GetProgram(_id, ProgramParameter.ActiveAttributes, out attribcount);
            StringBuilder n = new StringBuilder();
            for(int i = 0; i < attribcount; i++)
            {
                int length = 0;
                int size = 0;
                ActiveAttribType type;
                GL.GetActiveAttrib(_id, i, 256, out length, out size, out type, n);
                string name = n.ToString();
                int location = GL.GetAttribLocation(_id, name);
                _gl.RuntimeCheck(TAG);
                GLAttribute info = new GLAttribute(name,location,size,type);
                Attributes.Add(name, info);
                n.Clear();
            }
        }
        public void GetUniforms()
        {
            int uniformCount = 0;
            GL.GetProgram(_id, ProgramParameter.ActiveUniforms, out uniformCount);
            StringBuilder n = new StringBuilder();
            for(int i = 0; i < uniformCount; i++)
            {
                int length = 0;
                int size = 0;
                ActiveUniformType type;
                GL.GetActiveUniform(_id, i, 256, out length, out size, out type, n);
                _gl.RuntimeCheck(TAG);
                GLUniform info = new GLUniform(_gl, _id, n.ToString(), size, type);
                Uniforms.Add(n.ToString(), info);
                n.Clear();
            }
        }
        public void Use()
        {
            GL.UseProgram(_id);
            _gl.RuntimeCheck(TAG);
            foreach(GLUniform uniform in Uniforms.Values)
            {
                uniform.Use();
            }
        }
        public void Unuse()
        {
            foreach (GLUniform uniform in Uniforms.Values)
            {
                uniform.Unuse();
            }
            GL.UseProgram(0);
            _gl.RuntimeCheck(TAG);
        }
        public void Link()
        {
            GL.LinkProgram(_id);
            _gl.RuntimeCheck(TAG);
            int linkStatus = 0;
            GL.GetProgram(_id, ProgramParameter.LinkStatus, out linkStatus);
            if(linkStatus != 1)
            {
                string log = getLinkLog();
                if(log != null) { _gl.Log(TAG, "Cannot link program", OpenGL.LogCode.Error); }
                throw new OpenGLException("Program Link Error");
            }
        }
        public string getLinkLog()
        {
            int logLength = 0;
            GL.GetProgram(_id, ProgramParameter.InfoLogLength, out logLength);
            if(logLength <= 0) { return null; }
            string log = GL.GetProgramInfoLog(_id);
            _gl.RuntimeCheck(TAG);
            return log;
        }
        public void DeleteShaders(GLShader[] shaders)
        {
            foreach(GLShader s in shaders)
            {
                GL.DetachShader(_id, s.ID);
                s.Dispose();
            }
        }
        bool disposed;
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }
        private void dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    GL.DeleteProgram(_id);
                    disposed = true;
                }
                else
                {
                    _gl.Log(TAG, String.Format("Resource Leaked {0}", TAG),OpenGL.LogCode.Warn);
                    disposed = true;
                }
            }
        }
        ~GLProgram()
        {
            dispose(false);
        }
    }
    sealed class GLAttribute
    {
        private readonly string _name;
        private readonly int _size;
        private readonly ActiveAttribType _type;
        private readonly int _location;
        public string Name { get { return _name; } }
        public int Size { get { return _size; } }
        public ActiveAttribType Type { get { return _type; } }
        public int Location { get { return _location; } }
        public GLAttribute(string name, int location, int size, ActiveAttribType type)
        {
            _name = name;
            _location = location;
            _size = size;
            _type = type;
        }
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", _name, _location, _size, _type);
        }
    }
}
