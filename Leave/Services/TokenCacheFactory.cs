﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Leave.Services
{
    public class TokenCacheFactory : ITokenCacheFactory
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public TokenCacheFactory(IDistributedCache distributedCache, IMemoryCache memoryCache, IDataProtectionProvider dataProtectionProvider)
        {
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public TokenCache CreateForUser(ClaimsPrincipal user)
        {
            string userId = user.GetObjectId();
            return new AdalDistributedTokenCache(
                _distributedCache, _memoryCache, _dataProtectionProvider, userId);
        }
    }
}
