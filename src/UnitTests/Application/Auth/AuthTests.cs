using Application.Abstractions.Repository.Base;
using Application.Abstractions.Utils;
using Application.Services.AuthService;
using Ardalis.Specification;
using AutoMapper;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Auth.input;
using FluentAssertions;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Repository.Base;
using Microsoft.Extensions.Logging;
using Moq;


namespace TeachHub.UnitTests.Application.Auth;

public class EfCoreUserRepository : BaseRepository<UserEntity>, IBaseRepository<UserEntity>
{
    public EfCoreUserRepository(CourceDbContext context) : base(context) { }
}

public class EfCoreUserRoleRepository : BaseRepository<UserRoleEntity>, IBaseRepository<UserRoleEntity>
{
    public EfCoreUserRoleRepository(CourceDbContext context) : base(context) { }
}

    public class AuthServiceTests
{
    [Fact]
    public async Task LoginUser_ShouldReturnIpNotFound_WhenIpIsEmpty()
    {
        // Arrange
        var jwtMock = new Mock<IJwtService>();
        var userRepoMock = new Mock<IBaseRepository<UserEntity>>();
        var userRoleRepoMock = new Mock<IBaseRepository<UserRoleEntity>>();
        var passwordMock = new Mock<IPasswordHashService>();
        var loggerMock = new Mock<ILogger<AuthService>>();
        var mapperMock = new Mock<IMapper>();

        var authService = new AuthService(
            jwtMock.Object,
            userRepoMock.Object,
            userRoleRepoMock.Object,
            passwordMock.Object,
            loggerMock.Object,
            mapperMock.Object
        );

        var dto = new LoginUserDTO { email = "test@test.com" };

        // Act
        var result = await authService.LoginUser(dto, string.Empty, "agent");

        // Assert
        result.IsCompleted.Should().BeFalse();
        result.ErrorCode.Should().Be(errorCode.IpNotFound);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnUserNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var jwtMock = new Mock<IJwtService>();
        var userRepoMock = new Mock<IBaseRepository<UserEntity>>();
        var userRoleRepoMock = new Mock<IBaseRepository<UserRoleEntity>>();
        var passwordMock = new Mock<IPasswordHashService>();
        var loggerMock = new Mock<ILogger<AuthService>>();
        var mapperMock = new Mock<IMapper>();

        userRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);

        var authService = new AuthService(
            jwtMock.Object,
            userRepoMock.Object,
            userRoleRepoMock.Object,
            passwordMock.Object,
            loggerMock.Object,
            mapperMock.Object
        );

        var dto = new LoginUserDTO { email = "notfound@test.com" };

        // Act
        var result = await authService.LoginUser(dto, "127.0.0.1", "agent");

        // Assert
        result.IsCompleted.Should().BeFalse();
        result.ErrorCode.Should().Be(errorCode.UserNotFound);
    }



}
