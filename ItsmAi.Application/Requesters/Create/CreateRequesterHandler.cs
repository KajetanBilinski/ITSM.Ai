using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Entities;
using ItsmAi.Domain.Exceptions;

namespace ItsmAi.Application.Requesters.Create;

public class CreateRequesterHandler
{
    private readonly IRequesterRepository _requesterRepository;

    public CreateRequesterHandler(
        IRequesterRepository requesterRepository)
    {
        _requesterRepository = requesterRepository;
    }

    public async Task<Guid> HandleAsync(
        CreateRequesterCommand command,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail =
            command.Email.Trim().ToLowerInvariant();

        var exists = await _requesterRepository.ExistsByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (exists)
        {
            throw new DomainException(
                "Requester with this email already exists.");
        }

        var requester = new Requester(
            command.Name,
            normalizedEmail);

        await _requesterRepository.AddAsync(
            requester,
            cancellationToken);

        return requester.Id;
    }
}