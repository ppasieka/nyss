﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Features.Resources.Dto;
using RX.Nyss.Web.Services;
using RX.Nyss.Web.Utils;

namespace RX.Nyss.Web.Features.Resources
{
    [Route("api/resources")]
    public class ResourcesController : BaseController
    {
        private readonly IResourcesService _resourcesService;
        private readonly IInMemoryCache _inMemoryCache;

        public ResourcesController(
            IResourcesService resourcesService,
            IInMemoryCache inMemoryCache)
        {
            _resourcesService = resourcesService;
            _inMemoryCache = inMemoryCache;
        }

        [HttpPost("saveString"), AllowAnonymous]
        public async Task<Result> SaveString([FromBody] SaveStringRequestDto dto)
        {
            var result = await _resourcesService.SaveString(dto);

            if (result.IsSuccess)
            {
                foreach (var translation in dto.Translations)
                {
                    _inMemoryCache.Remove($"GetStrings.{translation.LanguageCode.ToLower()}");
                }
            }

            return result;
        }

        [HttpPost("saveEmailString"), AllowAnonymous]
        public async Task<Result> SaveEmailString([FromBody] SaveStringRequestDto dto)
        {
            var result = await _resourcesService.SaveEmailString(dto);

            if (result.IsSuccess)
            {
                foreach (var translation in dto.Translations)
                {
                    _inMemoryCache.Remove($"GetStrings.{translation.LanguageCode.ToLower()}");
                }
            }

            return result;
        }

        [HttpPost("saveSmsString"), AllowAnonymous]
        public async Task<Result> SaveSmsString([FromBody] SaveStringRequestDto dto)
        {
            var result = await _resourcesService.SaveSmsString(dto);

            if (result.IsSuccess)
            {
                foreach (var translation in dto.Translations)
                {
                    _inMemoryCache.Remove($"GetStrings.{translation.LanguageCode.ToLower()}");
                }
            }

            return result;
        }

        [HttpGet("getString/{key}"), AllowAnonymous]
        public async Task<Result<GetStringResponseDto>> GetString(string key) =>
            await _resourcesService.GetString(key);

        [HttpGet("getEmailString/{key}"), AllowAnonymous]
        public async Task<Result<GetStringResponseDto>> GetEmailString(string key) =>
            await _resourcesService.GetEmailString(key);

        [HttpGet("getSmsString/{key}"), AllowAnonymous]
        public async Task<Result<GetStringResponseDto>> GetSmsString(string key) =>
            await _resourcesService.GetSmsString(key);

        [HttpGet("listStringsTranslations")]
        [NeedsRole(Role.Administrator)]
        public async Task<Result<ListTranslationsResponseDto>> ListStringsTranslations() =>
            await _resourcesService.ListStringsTranslations();

        [HttpGet("listEmailTranslations")]
        [NeedsRole(Role.Administrator)]
        public async Task<Result<ListTranslationsResponseDto>> ListEmailTranslations() =>
            await _resourcesService.ListEmailTranslations();

        [HttpGet("listSmsTranslations")]
        [NeedsRole(Role.Administrator)]
        public async Task<Result<ListTranslationsResponseDto>> ListSmsTranslations() =>
            await _resourcesService.ListSmsTranslations();
    }
}
