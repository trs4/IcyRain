using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace IcyRain.Grpc.AspNetCore.Web.Internal;

internal sealed class GrpcWebFeature : IRequestBodyPipeFeature, IHttpResponseBodyFeature, IHttpResponseTrailersFeature, IHttpResetFeature
{
    private readonly IHttpResponseBodyFeature _initialResponseFeature;
    private readonly IRequestBodyPipeFeature? _initialRequestFeature;
    private readonly IHttpResetFeature? _initialResetFeature;
    private readonly IHttpResponseTrailersFeature? _initialTrailersFeature;
    private Stream? _responseStream;
    private bool _isComplete;

    public GrpcWebFeature(ServerGrpcWebContext grcpWebContext, HttpContext httpContext)
    {
        // Capture existing features. We'll use these internally, and restore them onto the context
        // once the middleware has finished executing

        // Note that some of these will be missing depending on the host and protocol
        // e.g.
        // - IHttpResponseTrailersFeature and IHttpResetFeature will be missing when HTTP/1.1
        // - IRequestBodyPipeFeature will be missing when in IIS
        _initialRequestFeature = httpContext.Features.Get<IRequestBodyPipeFeature>();
        _initialResponseFeature = GetRequiredFeature<IHttpResponseBodyFeature>(httpContext);
        _initialResetFeature = httpContext.Features.Get<IHttpResetFeature>();
        _initialTrailersFeature = httpContext.Features.Get<IHttpResponseTrailersFeature>();

        var innerReader = _initialRequestFeature?.Reader ?? httpContext.Request.BodyReader;
        var innerWriter = _initialResponseFeature.Writer ?? httpContext.Response.BodyWriter;

        Trailers = new HeaderDictionary();
        Reader = grcpWebContext.Request == ServerGrpcWebMode.GrpcWebText ? new Base64PipeReader(innerReader) : innerReader;
        Writer = grcpWebContext.Response == ServerGrpcWebMode.GrpcWebText ? new Base64PipeWriter(innerWriter) : innerWriter;

        httpContext.Features.Set<IRequestBodyPipeFeature>(this);
        httpContext.Features.Set<IHttpResponseBodyFeature>(this);
        httpContext.Features.Set<IHttpResponseTrailersFeature>(this);
        httpContext.Features.Set<IHttpResetFeature>(this);
    }

    private static T GetRequiredFeature<T>(HttpContext httpContext)
        => httpContext.Features.Get<T>() ?? throw new InvalidOperationException($"Couldn't get {typeof(T).FullName} from the current request");

    public PipeReader Reader { get; }

    public PipeWriter Writer { get; }

    Stream IHttpResponseBodyFeature.Stream => _responseStream ??= Writer.AsStream();

    public IHeaderDictionary Trailers { get; set; }

    public async Task CompleteAsync()
    {
        // TODO(JamesNK): When CompleteAsync is called from another thread (e.g. deadline exceeded),
        // there is the potential for the main thread and CompleteAsync to both be writing to the response.
        // Shouldn't matter to the client because it will have already thrown deadline exceeded error, but
        // the response could return badly formatted trailers
        await WriteTrailersAsync();
        await _initialResponseFeature.CompleteAsync();
        _isComplete = true;
    }

    public void DisableBuffering() => _initialResponseFeature.DisableBuffering();

    public void Reset(int errorCode)
    {
        // We set a reset feature so that HTTP/1.1 doesn't call HttpContext.Abort() on deadline
        // In HTTP/1.1 this will do nothing. In HTTP/2+ it will call the real reset feature
        _initialResetFeature?.Reset(errorCode);
    }

    internal void DetachFromContext(HttpContext httpContext)
    {
        httpContext.Features.Set(_initialRequestFeature!);
        httpContext.Features.Set(_initialResponseFeature);
        httpContext.Features.Set(_initialTrailersFeature!);
        httpContext.Features.Set(_initialResetFeature!);
    }

    public Task SendFileAsync(string path, long offset, long? count, CancellationToken token)
        => throw new NotSupportedException("Sending a file during a gRPC call is not supported");

    public Task StartAsync(CancellationToken token) => _initialResponseFeature.StartAsync(token);

    public Task WriteTrailersAsync()
    {
        if (!_isComplete && Trailers.Count > 0)
            return GrpcWebProtocolHelpers.WriteTrailersAsync(Trailers, Writer);

        return Task.CompletedTask;
    }

}
