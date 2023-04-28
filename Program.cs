using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

public static partial class Emscripten
{
    const string Lib = "__Emscripten_Internal";

    public enum EMSCRIPTEN_RESULT : int
    {
	Success = 0,
	Deferred = 1,
	NotSupported = -1,
	FailedNotDeferred = -2,
	InvalidTarget = -3,
	UnknownTarget = -4,
	InvalidParam = -5,
	Failed = -6,
	NoData = -7,
	TimedOut = -8,
    }

    public enum EM_BOOL : int
    {
	False = 0,
	True = 1,
    }

    public static partial class WebGL
    {
	public enum EM_WEBGL_POWER_PREFERENCE : int
	{
	    Default = 0,
	    LowPower = 1,
	    HighPerformance = 2,
	}

	public enum EMSCRIPTEN_WEBGL_CONTEXT_PROXY_MODE : int
	{
	    Disallow = 0,
	    Fallback = 1,
	    Always = 2,
	}

	// see emscripten/html5_webgl.h
	public struct ContextAttributes
	{
	    public EM_BOOL alpha;
	    public EM_BOOL depth;
	    public EM_BOOL stencil;
	    public EM_BOOL antialias;
	    public EM_BOOL premultipliedAlpha;
	    public EM_BOOL preserveDrawingBuffer;
	    public EM_WEBGL_POWER_PREFERENCE powerPreference;
	    public EM_BOOL failIfMajorPerformanceCaveat;

	    public int majorVersion;
	    public int minorVersion;

	    public EM_BOOL enableExtensionsByDefault;
	    public EM_BOOL explicitSwapControl;
	    public EMSCRIPTEN_WEBGL_CONTEXT_PROXY_MODE proxyContextToMainThread;
	    public EM_BOOL renderViaOffscreenBackBuffer;
	}

	[LibraryImport(Lib, EntryPoint="emscripten_webgl_init_context_attributes")]
	public static partial void InitContextAttributes(ref ContextAttributes attr);

	[LibraryImport(Lib, EntryPoint="emscripten_webgl_create_context", StringMarshalling=StringMarshalling.Utf8)]
	public static partial IntPtr CreateContext(string target, ref ContextAttributes attr);

	[LibraryImport(Lib, EntryPoint="emscripten_webgl_make_context_current")]
	public static partial EMSCRIPTEN_RESULT MakeContextCurrent(IntPtr context);

	[LibraryImport(Lib, EntryPoint="emscripten_webgl_destroy_context")]
	public static partial EMSCRIPTEN_RESULT DestroyContext(IntPtr context);
    }
}

public static partial class GLES2
{
    const string Lib = "__Emscripten_Internal";

    public static uint COLOR_BUFFER_BIT = 0x00004000;

    [LibraryImport(Lib, EntryPoint="glClearColor")]
    public static partial void ClearColor(float r, float g, float b, float a);

    [LibraryImport(Lib, EntryPoint="glClear")]
    public static partial void Clear(uint mask);
}

public partial class MyClass
{
    public static void Main () {}

    [JSExport]
    internal static string Greeting()
    {
        var text = $"The page at {GetHRef()} should contain a pleasant purple rectangle";
        Console.WriteLine(text);
        return text;
    }

    [JSImport("window.location.href", "main.js")]
    internal static partial string GetHRef();

    [JSExport]
    internal static void Draw(string canvasSelector)
    {
	try
	{
	    Emscripten.WebGL.ContextAttributes attr = new();
	    Emscripten.WebGL.InitContextAttributes(ref attr);
	    attr.alpha = Emscripten.EM_BOOL.False;
	    IntPtr context = Emscripten.WebGL.CreateContext(canvasSelector, ref attr);
	    if (context == IntPtr.Zero) {
		Console.WriteLine ($"got null context with selector {canvasSelector}");
		return;
	    }
	    CheckResult(Emscripten.WebGL.MakeContextCurrent(context));
	    
	    (int, int, int) brand = (0x51, 0x2b, 0xd4);
	    float r = brand.Item1 / 255.0f;
	    float g = brand.Item2 / 255.0f;
	    float b = brand.Item3 / 255.0f;
	    GLES2.ClearColor(r, g, b, 1.0f);
	    
	    GLES2.Clear(GLES2.COLOR_BUFFER_BIT);

	    CheckResult(Emscripten.WebGL.DestroyContext(context));
	}
	catch (Exception e)
	{
	    Console.WriteLine ($"Unexpected error {e}");
	}
    }

    static void CheckResult(Emscripten.EMSCRIPTEN_RESULT r, [CallerArgumentExpression(nameof(r))] string expr = default)
    {
	if (r != Emscripten.EMSCRIPTEN_RESULT.Success) {
	    throw new InvalidOperationException($"emscripten returned {r} in {expr}");
	}
    }
}
