using ItsmAi.Domain.Exceptions;

namespace ItsmAi.Domain.Entities;

public class Requester
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Requester(
        string name,
        string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Requester name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException(
                "Requester email cannot be empty.");
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        CreatedAt = DateTime.UtcNow;
    }
}