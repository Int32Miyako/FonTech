using FonTech.Domain.Dto.Report;
using MediatR;

namespace FonTech.Application.Queries;

public class GetReportsByIdQuery(long userId) : IRequest<ReportDto>
{
    
}