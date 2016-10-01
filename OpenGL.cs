using System;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
namespace Graphics
{
    //OpenGL exception so we know when something went wrong with OpenGL
    class OpenGLException : Exception
    {
        public OpenGLException() : base() { }
        public OpenGLException(string message) : base(message) { }
    }
    /* Rishub Nagpal
     * June 25 2016
     * OpenGL wrapper for easy context handling and logging
     */
    sealed class OpenGL
    {
        private static readonly string PathTag = "Add your namespace here";
        public static readonly string TAG = "OpenGL";
        private readonly string _version;
        private readonly string _glslversion;
        private readonly string _vendor;
        private readonly string _extensions;
        private readonly string _renderer;
        private readonly IGraphicsContext _context;
        public string Version { get { return _version;  } }
        public string GLSLVersion { get { return _glslversion; } }
        public string Vender { get { return _vendor; } }
        public string Extensions { get { return _extensions; } }
        public string Renderer { get { return _renderer; } }
        public IGraphicsContext Context { get { return _context; } }
        public enum LogCode
        {
            Debug, 
            Error, 
            Info, 
            Verbose, 
            Warn, 
            WriteLine, 
            WTF //What a terrible failure
        };
        public OpenGL(IGraphicsContext context)
        {
            _context = context;
            _version = GL.GetString(StringName.Version);
            _glslversion = GL.GetString(StringName.ShadingLanguageVersion);
            _vendor = GL.GetString(StringName.Vendor);
            _extensions = GL.GetString(StringName.Extensions);
            _renderer = GL.GetString(StringName.Renderer);
        }
        public void FlushErrors()
        {
            int t = 0;
            while(t <=5)
            {
                ErrorCode e = GL.GetErrorCode();
                if (e == ErrorCode.NoError)
                    break;
                t++;
            }
        }
        public void Log(string tag,string message, LogCode code = LogCode.Debug/*, 
            Android.Util.LogPriority priority = Android.Util.LogPriority.Debug*/)
        {
            tag = PathTag + tag;
                        switch (code)
            {
                case LogCode.Debug:
                    //Android.Util.Log.Debug(tag, message);
                    Console.WriteLine("[{0}] {1}] {2}", "DEBUG", tag, message);
                    break;
                case LogCode.Error:
                    //Android.Util.Log.Error(tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "ERROR", tag, message);
                    break;
                case LogCode.Info:
                    //Android.Util.Log.Info(tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "INFO", tag, message);
                    break;
                case LogCode.Verbose:
                    //Android.Util.Log.Verbose(tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "VERBOSE", tag, message);
                    break;
                case LogCode.Warn:
                    //Android.Util.Log.Warn(tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "WARN", tag, message);
                    break;
                case LogCode.WriteLine:
                    //Android.Util.Log.WriteLine(priority, tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "WRITELINE", tag, message);
                    break;
                case LogCode.WTF:
                    //Android.Util.Log.Wtf(tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "WTF", tag, message);
                    break;
                default: //if this case is ever reached, something very very bad happened
                    //Android.Util.Log.Wtf(tag, message);
                    Console.WriteLine("[{0}] {1} {2}", "DEFAULT", tag, message);
                    break;
            }
        }
        public void RuntimeCheck(string tag)
        {
            ErrorCode err = GL.GetErrorCode();
            if (err != ErrorCode.NoError)
            {
                FlushErrors();
                //Android.Util.Log.Error(PathTag + tag, err.ToString());
                Console.WriteLine((PathTag + tag) + err);
                throw new OpenGLException(err.ToString());
            }
        }
        public void SetActiveTexture(TextureUnit tex)
        {
            GL.ActiveTexture(tex);
            RuntimeCheck(TAG);
        }
    }
}`
