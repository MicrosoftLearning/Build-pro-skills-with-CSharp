using NSubstitute;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Services;
using Library.ApplicationCore.Enums;
using Microsoft.Extensions.Logging;

namespace Library.UnitTests.ApplicationCore.PatronServiceTests;

public class RenewMembershipTest {
    private readonly IPatronRepository _mockPatronRepository;
    private readonly PatronService _patronService;

    public RenewMembershipTest()
    {
        _mockPatronRepository = Substitute.For<IPatronRepository>();
        var logger = Substitute.For<ILogger<PatronService>>();
        _patronService = new PatronService(_mockPatronRepository, logger);
    }

    [Fact(DisplayName = "PatronService.RenewMembership: Renews the membership successfully without loans")]
    public async Task RenewMembership_RenewsMembershipSuccessfully()
    {
        // Arrange
        var patron = PatronFactory.CreateCurrentPatron();
        var membershipEnd = patron.MembershipEnd;
        var patronId = patron.Id;
        _mockPatronRepository.GetPatron(patronId).Returns(patron);

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.Success, renewalStatus);
        Assert.Equal(membershipEnd.AddYears(1), patron.MembershipEnd);
    }

    [Fact(DisplayName = "PatronService.RenewMembership: Renews the membership successfully with expired membership")]
    public async Task RenewMembership_RenewsMembershipSuccessfullyWithExpiredMembership()
    {
        // Arrange
        var patron = PatronFactory.CreateExpiredPatron();
        var patronId = patron.Id;
        _mockPatronRepository.GetPatron(patronId).Returns(patron);
        DateTime membershipEnd = patron.MembershipEnd;

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.Success, renewalStatus);
        Assert.Equal(membershipEnd.AddYears(1), patron.MembershipEnd);
    }

    [Fact(DisplayName = "PatronService.RenewMembership: Renews the membership successfully with returned loans")]
    public async Task RenewMembership_RenewsMembershipSuccessfullyWithReturnedLoans()
    {
        // Arrange
        var membershipEnd = DateTime.Now.AddDays(1);
        var patron = PatronFactory.CreateCurrentPatron();
        patron.MembershipEnd = membershipEnd;
        var patronId = patron.Id;
        patron.Loans = new List<Loan> {
            LoanFactory.CreateReturnedLoanForPatron(patron)
        };
        _mockPatronRepository.GetPatron(patronId).Returns(patron);

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.Success, renewalStatus);
        Assert.Equal(membershipEnd.AddYears(1), patron.MembershipEnd);
    }


    [Fact(DisplayName = "PatronService.RenewMembership: Renews the membership successfully with current loans")]
    public async Task RenewMembership_RenewsMembershipSuccessfullyWithCurrentLoans()
    {
        // Arrange
        var membershipEnd = DateTime.Now.AddDays(1);
        var patron = PatronFactory.CreateCurrentPatron();
        patron.MembershipEnd = membershipEnd;
        var patronId = patron.Id;
        patron.Loans = new List<Loan> {
            LoanFactory.CreateCurrentLoanForPatron(patron)
        };
        _mockPatronRepository.GetPatron(patronId).Returns(patron);

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.Success, renewalStatus);
        Assert.Equal(membershipEnd.AddYears(1), patron.MembershipEnd);
    }

    [Fact(DisplayName = "PatronService.RenewMembership: Returns PatronNotFound if patron is not found")]
    public async Task RenewMembership_ReturnsPatronNotFound()
    {
        // Arrange
        var patronId = 42;
        _mockPatronRepository.GetPatron(patronId).Returns((Patron?)null);

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.PatronNotFound, renewalStatus);
    }

    [Fact(DisplayName = "PatronService.RenewMembership: Returns TooEarlyToRenew if renewal is not allowed yet")]
    public async Task RenewMembership_ReturnsTooEarlyToRenew()
    {
        // Arrange
        var patron = PatronFactory.CreateTooEarlyToRenewPatron();
        var patronId = patron.Id;
        _mockPatronRepository.GetPatron(patronId).Returns(patron);

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.TooEarlyToRenew, renewalStatus);
    }

    [Fact(DisplayName = "PatronService.RenewMembership: Returns LoanNotReturned if patron has overdue loans")]
    public async Task RenewMembership_ReturnsLoanNotReturned()
    {
        // Arrange
        var patron = PatronFactory.CreateCurrentPatron();
        var patronId = patron.Id;
        patron.Loans = new List<Loan> {
            LoanFactory.CreateExpiredLoanForPatron(patron)
        };
        _mockPatronRepository.GetPatron(patronId).Returns(patron);

        // Act
        MembershipRenewalStatus renewalStatus = await _patronService.RenewMembership(patronId);

        // Assert
        Assert.Equal(MembershipRenewalStatus.LoanNotReturned, renewalStatus);
    }
}