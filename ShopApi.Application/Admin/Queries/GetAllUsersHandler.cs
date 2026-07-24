using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Admin.Queries;

public class GetAllUsersHandler(IUserRepository users)
    : IRequestHandler<GetAllUsersQuery, List<UserResponseDto>>
{
    public async Task<List<UserResponseDto>> Handle(GetAllUsersQuery query, CancellationToken ct)
    {
        var list = await users.GetAllAsync(ct);
        return list.Select(u => new UserResponseDto(u.Id, u.Name, u.Email, u.Phone, u.Role)).ToList();
    }
}