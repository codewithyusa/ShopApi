using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Admin.Queries;

public record GetAllUsersQuery(PagedRequest Paging) : IRequest<PagedResponse<UserResponseDto>>;