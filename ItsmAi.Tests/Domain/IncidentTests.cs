using System;
using System.Collections.Generic;
using System.Text;
using ItsmAi.Domain.Entities;
using ItsmAi.Domain.Enums;
using ItsmAi.Domain.Exceptions;

namespace ItsmAi.Tests.Domain;

public class IncidentTests
{
    [Fact]
    public void Create_ShouldCreateNewIncident()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        Assert.Equal(IncidentStatus.New, incident.Status);
        Assert.Equal(IncidentPriority.Medium, incident.Priority);
        Assert.NotEqual(Guid.Empty, incident.Id);
    }

    [Fact]
    public void StartProgress_WhenIncidentIsNew_ShouldChangeStatus()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        incident.StartProgress();

        Assert.Equal(IncidentStatus.InProgress, incident.Status);
    }

    [Fact]
    public void Resolve_WhenIncidentIsNew_ShouldThrowException()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        Assert.Throws<DomainException>(
            () => incident.Resolve());
    }

    [Fact]
    public void Incident_ShouldFollowValidLifecycle()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        incident.StartProgress();
        Assert.Equal(IncidentStatus.InProgress, incident.Status);

        incident.Resolve();
        Assert.Equal(IncidentStatus.Resolved, incident.Status);

        incident.Close();
        Assert.Equal(IncidentStatus.Closed, incident.Status);
    }

    [Fact]
    public void ChangePriority_WhenIncidentIsOpen_ShouldChangePriority()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Low);

        incident.ChangePriority(IncidentPriority.High);

        Assert.Equal(IncidentPriority.High, incident.Priority);
    }

    [Fact]
    public void Create_ShouldGenerateIncidentNumber()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        Assert.False(string.IsNullOrWhiteSpace(incident.Number));
        Assert.StartsWith("INC-", incident.Number);
    }

    [Fact]
    public void ChangeStatus_WhenIncidentIsNewAndTargetIsInProgress_ShouldChangeStatus()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        incident.ChangeStatus(IncidentStatus.InProgress);

        Assert.Equal(
            IncidentStatus.InProgress,
            incident.Status);
    }

    [Fact]
    public void ChangeStatus_WhenTargetIsNew_ShouldThrowDomainException()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        Assert.Throws<DomainException>(
            () => incident.ChangeStatus(IncidentStatus.New));
    }

    [Fact]
    public void AddComment_WithValidContent_ShouldAddComment()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        var comment = incident.AddComment(
            "User restarted the laptop.");

        Assert.Single(incident.Comments);

        Assert.Equal(
            "User restarted the laptop.",
            comment.Content);

        Assert.Equal(
            incident.Id,
            comment.IncidentId);
    }

    [Fact]
    public void AddComment_WithEmptyContent_ShouldThrowDomainException()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        Assert.Throws<DomainException>(
            () => incident.AddComment("   "));
    }

    [Fact]
    public void AddComment_ShouldTrimContent()
    {
        var incident = new Incident(
            "VPN problem",
            "User cannot connect to VPN",
            IncidentPriority.Medium);

        var comment = incident.AddComment(
            "   Restarted VPN service.   ");

        Assert.Equal(
            "Restarted VPN service.",
            comment.Content);
    }
}
