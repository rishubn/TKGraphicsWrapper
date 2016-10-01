using System;
using OpenTK.Graphics.ES20;


namespace Graphics
{
    /* Rishub Nagpal
     * June 22 2016
     * OpenGL VAO wrapper
     */
    sealed class GLVAO : IDisposable
    {
        private readonly OpenGL _gl;
        private readonly int _id;
        public int ID { get { return _id; } }
        public static readonly string TAG = "GLVAO";

        public GLVAO(OpenGL gl)
        {
            _gl = gl;
            GL.Oes.GenVertexArrays(1, out _id);
            _gl.RuntimeCheck(TAG);
        }
        public void Bind()
        {
            GL.Oes.BindVertexArray(_id);
            _gl.RuntimeCheck(TAG);
        }
        public void Unbind()
        {
            GL.Oes.BindVertexArray(0);
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
                    int id = _id;
                    GL.Oes.DeleteVertexArrays(1, ref id);
                    _gl.RuntimeCheck(TAG);
                    disposed = true;
                }
                else
                {
                    _gl.Log(TAG, String.Format("Resource Leaked {0}", TAG), OpenGL.LogCode.Warn);
                    disposed = true;
                }
            }
        }
        ~GLVAO()
        {
            dispose(false);
        }
    }
}
