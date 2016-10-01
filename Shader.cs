using System;
using OpenTK.Graphics.ES20;

namespace Graphics
{
    /* Rishub Nagpal
     * June 9 2016
     * OpenGL Shader Wrapper
     */
    sealed class GLShader : IDisposable
    {
        private int _id;
        private ShaderType _type;
        private readonly OpenGL _gl;
        public int ID { get { return this._id; } }
        public ShaderType Type { get { return this._type; } }
        public static readonly string TAG = "GLShader";
        public GLShader(OpenGL gl, ShaderType type)
        {
            _gl = gl;
            _type = type;
            _id = GL.CreateShader(type);
            if(_id == 0)
            {
                throw new OpenGLException("GL.CreateShader failed");
            }
        }
        public GLShader(OpenGL gl, ShaderType type, string code)
        {
            _gl = gl;
            CreateShader(type, code);
            Compile();
        }
        public void CreateShader(ShaderType type, string code)
        {
            _id = GL.CreateShader(type);
            _type = type;
            GL.ShaderSource(_id, code);
            runtimeCheck();
        }
        public void Compile()
        {
            GL.CompileShader(_id);
            runtimeCheck();
            string infolog = GetInfoLog();
            if(infolog != null)
            {
                _gl.Log(TAG, infolog, OpenGL.LogCode.Info);
            }
            int compiled = 0;
            GL.GetShader(_id, ShaderParameter.CompileStatus, out compiled);
            if(compiled != 1)
            {
                throw new OpenGLException("shader did not compile");
            }
        }
        public string GetInfoLog()
        {
            int logLength = 0;   
            GL.GetShader(_id, ShaderParameter.InfoLogLength,out logLength);
            if(logLength <= 0)
            {
                return null;
            }
           
            string log = GL.GetShaderInfoLog(_id);
            runtimeCheck();
            return log;

        }
        bool disposed;
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }
        private void runtimeCheck()
        {
            _gl.RuntimeCheck(TAG);
        }
        private void dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    GL.DeleteShader(_id);
                }
                else //Always make sure you handle your own resources, not GC
                {
                    string message = String.Format("Resource Leaked {0}", this);
                    _gl.Log(TAG, message, OpenGL.LogCode.Warn);
                    disposed = true;
                }
            }
        }
        ~GLShader()
        {
            dispose(false);
        }
    }
}
