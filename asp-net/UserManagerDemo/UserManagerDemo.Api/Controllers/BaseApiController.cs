using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManagerDemo.Api.Attributes;

namespace UserManagerDemo.API.Controllers;

[ApiController]
[UnitOfWork]
[Route("api/[controller]/[action]")]
public abstract class BaseApiController<T> : ControllerBase
{
    protected readonly ILogger<T> _logger;
    protected readonly IMapper _mapper;

    protected BaseApiController(IMapper mapper, ILogger<T> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }
}