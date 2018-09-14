using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using TuanZi.Data;
using TuanZi.Entity;
using TuanZi.EventBuses;
using TuanZi.Extensions;
using TuanZi.Identity.Events;

namespace TuanZi.Identity
{
    public abstract class UserStoreAppBase<TUser, TUserKey, TUserClaim, TUserLogin, TUserToken, TRole, TRoleKey, TUserRole>
        : IQueryableUserStore<TUser>,
          IUserLoginStore<TUser>,
          IUserClaimStore<TUser>,
          IUserPasswordStore<TUser>,
          IUserSecurityStampStore<TUser>,
          IUserEmailStore<TUser>,
          IUserLockoutStore<TUser>,
          IUserPhoneNumberStore<TUser>,
          IUserTwoFactorStore<TUser>,
          IUserAuthenticationTokenStore<TUser>,
          IUserAuthenticatorKeyStore<TUser>,
          IUserTwoFactorRecoveryCodeStore<TUser>,
          IUserRoleStore<TUser>
        where TUser : UserBase<TUserKey>
        where TUserClaim : UserClaimBase<TUserKey>, new()
        where TUserLogin : UserLoginBase<TUserKey>, new()
        where TUserToken : UserTokenBase<TUserKey>, new()
        where TRole : RoleBase<TRoleKey>
        where TUserRole : UserRoleBase<TUserKey, TRoleKey>, new()
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>
    {
        private readonly IRepository<TUser, TUserKey> _userRepository;
        private readonly IRepository<TUserLogin, Guid> _userLoginRepository;
        private readonly IRepository<TUserClaim, int> _userClaimRepository;
        private readonly IRepository<TUserToken, Guid> _userTokenRepository;
        private readonly IRepository<TRole, TRoleKey> _roleRepository;
        private readonly IRepository<TUserRole, Guid> _userRoleRepository;
        private readonly IEventBus _eventBus;
        private bool _disposed;

        public Guid? AppId { get; set; }

        protected UserStoreAppBase(
            IRepository<TUser, TUserKey> userRepository,
            IRepository<TUserLogin, Guid> userLoginRepository,
            IRepository<TUserClaim, int> userClaimRepository,
            IRepository<TUserToken, Guid> userTokenRepository,
            IRepository<TRole, TRoleKey> roleRepository,
            IRepository<TUserRole, Guid> userRoleRepository,
            IEventBus eventBus
        )
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _userClaimRepository = userClaimRepository;
            _userTokenRepository = userTokenRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _eventBus = eventBus;
        }

        #region Implementation of IQueryableUserStore<TUser>

        public IQueryable<TUser> Users => _userRepository.TrackQuery();

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _disposed = true;
        }

        #endregion

        #region Implementation of IUserStore<TUser>

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(ConvertIdToString(user.Id));
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            await _userRepository.InsertAsync(user);

            int count = _userRepository.Query().Count();
            if (count == 1)
            {
                TRole adminRole = _roleRepository.Query().FirstOrDefault();
                if (adminRole != null)
                {
                    TUserRole userRole = new TUserRole() { UserId = user.Id, RoleId = adminRole.Id };
                    await _userRoleRepository.InsertAsync(userRole);

                    user.IsSystem = true;
                    await _userRepository.UpdateAsync(user);
                }
            }

            TRole defaultRole = _roleRepository.Query().FirstOrDefault(m => m.IsDefault);
            if (defaultRole != null)
            {
                TUserRole userRole = new TUserRole() { UserId = user.Id, RoleId = defaultRole.Id };
                await _userRoleRepository.InsertAsync(userRole);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            if (user.Email.IsMissing())
            {
                user.EmailConfirmed = false;
            }
            if (user.PhoneNumber.IsMissing())
            {
                user.PhoneNumberConfirmed = false;
            }

            await _userRepository.UpdateAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            if (user.IsSystem)
            {
                return new IdentityResult().Failed($"User '{user.UserName}' is a system user and cannot be deleted");
            }
            await _userRepository.DeleteAsync(user);
            return IdentityResult.Success;
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            TUserKey id = ConvertIdFromString(userId);
            return Task.FromResult(_userRepository.Get(id));
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();


             return Task.FromResult(_userRepository.TrackQuery().FirstOrDefault(m => m.NormalizedUserName == normalizedUserName && m.AppId == AppId));
        }

        #endregion

        #region Implementation of IUserLoginStore<TUser>

        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            Check.NotNull(login, nameof(login));

            TUserLogin userLogin = new TUserLogin()
            {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };
            await _userLoginRepository.InsertAsync(userLogin);
        }

        public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            Check.NotNullOrEmpty(loginProvider, nameof(loginProvider));
            Check.NotNullOrEmpty(providerKey, nameof(providerKey));

            await _userLoginRepository.DeleteBatchAsync(m => m.UserId.Equals(user.Id) && m.LoginProvider == loginProvider && m.ProviderKey == providerKey);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            IList<UserLoginInfo> loginInfos = _userLoginRepository.Query(m => m.UserId.Equals(user.Id)).Select(m =>
                new UserLoginInfo(m.LoginProvider, m.ProviderKey, m.ProviderDisplayName)).ToList();
            return Task.FromResult(loginInfos);
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNullOrEmpty(loginProvider, nameof(loginProvider));
            Check.NotNullOrEmpty(providerKey, nameof(providerKey));

            TUserKey userId = _userLoginRepository.TrackQuery(m => m.LoginProvider == loginProvider && m.ProviderKey == providerKey)
                .Select(m => m.UserId).FirstOrDefault();
            if (Equals(userId, default(TUserKey)))
            {
                return Task.FromResult(default(TUser));
            }
            TUser user = _userRepository.Get(userId);
            return Task.FromResult(user);
        }

        #endregion

        #region Implementation of IUserClaimStore<TUser>

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            IList<Claim> claims = _userClaimRepository.Query(m => m.UserId.Equals(user.Id))
                .Select(m => new Claim(m.ClaimType, m.ClaimValue)).ToList();
            return Task.FromResult(claims);
        }

        public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            TUserClaim[] userClaims = claims.Select(m => new TUserClaim() { UserId = user.Id, ClaimType = m.Type, ClaimValue = m.Value }).ToArray();
            await _userClaimRepository.InsertAsync(userClaims);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            List<TUserClaim> userClaims = _userClaimRepository.TrackQuery(m => m.UserId.Equals(user.Id) && m.ClaimType == claim.Type && m.ClaimValue == claim.Value).ToList();
            foreach (TUserClaim userClaim in userClaims)
            {
                userClaim.ClaimType = newClaim.Type;
                userClaim.ClaimValue = newClaim.Value;
            }
            return Task.CompletedTask;
        }

        public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            await _userClaimRepository.DeleteBatchAsync(m =>
                m.UserId.Equals(user.Id) && claims.Any(n => n.Type == m.ClaimType && n.Value == m.ClaimValue));
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(claim, nameof(claim));

            TUserKey[] userIds = _userClaimRepository.Query(m => m.ClaimType == claim.Type && m.ClaimValue == claim.Value)
                .Select(m => m.UserId).ToArray();
            IList<TUser> users = _userRepository.TrackQuery(m => userIds.Contains(m.Id)).ToList();
            return Task.FromResult(users);
        }

        #endregion

        #region Implementation of IUserPasswordStore<TUser>

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        #endregion

        #region Implementation of IUserSecurityStampStore<TUser>

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.SecurityStamp = stamp;

            OnlineUserCacheRemoveEventData eventData = new OnlineUserCacheRemoveEventData() { UserNames = new[] { user.UserName } };
            _eventBus.Publish(eventData);

            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region Implementation of IUserEmailStore<TUser>

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            Check.NotNull(email, nameof(email));

            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.EmailConfirmed = true;
            return Task.CompletedTask;
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            TUser user = _userRepository.TrackQuery().FirstOrDefault(m => m.NormalizeEmail == normalizedEmail && m.AppId == AppId);
            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.NormalizeEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        #endregion

        #region Implementation of IUserLockoutStore<TUser>

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        #endregion

        #region Implementation of IUserPhoneNumberStore<TUser>

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        #endregion

        #region Implementation of IUserTwoFactorStore<TUser>

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        #endregion

        #region Implementation of IUserAuthenticationTokenStore<TUser>

        public async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            TUserToken token = _userTokenRepository.TrackQuery()
                .FirstOrDefault(m => m.Id.Equals(user.Id) && m.LoginProvider == loginProvider && m.Name == name);
            if (token == null)
            {
                token = new TUserToken() { UserId = user.Id, LoginProvider = loginProvider, Name = name, Value = value };
                await _userTokenRepository.InsertAsync(token);
            }
            else
            {
                token.Value = value;
                await _userTokenRepository.UpdateAsync(token);
            }
        }

        public async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            await _userTokenRepository.DeleteBatchAsync(m => m.UserId.Equals(user.Id) && m.LoginProvider == loginProvider && m.Name == name);
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            string value = _userTokenRepository.Query(m => m.UserId.Equals(user.Id) && m.LoginProvider == loginProvider && m.Name == name)
                .Select(m => m.Value).FirstOrDefault();
            return Task.FromResult(value);
        }

        #endregion

        #region Implementation of IUserAuthenticatorKeyStore<TUser>

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        #endregion

        #region Implementation of IUserTwoFactorRecoveryCodeStore<TUser>

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            string mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            Check.NotNullOrEmpty(code, nameof(code));

            string mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? String.Empty;
            string[] splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                List<string> updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            string mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken);
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }

        #endregion

        #region Implementation of IUserRoleStore<TUser>

        public async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            Check.NotNullOrEmpty(normalizedRoleName, nameof(normalizedRoleName));

            TRoleKey roleId = _roleRepository.Query(m => m.NormalizedName == normalizedRoleName).Select(m => m.Id).FirstOrDefault();
            if (Equals(roleId, default(TRoleKey)))
            {
                throw new InvalidOperationException($"Role '{normalizedRoleName}' does not exists");
            }
            TUserRole userRole = new TUserRole() { RoleId = roleId, UserId = user.Id };
            await _userRoleRepository.InsertAsync(userRole);
        }

        public async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));
            Check.NotNullOrEmpty(normalizedRoleName, nameof(normalizedRoleName));

            TRole role = _roleRepository.Query().FirstOrDefault(m => m.NormalizedName == normalizedRoleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role'{normalizedRoleName}' does not exists");
            }
            if (user.IsSystem && role.IsSystem)
            {
                throw new InvalidOperationException($"System role '{role.Name}' of system user '{user.UserName}' cannot be removed");
            }
            await _userRoleRepository.DeleteBatchAsync(m => m.UserId.Equals(user.Id) && m.RoleId.Equals(role.Id));
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            IList<string> list = new List<string>();
            List<TRoleKey> roleIds = _userRoleRepository.Query(m => m.UserId.Equals(user.Id)).Select(m => m.RoleId).ToList();
            if (roleIds.Count == 0)
            {
                return Task.FromResult(list);
            }
            list = _roleRepository.Query(m => roleIds.Contains(m.Id)).Select(m => m.Name).ToList();
            return Task.FromResult(list);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(user, nameof(user));

            TRoleKey roleId = _roleRepository.Query(m => m.NormalizedName == roleName).Select(m => m.Id).FirstOrDefault();
            if (Equals(roleId, default(TRoleKey)))
            {
                throw new InvalidOperationException($"Role '{roleName}' does not exists");
            }
            bool exist = _userRoleRepository.Query(m => m.UserId.Equals(user.Id) && m.RoleId.Equals(roleId)).Any();
            return Task.FromResult(exist);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNullOrEmpty(roleName, nameof(roleName));

            TRoleKey roleId = _roleRepository.Query(m => m.NormalizedName == roleName).Select(m => m.Id).FirstOrDefault();
            if (Equals(roleId, default(TRoleKey)))
            {
                throw new InvalidOperationException($"Role '{roleName}' does not exists");
            }
            List<TUserKey> userIds = _userRoleRepository.Query(m => m.RoleId.Equals(roleId)).Select(m => m.UserId).ToList();
            IList<TUser> users = _userRepository.TrackQuery(m => userIds.Contains(m.Id)).ToList();
            return Task.FromResult(users);
        }

        #endregion

        #region Other

        public virtual TUserKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TUserKey);
            }
            return (TUserKey)TypeDescriptor.GetConverter(typeof(TUserKey)).ConvertFromInvariantString(id);
        }

        public virtual string ConvertIdToString(TUserKey id)
        {
            if (id.Equals(default(TUserKey)))
            {
                return null;
            }
            return id.ToString();
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion
    }
}