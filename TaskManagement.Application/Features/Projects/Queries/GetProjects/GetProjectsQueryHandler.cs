using MediatR;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Projects.Dtos;
using TaskManagement.Application.Features.Projects.Queries.GetProjects;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _unitOfWork.Projects.GetAllAsync(cancellationToken);

        return projects.Select(project => new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt
        }).ToList();
    }
}