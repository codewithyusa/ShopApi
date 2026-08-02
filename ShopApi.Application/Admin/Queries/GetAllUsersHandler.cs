using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Admin.Queries;

public class GetAllUsersHandler(IUserRepository users)
    : IRequestHandler<GetAllUsersQuery, PagedResponse<UserResponseDto>>
{
    public async Task<PagedResponse<UserResponseDto>> Handle(GetAllUsersQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await users.GetAllPagedAsync(query.Paging, ct);

        return new PagedResponse<UserResponseDto>
        {
            Items = items.Select(u => new UserResponseDto(u.Id, u.Name, u.Email, u.Phone, u.Role)).ToList(),
            TotalCount = totalCount,
            Page = query.Paging.Page,
            PageSize = query.Paging.PageSize
        };
    }
}