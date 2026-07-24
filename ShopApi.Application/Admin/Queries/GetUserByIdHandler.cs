using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Admin.Queries;

public class GetUserByIdHandler(IUserRepository users)
    : IRequestHandler<GetUserByIdQuery, UserResponseDto?>
{
    public async Task<UserResponseDto?> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(query.UserId, ct);
        return user is null ? null : new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role);
    }
}