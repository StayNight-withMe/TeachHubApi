using Application.Abstractions.Utils;
using Application.Mapping.MapperDTO;
using Application.Services.AuthService;
using AutoMapper;
using Core.Common.EnumS;
using Core.Common.Types.HashId;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Auth.input;
using Core.Models.TargetDTO.Users.input;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;


namespace TeachHub.UnitTests.Application.Auth;

public class AuthServiceIntegrationTests
{
    private readonly IMapper _mapper;

    public AuthServiceIntegrationTests()
    {
        // Настраиваем минимальный AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserAuthMappingSource, UserAuthDto>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task LoginUser_ShouldReturnPasswordDontMatch_WhenPasswordIsIncorrect()
    {
        // Arrange
        var context = TestDbContextFactory.Create();
        try
        {
            var user = new UserEntity { id = 1, email = "test@test.com", password = "hashed_password", name = "Test" };
            context.users.Add(user);
            await context.SaveChangesAsync();

            var authService = new AuthService(
                Mock.Of<IJwtService>(),
                new EfCoreUserRepository(context),
                new EfCoreUserRoleRepository(context),
                Mock.Of<IPasswordHashService>(), 
                Mock.Of<ILogger<AuthService>>(),
                _mapper
            );

            // Мокаю VerifyPassword, чтобы изолировать логику БД
            var passwordMock = new Mock<IPasswordHashService>();
            passwordMock.Setup(p => p.VerifyPassword("wrong", "hashed_password")).Returns(false);
            // Но AuthService уже создан... → лучше переделать: внедрить мок

            // Пересоздаём с моком
            authService = new AuthService(
                Mock.Of<IJwtService>(),
                new EfCoreUserRepository(context),
                new EfCoreUserRoleRepository(context),
                passwordMock.Object,
                Mock.Of<ILogger<AuthService>>(),
                _mapper
            );

            var dto = new LoginUserDTO { email = "test@test.com", password = "wrong", role = AllRole.user };

            // Act
            var result = await authService.LoginUser(dto, "127.0.0.1", "agent");

            // Assert
            result.ErrorCode.Should().Be(errorCode.PasswordDontMatch);
            result.IsCompleted.Should().BeFalse();
        }
        finally
        {
            TestDbContextFactory.Destroy(context);
        }
    }

    [Fact]
    public async Task LoginUser_ShouldReturnSuccess_WhenEverythingIsCorrect()
    {
        // Arrange
        var context = TestDbContextFactory.Create();
        try
        {
            // Подготовка данных
            var user = new UserEntity { id = 55, email = "ok@test.com", password = "hashed", name = "TestUser" };
            var userRole = new UserRoleEntity { userid = 55, roleid = (int)AllRole.user };

            context.users.Add(user);
            context.usersroles.Add(userRole);
            await context.SaveChangesAsync();

            // Мокаю пароль (чтобы не зависеть от Argon2)
            var passwordMock = new Mock<IPasswordHashService>();
            passwordMock.Setup(p => p.VerifyPassword("123", "hashed")).Returns(true);

            var authService = new AuthService(
                Mock.Of<IJwtService>(),
                new EfCoreUserRepository(context),
                new EfCoreUserRoleRepository(context),
                passwordMock.Object,
                Mock.Of<ILogger<AuthService>>(),
                _mapper
            );

            var dto = new LoginUserDTO { email = "ok@test.com", password = "123", role = AllRole.user };
            var expectedDto = new UserAuthDto
            {
                id = new Hashid(55),
                email = "ok@test.com",
                name = "TestUser",
                role = Enum.GetName(AllRole.user)!,
                ip = "1.1.1.1",
                useragent = "agent"
            };

            // Act
            var result = await authService.LoginUser(dto, "1.1.1.1", "agent");

            // Assert
            result.IsCompleted.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto, opt => opt.Excluding(x => x.id)); // Hashid может не совпасть по ссылке

            // Проверим ID отдельно
            result.Value.id.Value.Should().Be(55);
        }
        finally
        {
            TestDbContextFactory.Destroy(context);
        }
    }
}