﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Features.Manager.Dto;
using RX.Nyss.Web.Features.User;
using RX.Nyss.Web.Services;
using RX.Nyss.Web.Utils.DataContract;
using RX.Nyss.Web.Utils.Logging;
using static RX.Nyss.Web.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.Manager
{
    public interface IManagerService
    {
        Task<Result> CreateManager(int nationalSocietyId, CreateManagerRequestDto createManagerRequestDto);
        Task<Result<GetManagerResponseDto>> GetManager(int managerId);
        Task<Result> UpdateManager(int managerId, EditManagerRequestDto editManagerRequestDto);
        Task<Result> DeleteManager(int managerId, IEnumerable<string> deletingUserRoles);
    }


    public class ManagerService : IManagerService
    {
        private readonly ILoggerAdapter _loggerAdapter;
        private readonly INyssContext _dataContext;
        private readonly IIdentityUserRegistrationService _identityUserRegistrationService;
        private readonly INationalSocietyUserService _nationalSocietyUserService;
        private readonly IVerificationEmailService _verificationEmailService;
        private readonly IUserService _userService;

        public ManagerService(IIdentityUserRegistrationService identityUserRegistrationService, INationalSocietyUserService nationalSocietyUserService, INyssContext dataContext, ILoggerAdapter loggerAdapter, IVerificationEmailService verificationEmailService, IUserService userService)
        {
            _identityUserRegistrationService = identityUserRegistrationService;
            _nationalSocietyUserService = nationalSocietyUserService;
            _dataContext = dataContext;
            _loggerAdapter = loggerAdapter;
            _verificationEmailService = verificationEmailService;
            _userService = userService;
        }

        public async Task<Result> CreateManager(int nationalSocietyId, CreateManagerRequestDto createManagerRequestDto)
        {
            try
            {
                string securityStamp;
                ManagerUser user;
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var identityUser = await _identityUserRegistrationService.CreateIdentityUser(createManagerRequestDto.Email, Role.Manager);
                    securityStamp = await _identityUserRegistrationService.GenerateEmailVerification(identityUser.Email);

                    user = await CreateManagerUser(identityUser, nationalSocietyId, createManagerRequestDto);
                    
                    transactionScope.Complete();
                }
                await _verificationEmailService.SendVerificationEmail(user, securityStamp);
                return Success(ResultKey.User.Registration.Success);
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }
        }

        private async Task<ManagerUser> CreateManagerUser(IdentityUser identityUser, int nationalSocietyId, CreateManagerRequestDto createManagerRequestDto)
        {
            var nationalSociety = await _dataContext.NationalSocieties.Include(ns => ns.ContentLanguage)
                .SingleOrDefaultAsync(ns => ns.Id == nationalSocietyId);

            if (nationalSociety == null)
            {
                throw new ResultException(ResultKey.User.Registration.NationalSocietyDoesNotExist);
            }

            var defaultUserApplicationLanguage = await _dataContext.ApplicationLanguages
                .SingleOrDefaultAsync(al => al.LanguageCode == nationalSociety.ContentLanguage.LanguageCode);

            var user = new ManagerUser
            {
                IdentityUserId = identityUser.Id,
                EmailAddress = identityUser.Email,
                Name = createManagerRequestDto.Name,
                PhoneNumber = createManagerRequestDto.PhoneNumber,
                AdditionalPhoneNumber = createManagerRequestDto.AdditionalPhoneNumber,
                Organization = createManagerRequestDto.Organization,
                ApplicationLanguage = defaultUserApplicationLanguage,
            };

            var userNationalSociety = CreateUserNationalSocietyReference(nationalSociety, user);

            await _dataContext.AddAsync(userNationalSociety);
            await _dataContext.SaveChangesAsync();
            return user;
        }

        private UserNationalSociety CreateUserNationalSocietyReference(Nyss.Data.Models.NationalSociety nationalSociety, Nyss.Data.Models.User user) =>
            new UserNationalSociety
            {
                NationalSociety = nationalSociety,
                User = user
            };

        public async Task<Result<GetManagerResponseDto>> GetManager(int nationalSocietyUserId)
        {
            var manager = await _dataContext.Users
                .OfType<ManagerUser>()
                .Where(u => u.Id == nationalSocietyUserId)
                .Select(u => new GetManagerResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role,
                    Email = u.EmailAddress,
                    PhoneNumber = u.PhoneNumber,
                    AdditionalPhoneNumber = u.AdditionalPhoneNumber,
                    Organization = u.Organization,
                })
                .SingleOrDefaultAsync();

            if (manager == null)
            {
                _loggerAdapter.Debug($"Data manager with id {nationalSocietyUserId} was not found");
                return Error<GetManagerResponseDto>(ResultKey.User.Common.UserNotFound);
            }

            return new Result<GetManagerResponseDto>(manager, true);
        }

        public async Task<Result> UpdateManager(int managerId, EditManagerRequestDto editManagerRequestDto)
        {
            try
            {
                var user = await _nationalSocietyUserService.GetNationalSocietyUser<ManagerUser>(managerId);

                user.Name = editManagerRequestDto.Name;
                user.PhoneNumber = editManagerRequestDto.PhoneNumber;
                user.Organization = editManagerRequestDto.Organization;
                user.AdditionalPhoneNumber = editManagerRequestDto.AdditionalPhoneNumber;

                await _dataContext.SaveChangesAsync();
                return Success();
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }
        }

        public async Task<Result> DeleteManager(int managerId, IEnumerable<string> deletingUserRoles)
        {
            try
            {
                using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var manager = await _nationalSocietyUserService.GetNationalSocietyUserIncludingNationalSocieties<ManagerUser>(managerId);
                _userService.EnsureHasPermissionsToDelteUser(manager.Role, deletingUserRoles);

                await HandleHeadManagerStatus(manager);

                _nationalSocietyUserService.DeleteNationalSocietyUser<ManagerUser>(manager);
                await _identityUserRegistrationService.DeleteIdentityUser(manager.IdentityUserId);

                await _dataContext.SaveChangesAsync();

                transactionScope.Complete();
                return Success(ResultKey.User.Registration.Success);
            }
            catch (ResultException e)
            {
                _loggerAdapter.Debug(e);
                return e.Result;
            }

            async Task HandleHeadManagerStatus(ManagerUser manager)
            {
                var nationalSociety = await _dataContext.NationalSocieties.FindAsync(manager.UserNationalSocieties.Single().NationalSocietyId);
                if (nationalSociety.PendingHeadManager == manager)
                {
                    nationalSociety.PendingHeadManager = null;
                }
                if (nationalSociety.HeadManager == manager)
                {
                    throw new ResultException(ResultKey.User.Deletion.CannotDeleteHeadManager);
                }
            }
        }
    }
}
