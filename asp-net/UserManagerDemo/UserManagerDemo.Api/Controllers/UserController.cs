using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagerDemo.Api.Common.Results;
using UserManagerDemo.Application.Common.Interface;
using UserManagerDemo.Application.Users.Dtos;
using UserManagerDemo.Domain.Entity;

namespace UserManagerDemo.API.Controllers
{
    [Authorize]
    public class UserController : BaseApiController<UserController>

    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUserProfile, Guid> _userRepository;
        private readonly IValidator<UserDto> _userValidator;

        public UserController(
            IMapper mapper,
            ILogger<UserController> logger,
            IRepository<ApplicationUserProfile, Guid> userRepository,
            IValidator<UserDto> userValidator,
            UserManager<ApplicationUser> userManager) : base(mapper, logger)
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<Result<ReadUserDto>> Create(UserDto dto)
        {
            var user = _mapper.Map<ApplicationUserProfile>(dto);
            await _userRepository.AddAsync(user);

            _logger.LogInformation($"User {user.Id} created");

            return Result.Ok(_mapper.Map<ReadUserDto>(user));
        }

        [HttpDelete]
        public async Task<Result> Delete(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User {id} not found for delete");
                return Result.Fail($"User {id} not found for delete");
            }

            var appUser = await _userManager.FindByIdAsync(user.UserId.ToString());
            await _userRepository.DeleteAsync(user);
            await _userManager.DeleteAsync(appUser);

            _logger.LogInformation($"User {id} deleted");

            return Result.Ok().WithSuccess($"User {id} deleted");
        }

        [HttpGet]
        public async Task<Result<IEnumerable<ReadUserDto>>> GetAll()
        {
            _logger.LogInformation("Fetching all users...");
            var users = await _userRepository.GetAllAsync();
            return Result.Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
        }

        [HttpGet]
        public async Task<Result<ReadUserDto>> GetById(Guid id)
        {
            _logger.LogInformation($"Fetching user {id}");
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", id);
                return Result.Fail(new Error($"User {id} not found"));
            }

            return Result.Ok(_mapper.Map<ReadUserDto>(user));
        }

        [HttpGet]
        public async Task<Result<PagedResult<ReadUserDto>>> GetPagging(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching user at page: {pageIndex}, size: {pageSize}");

            var (users, totalCount) = await _userRepository.GetPagedAsync(pageIndex, pageSize);

            var pagedResult = new PagedResult<ReadUserDto>(
                _mapper.Map<IEnumerable<ReadUserDto>>(users),
                totalCount,
                pageIndex,
                pageSize
            );
            return Result.Ok(pagedResult);
        }

        [HttpPut]
        public async Task<Result> Update(Guid id, UpdateUserDto dto)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning($"User {id} not found for update");
                return Result.Fail($"User {id} not found for update");
            }

            _mapper.Map(dto, existing);
            await _userRepository.UpdateAsync(existing);

            _logger.LogInformation($"User {id} updated");

            return Result.Ok().WithSuccess($"User {id} updated");
        }
    }
}