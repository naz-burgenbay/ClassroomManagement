using Microsoft.AspNetCore.Identity;

// Dummy stores for no-database mode
public class NullUserStore<TUser> : IUserStore<TUser> where TUser : class
{
    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Failed());
    public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Failed());
    public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Failed());
    public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<TUser>(null);
    public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<TUser>(null);
    public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult<string>(null);
    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult<string>(null);
    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult<string>(null);
    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken) => Task.CompletedTask;
    public void Dispose() { }
}

public class NullRoleStore<TRole> : IRoleStore<TRole> where TRole : class
{
    public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Failed());
    public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Failed());
    public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Failed());
    public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) => Task.FromResult<TRole>(null);
    public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) => Task.FromResult<TRole>(null);
    public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult<string>(null);
    public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult<string>(null);
    public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult<string>(null);
    public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task SetRoleNameAsync(TRole role, string name, CancellationToken cancellationToken) => Task.CompletedTask;
    public void Dispose() { }
}
