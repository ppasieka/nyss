﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Data.Queries;
using RX.Nyss.Web.Features.ProjectAlertRecipients.Dto;
using RX.Nyss.Web.Features.Users.Dto;
using RX.Nyss.Web.Services.Authorization;
using static RX.Nyss.Common.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.Users
{
    public interface IUserService
    {
        Task<Result<List<GetNationalSocietyUsersResponseDto>>> List(int nationalSocietyId);
        Task<Result<NationalSocietyUsersCreateFormDataResponseDto>> GetCreateFormData(int nationalSocietyId);
        Task<Result> AddExisting(int nationalSocietyId, string userEmail);
        Task<string> GetUserApplicationLanguageCode(string userIdentityName);
        Task<Result<NationalSocietyUsersEditFormDataResponseDto>> GetEditFormData(int nationalSocietyUserId, int nationalSocietyId);
    }

    public class UserService : IUserService
    {
        private readonly INyssContext _nyssContext;
        private readonly IAuthorizationService _authorizationService;

        public UserService(INyssContext nyssContext, IAuthorizationService authorizationService)
        {
            _nyssContext = nyssContext;
            _authorizationService = authorizationService;
        }

        public async Task<Result<List<GetNationalSocietyUsersResponseDto>>> List(int nationalSocietyId)
        {
            var usersQuery = await GetFilteredUsersQuery(nationalSocietyId);

            var users = await usersQuery
                .Select(uns => new GetNationalSocietyUsersResponseDto
                {
                    Id = uns.User.Id,
                    Name = uns.User.Name,
                    Email = uns.User is DataConsumerUser && uns.User.IsFirstLogin
                        ? "***"
                        : uns.User.EmailAddress,
                    PhoneNumber = uns.User is DataConsumerUser && uns.User.IsFirstLogin
                        ? "***"
                        : uns.User.PhoneNumber,
                    Role = uns.User.Role.ToString(),
                    Project = uns.User is SupervisorUser
                        ? ((SupervisorUser)uns.User).CurrentProject != null ? ((SupervisorUser)uns.User).CurrentProject.Name : null
                        : null,
                    OrganizationName = uns.Organization.Name,
                    OrganizationId = uns.Organization.Id,
                    IsHeadManager = uns.Organization.HeadManagerId.HasValue && uns.Organization.HeadManagerId == uns.User.Id,
                    IsPendingHeadManager = uns.Organization.PendingHeadManagerId.HasValue && uns.Organization.PendingHeadManagerId == uns.User.Id,
                    IsVerified = !uns.User.IsFirstLogin
                })
                .OrderByDescending(u => u.IsHeadManager).ThenBy(u => u.Name)
                .ToListAsync();

            return new Result<List<GetNationalSocietyUsersResponseDto>>(users, true);
        }

        public async Task<Result<NationalSocietyUsersCreateFormDataResponseDto>> GetCreateFormData(int nationalSocietyId)
        {
            var currentUser = _authorizationService.GetCurrentUser();
            var organizationId = await _nyssContext.UserNationalSocieties
                .Where(un => un.UserId == currentUser.Id && un.NationalSocietyId == nationalSocietyId)
                .Select(un => un.OrganizationId)
                .SingleOrDefaultAsync();

            var formData = await _nyssContext.NationalSocieties
                .Where(ns => ns.Id == nationalSocietyId)
                .Select(ns => new NationalSocietyUsersCreateFormDataResponseDto
                {
                    Projects = _nyssContext.Projects
                        .Where(p => p.NationalSociety.Id == ns.Id && (currentUser.Role == Role.Administrator || p.ProjectOrganizations.Any(po => po.OrganizationId == organizationId)))
                        .Where(p => p.State == ProjectState.Open)
                        .Select(p => new ListOpenProjectsResponseDto
                        {
                            Id = p.Id,
                            Name = p.Name
                        }).ToList(),
                    Organizations = ns.Organizations
                        .Select(o => new OrganizationsDto
                        {
                            Id = o.Id,
                            Name = o.Name,
                            IsDefaultOrganization = o == ns.DefaultOrganization,
                            HasHeadManager = o.HeadManagerId.HasValue || o.PendingHeadManagerId.HasValue
                        }).ToList(),
                    HasCoordinator = ns.NationalSocietyUsers.Any(u => u.User.Role == Role.Coordinator),
                    IsHeadManager = ns.DefaultOrganization.HeadManager == currentUser
                }).SingleAsync();

            return Success(formData);
        }

        public async Task<Result<NationalSocietyUsersEditFormDataResponseDto>> GetEditFormData(int nationalSocietyUserId, int nationalSocietyId)
        {
            var user = await _nyssContext.Users
                .FilterAvailable()
                .Where(u => u.Id == nationalSocietyUserId && u.UserNationalSocieties.Any(uns => uns.NationalSocietyId == nationalSocietyId))
                .Select(u => new
                {
                    User = u,
                    UserNationalSociety = u.UserNationalSocieties.FirstOrDefault(uns => uns.NationalSocietyId == nationalSocietyId)
                })
                .Select(u => new NationalSocietyUsersEditFormDataResponseDto
                {
                    Email = u.User.EmailAddress,
                    Role = u.User.Role,
                    Organizations = _nyssContext.Organizations
                        .Where(o => o.NationalSociety.Id == nationalSocietyId)
                        .Select(o => new OrganizationsDto
                        {
                            Id = o.Id,
                            Name = o.Name
                        }).ToList()
                }).SingleOrDefaultAsync();

            return Success(user);
        }

        public async Task<Result> AddExisting(int nationalSocietyId, string userEmail)
        {
            var userData = await _nyssContext.Users.FilterAvailable()
                .Where(u => u.EmailAddress == userEmail)
                .Select(u => new
                {
                    u.Id,
                    u.Role
                })
                .SingleOrDefaultAsync();

            if (userData == null)
            {
                return Error(ResultKey.User.Registration.UserNotFound);
            }

            if (userData.Role != Role.TechnicalAdvisor && userData.Role != Role.DataConsumer)
            {
                return Error(ResultKey.User.Registration.NoAssignableUserWithThisEmailFound);
            }

            if (userData.Role == Role.TechnicalAdvisor && !_authorizationService.IsCurrentUserInAnyRole(Role.Manager, Role.TechnicalAdvisor))
            {
                return Error(ResultKey.User.Registration.TechnicalAdvisorsCanBeAttachedOnlyByManagers);
            }

            var userAlreadyIsInThisNationalSociety = await _nyssContext.UserNationalSocieties
                .FilterAvailableUsers()
                .AnyAsync(uns => uns.NationalSocietyId == nationalSocietyId && uns.UserId == userData.Id);

            if (userAlreadyIsInThisNationalSociety)
            {
                return Error(ResultKey.User.Registration.UserIsAlreadyInThisNationalSociety);
            }

            var nationalSocietyIsArchived = await _nyssContext.NationalSocieties.AnyAsync(ns => ns.Id == nationalSocietyId && ns.IsArchived);
            if (nationalSocietyIsArchived)
            {
                return Error(ResultKey.User.Registration.CannotAddExistingUsersToArchivedNationalSociety);
            }

            var userNationalSociety = new UserNationalSociety
            {
                NationalSocietyId = nationalSocietyId,
                UserId = userData.Id
            };

            var currentUser = await _authorizationService.GetCurrentUserAsync();
            userNationalSociety.Organization = await _nyssContext.UserNationalSocieties
                .Where(uns => uns.UserId == currentUser.Id && uns.NationalSocietyId == nationalSocietyId)
                .Select(uns => uns.Organization)
                .SingleOrDefaultAsync();

            await _nyssContext.UserNationalSocieties.AddAsync(userNationalSociety);
            await _nyssContext.SaveChangesAsync();
            return Success();
        }

        public async Task<string> GetUserApplicationLanguageCode(string userIdentityName) =>
            await _nyssContext.Users.FilterAvailable()
                .Where(u => u.EmailAddress == userIdentityName)
                .Select(u => u.ApplicationLanguage.LanguageCode)
                .SingleAsync();

        private async Task<IQueryable<UserNationalSociety>> GetFilteredUsersQuery(int nationalSocietyId)
        {
            var currentUser = await _authorizationService.GetCurrentUserAsync();
            var query = _nyssContext.UserNationalSocieties
                .Where(uns => uns.NationalSocietyId == nationalSocietyId);

            return currentUser switch
            {
                AdministratorUser _ => query,
                GlobalCoordinatorUser _ => (query.Any(uns => uns.User is CoordinatorUser)
                    ? query.Where(uns => uns.User is CoordinatorUser)
                    : query.Where(uns => uns.NationalSociety.DefaultOrganization.HeadManager == uns.User
                        || uns.NationalSociety.DefaultOrganization.PendingHeadManager == uns.User)),
                CoordinatorUser _ => query.Where(u => u.User is CoordinatorUser || u.User is DataConsumerUser
                    || u.Organization.HeadManager == u.User || u.Organization.PendingHeadManager == u.User),
                ManagerUser _ => query.Where(uns =>
                    uns.User is CoordinatorUser || uns.User is DataConsumerUser
                    || uns.OrganizationId == query.Where(x => x.User == currentUser).Select(x => x.OrganizationId).FirstOrDefault()),
                _ => query.Where(uns => uns.OrganizationId == query.Where(x => x.User == currentUser).Select(x => x.OrganizationId).FirstOrDefault())
            };
        }
    }
}
