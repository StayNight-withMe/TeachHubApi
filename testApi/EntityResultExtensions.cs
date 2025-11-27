using Microsoft.AspNetCore.Mvc;
using Core.Model.ReturnEntity;

namespace testApi
{
    public static class EntityResultExtensions
    {


        public static int ErrorMap(errorCode num)
        {
            if ((int)num < 100)
            {
                return 400;
            }
            else if ((int)num < 200)
            {
                return 409;
            }
            else
            {
                return 500;
            }

        }



        private static IActionResult MapToAction(EntityOfTResult result, ControllerBase controllerBase)
        {

            return result.ErrorCode switch
            {
                // 400 Bad Request
                errorCode.InvalidDataFormat or errorCode.PasswordTooShort or errorCode.EmailInvalid
                    => controllerBase.BadRequest(new { code = result.ErrorCode.ToString(), message = "некорректные данные для регистрации" }),

                // 409 Conflict
                errorCode.UserAlreadyExists
                    => controllerBase.Conflict(new { code = result.ErrorCode.ToString(), message = "пользователь уже существует" }),

                // 500 Internal Server Error 
                _ => controllerBase.StatusCode(ErrorMap(result.ErrorCode), new { code = result.ErrorCode.ToString(), message = result.MessageForUser })

            };

        }

        public static IActionResult ToActionResult<T>(TResult<T> result, ControllerBase controllerBase) 
        {
            if (result.IsCompleted)
            {
                return controllerBase.StatusCode(200, result.Value);
            }

            return MapToAction(result, controllerBase);

         }
        public static IActionResult ToActionResult(TResult result, ControllerBase controllerBase)
        {
            if (result.IsCompleted)
            {
               return controllerBase.StatusCode(200, "Ok");
            }

            return MapToAction(result, controllerBase);

        }



    }


} 


  