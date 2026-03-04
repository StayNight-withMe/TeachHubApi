using Microsoft.Extensions.Logging;

namespace Logger
{
    public static class LoggerExtensions
    {
        public static void LogLoginUserNotFound(this ILogger logger, int id, string email)
            => logger.LogInformation(
                "Пользователь не найден при попытке входа. UserId: {UserId}, Email: {Email}",
                id, email);

        public static void LogRemoveUserNotFound(this ILogger logger, int id)
            => logger.LogInformation(
                "Попытка удаления несуществующего пользователя. UserId: {UserId}",
                id);

        public static void LogProfileUpdateUserNotFound(this ILogger logger, int id)
            => logger.LogInformation(
                "Не удалось обновить профиль: пользователь не найден. UserId: {UserId}",
                id);

        public static void LogAffectedRows(this ILogger logger, int count)
            => logger.LogInformation(
                "Операция завершена. Затронуто строк: {Count}",
                count);

        public static void LogCriticalError(this ILogger logger, Exception exception)
            => logger.LogCritical(
            "Критическая ошибка : {ex.Message}",
            exception.Message);

        public static void LogError(this ILogger logger, Exception exception)
            => logger.LogError(
            "Ошибка : {ex.Message}",
            exception.Message);

        public static void LogDBError(this ILogger logger, Exception exception)
            => logger.LogError(
            "Ошибка : {ex.Message}",
            exception.Message);

    }
}



