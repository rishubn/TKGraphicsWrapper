using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.ES20;


namespace TreeGo.Droid.Graphics
{
    /* Rishub Nagpal
     * June 22 2016
     * OpenGL Buffer wrapper
     */
    sealed class GLBuffer<TVertex> : IDisposable
        where TVertex : struct
    {
        private readonly int _id;
        private readonly BufferUsage _usage;
        private readonly BufferTarget _target;
        private int _size;
        private bool _firstLoad;
        private readonly OpenGL _gl;
        public int ID { get { return _id; } }
        public BufferUsage Usage { get { return _usage; } }
        public BufferTarget Target { get { return _target; } }
        public int Size { get { return _size; } }
        public static readonly string TAG = "GLBuffer";

        public GLBuffer(OpenGL gl, BufferTarget target, BufferUsage usage)
        {
            _target = target;
            _usage = usage;
            _firstLoad = true;
            _gl = gl;
            GL.GenBuffers(1, out _id);
            _gl.RuntimeCheck(TAG);
            _size = 0;
        }
        public GLBuffer(OpenGL gl, BufferTarget target, BufferUsage usage, TVertex[] vertices) 
            : this(gl, target, usage)
        {
            SetData(vertices);

        }
        public void SetData(TVertex[] vs)
        {
            Bind();
            _size = Marshal.SizeOf(typeof(TVertex)) * vs.Length;
            if (!_firstLoad)
            {
                //Clear the buffer
                GL.BufferData(_target, new IntPtr(_size), IntPtr.Zero, _usage);
                GL.BufferSubData(_target, IntPtr.Zero, new IntPtr(_size), vs);
            }
            else
            {
                GL.BufferData(_target, new IntPtr(_size), vs, _usage);
            }
            _gl.RuntimeCheck(TAG);
            _firstLoad = false;
        }
        public void Bind()
        {
            GL.BindBuffer(_target, _id);
            _gl.RuntimeCheck(TAG);
        }
        public void Unbind()
        {
            GL.BindBuffer(_target, 0);
            _gl.RuntimeCheck(TAG);
        }
        //Resource handler
        //If GC collects this buffer, Log will state the resource has been leaked
        //Always manually dispose GL resources
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
                    GL.DeleteBuffers(1, ref id);
                    disposed = true;
                }
                else
                {
                    _gl.Log(TAG, String.Format("Resource Leaked {0}", TAG), OpenGL.LogCode.Warn);
                    disposed = true;
                }  
            }
        }
        ~GLBuffer()
        {
            dispose(false);
        }
    }
}