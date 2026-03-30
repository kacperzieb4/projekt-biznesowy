using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AttractionCatalog.API.Controllers
{
    /// <summary>
    /// Base API Controller that provides streamlined access to common infrastructure.
    /// Eliminates constructor boilerplate in derived controllers.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender? _mediator;

        /// <summary>
        /// Access the MediatR Sender via property injection (from HttpContext).
        /// Using ISender instead of IMediator for stricter adherence to CQS principles.
        /// </summary>
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
