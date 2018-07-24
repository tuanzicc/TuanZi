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


namespace TuanZi.Identity
{
    public abstract class RoleStoreBase<TRole, TRoleKey, TRoleClaim>
       : IQueryableRoleStore<TRole>,
         IRoleClaimStore<TRole>
       where TRole : RoleBase<TRoleKey>
       where TRoleClaim : RoleClaimBase<TRoleKey>, new()
       where TRoleKey : IEquatable<TRoleKey>
    {
        private readonly IRepository<TRole, TRoleKey> _roleRepository;
        private readonly IRepository<TRoleClaim, int> _roleClaimRepository;
        private bool _disposed;

        protected RoleStoreBase(
            IRepository<TRole, TRoleKey> roleRepository,
            IRepository<TRoleClaim, int> roleClaimRepository)
        {
            _roleRepository = roleRepository;
            _roleClaimRepository = roleClaimRepository;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            _disposed = true;
        }

        #endregion

        #region Implementation of IQueryableRoleStore<TRole>

        public IQueryable<TRole> Roles => _roleRepository.Query();

        #endregion

        #region Implementation of IRoleStore<TRole>

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            if (role.IsDefault)
            {
                string defaultRole = _roleRepository.Query(m => m.IsDefault, false).Select(m => m.Name).FirstOrDefault();
                if (defaultRole != null)
                {
                    return new IdentityResult().Failed($"The default role '{defaultRole}' already exists in the system.");
                }
            }
            await _roleRepository.InsertAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            if (role.IsSystem)
            {
                return new IdentityResult().Failed($"Role '{role.Name}' is a system role and cannot be updated");
            }
            if (role.IsDefault)
            {
                var defaultRole = _roleRepository.Query(m => m.IsDefault, false).Select(m => new { m.Id, m.Name }).FirstOrDefault();
                if (defaultRole != null && !defaultRole.Id.Equals(role.Id))
                {
                    return new IdentityResult().Failed($"The default role '{defaultRole}' already exists in the system.");
                }
            }
            await _roleRepository.UpdateAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            if (role.IsSystem)
            {
                return new IdentityResult().Failed($"Role '{role.Name}' is a system role and cannot be deleted");
            }
            await _roleRepository.DeleteAsync(role);
            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            return Task.FromResult(ConvertIdToString(role.Id));
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            TRoleKey id = ConvertIdFromString(roleId);
            return Task.FromResult(Roles.FirstOrDefault(m => m.Id.Equals(id)));
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(Roles.FirstOrDefault(m => m.NormalizedName == normalizedRoleName));
        }

        #endregion

        #region Implementation of IRoleClaimStore<TRole>

        public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));

            IList<Claim> list = _roleClaimRepository.Query(m => m.RoleId.Equals(role.Id)).Select(n => new Claim(n.ClaimType, n.ClaimValue)).ToList();
            return Task.FromResult(list);
        }

        public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            TRoleClaim roleClaim = new TRoleClaim() { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
            await _roleClaimRepository.InsertAsync(roleClaim);
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            Check.NotNull(role, nameof(role));
            Check.NotNull(claim, nameof(claim));

            return _roleClaimRepository.DeleteBatchAsync(m => m.RoleId.Equals(role.Id) && m.ClaimValue == claim.Type && m.ClaimValue == claim.Value);
        }

        #endregion

        public virtual TRoleKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TRoleKey);
            }
            return (TRoleKey)TypeDescriptor.GetConverter(typeof(TRoleKey)).ConvertFromInvariantString(id);
        }

        public virtual string ConvertIdToString(TRoleKey id)
        {
            if (id.Equals(default(TRoleKey)))
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
    }
}