using System;
using System.Web;
using ServiceStack.Web;

namespace ServiceStack.Host
{
	public class RequestPreferences : IRequestPreferences
	{
		private readonly HttpContext httpContext;

		public RequestPreferences(IHttpRequest httpRequest)
		{
			this.acceptEncoding = httpRequest.Headers[HttpHeaders.AcceptEncoding];
			if (this.acceptEncoding.IsNullOrEmpty())
			{
				this.acceptEncoding = "none";
				return;
			}
			this.acceptEncoding = this.acceptEncoding.ToLower();
		}

		public RequestPreferences(HttpContext httpContext)
		{
			this.httpContext = httpContext;
		}

		public static HttpWorkerRequest GetWorker(HttpContext context)
		{
			var provider = (IServiceProvider)context;
			var worker = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
			return worker;
		}

		private HttpWorkerRequest httpWorkerRequest;
	    private HttpWorkerRequest HttpWorkerRequest
		{
			get
			{
				if (this.httpWorkerRequest == null)
				{
					this.httpWorkerRequest = GetWorker(this.httpContext);
				}
				return this.httpWorkerRequest;
			}
		}

		private string acceptEncoding;
		public string AcceptEncoding
		{
			get
			{
				if (acceptEncoding == null)
				{
					acceptEncoding = HttpWorkerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderAcceptEncoding);
					if (acceptEncoding != null) acceptEncoding = acceptEncoding.ToLower();
				}
				return acceptEncoding;
			}
		}

		public bool AcceptsGzip
		{
			get
			{
				return AcceptEncoding != null && AcceptEncoding.Contains("gzip");
			}
		}

		public bool AcceptsDeflate
		{
			get
			{
				return AcceptEncoding != null && AcceptEncoding.Contains("deflate");
			}
		}
	
	}
}