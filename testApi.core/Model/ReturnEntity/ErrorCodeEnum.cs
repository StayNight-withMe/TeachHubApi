namespace Core.Model.ReturnEntity
{
    public enum errorCode
    {
        None = 0,               //  то шо усё Чотка если надо будет

        // Ошибки валидации (400 Bad Request)
        InvalidDataFormat = 1,  
        PasswordTooShort = 2,
        EmailInvalid = 3,
        PasswordTooLong = 4,
        UserNotFound = 5,
        CoursesNotFoud = 6,
        ChapterNotFound = 7,
        IpNotFound = 8,

        // Бизнес-конфликты (409 Conflict)
        UserAlreadyExists = 101, 
        CourseTitleAlreadyExists = 102,
        InvalidPagination = 103,
        EmailAlreadyExists = 104,
        PasswordIsEasily = 105,
        PasswordDontMatch = 107,

        // Системные ошибки (500 Internal Server Error)
        DatabaseError = 201,
        UnknownError = 299
    }
}
