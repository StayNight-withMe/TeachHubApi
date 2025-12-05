using Microsoft.AspNetCore.Mvc;
using Core.Model.ReturnEntity;
using System.Threading.Tasks;

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
                errorCode.FollowingError
                    => controllerBase.Conflict(ErrorMap(result.ErrorCode)),

                errorCode.UserAlreadyExists
                     => controllerBase.Conflict(new { code = result.ErrorCode.ToString(), message = "пользователь уже существует" }),

                // 500 Internal Server Error 
                _ => controllerBase.StatusCode(ErrorMap(result.ErrorCode), new { code = result.ErrorCode.ToString(), message = result.MessageForUser })
            };
         }
            

        public static async Task<IActionResult> ToActionResult<T>(TResult<T> result,
            ControllerBase controllerBase,
            Func<Task>? opt = null 
            ) 
        {
            if (result.IsCompleted)
            {
                if (opt != null)
                {
                    await opt();
                }
                return controllerBase.StatusCode(200, result.Value);
            }
            return MapToAction(result, controllerBase);

         }
        public static async Task<IActionResult> ToActionResult(
            TResult result,
            ControllerBase controllerBase,
            Func<Task>? opt = null
            )
        {
            if (result.IsCompleted)
            {
                if(opt != null)
                {
                    await opt();
                }
               return controllerBase.StatusCode(200, "Ok");
            }

            return MapToAction(result, controllerBase);

        }

    }

} 


  