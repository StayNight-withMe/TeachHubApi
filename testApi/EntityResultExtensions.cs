using Microsoft.AspNetCore.Mvc;
using Core.Model.ReturnEntity;

namespace testApi
{
    public static class EntityResultExtensions
    {

        public static IActionResult ToActionResult(EntityOfTResult result, ControllerBase controllerBase)
        {

            Func<errorCode, int> map = (errorCode) =>
            {
                if((int)errorCode < 100)
                {
                    return 400;
                }
                else if((int)errorCode < 200)
                {
                    return 409;
                }
                else
                {
                    return 500;
                }


            };



            if (result.IsCompleted)
            {
                return controllerBase.StatusCode(200, "OK");
            }

            return result.ErrorCode switch
            {
                // 400 Bad Request
                errorCode.InvalidDataFormat or errorCode.PasswordTooShort or errorCode.EmailInvalid
                    => controllerBase.BadRequest(new { code = result.ErrorCode.ToString(), message = "некорректные данные для регистрации" }),

                // 409 Conflict
                errorCode.UserAlreadyExists
                    => controllerBase.Conflict(new { code = result.ErrorCode.ToString(), message = "пользователь уже существует" }),

                // 500 Internal Server Error 
                _ => controllerBase.StatusCode(map(result.ErrorCode), new { code = result.ErrorCode.ToString(), message = result.MessageForUser })

            };
         }



        }


        } 


  