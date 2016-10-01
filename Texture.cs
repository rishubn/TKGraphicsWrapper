using System;
using OpenTK;
using OpenTK.Graphics.ES20;



namespace TreeGo.Droid.Graphics
{
    class GLTexture : IDisposable
    {
        public readonly OpenGL _gl;
        private readonly int _id;
        private readonly TextureTarget _target;
        public int ID { get { return _id; } }
        public TextureTarget Target { get { return _target; } }
        public static readonly string TAG = "GLTexture";

        public GLTexture(OpenGL gl, TextureTarget target)
        {
            _gl = gl;
            _target = target;
            GL.GenTextures(1, out _id);
            _gl.RuntimeCheck(TAG);
        }

        public void use(int textureUnit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            bind();    
        }
        public int GetParam(GetTextureParameter texp)
        {
            int res;
            bind();
            GL.GetTexParameter(_target, texp, out res);
            _gl.RuntimeCheck(TAG);
            return res;
        }
        public void SetWrapS(TextureWrapMode wrapS)
        {
            bind();
            GL.TexParameter(_target, TextureParameterName.TextureWrapS, (int)wrapS);
            _gl.RuntimeCheck(TAG);
        }
        public void SetWrapT(TextureWrapMode wrapT)
        {
            bind();
            GL.TexParameter(_target, TextureParameterName.TextureWrapT, (int)wrapT);
            _gl.RuntimeCheck(TAG);
        }
        public void SetMinFilter(TextureMinFilter minFilter)
        {
            bind();
            GL.TexParameter(_target, TextureParameterName.TextureMinFilter, (int)minFilter);
            _gl.RuntimeCheck(TAG);
        }
        public void SetMagFilter(TextureMagFilter magFilter)
        {
            bind();
            GL.TexParameter(_target, TextureParameterName.TextureMagFilter, (int)magFilter);
            _gl.RuntimeCheck(TAG);
        }
        public void GenerateMipmap()
        {
            bind();
            GL.GenerateMipmap(_target);
            _gl.RuntimeCheck(TAG);
        }
        private void bind()
        {
            GL.BindTexture(_target, _id);
            _gl.RuntimeCheck(TAG);
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
                    GL.DeleteTexture(_id);
                    disposed = true;
                }
                else
                {
                    _gl.Log(TAG, String.Format("Resource Leaked {0}", TAG), OpenGL.LogCode.Warn);
                    disposed = true;
                }
            }
        }
        ~GLTexture()
        {
            dispose(false);
        }
    }

    class GLTexture2D : GLTexture
    {
        public GLTexture2D(OpenGL _gl) : base(_gl, TextureTarget.Texture2D) { }

        public void SetImage(int level, Android.Graphics.Bitmap bmp, int border)
        {
            Android.Opengl.GLUtils.TexImage2D((int)Target, level, bmp, border);
            _gl.RuntimeCheck(TAG);

        }
    }
}