using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Queries;

public class GetProfileHandler(IUserRepository users)
    : IRequestHandler<GetProfileQuery, UserResponseDto?>
{
    public async Task<UserResponseDto?> Handle(GetProfileQuery query, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(query.UserId, ct);
        return user is null
            ? null
            : new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role);
    }
}