using Library.ApplicationCore.Enums;

namespace Library.ApplicationCore;
public interface IPatronService
{
    Task<MembershipRenewalStatus> RenewMembership(int patronId);
}