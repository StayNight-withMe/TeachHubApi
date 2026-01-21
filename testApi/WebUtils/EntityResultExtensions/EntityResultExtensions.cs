using Microsoft.AspNetCore.Mvc;
using Core.Models.ReturnEntity;

namespace testApi.WebUtils.EntityResultExtensions
{
    public static class EntityResultExtensions
    {


        public static int ErrorMap(errorCode num)
        {
            return (int)num switch
            {
                < 100 => 400,
                < 200 => 409,
                _ => 500,
            };

        }

        private static IActionResult MapToAction(EntityOfTResult result, ControllerBase controllerBase)
        {

            return result.ErrorCode switch
            {
                // 400 Bad Request
                errorCode.InvalidDataFormat or errorCode.PasswordTooShort or errorCode.EmailInvalid
                    => controllerBase.BadRequest(new { code = result.ErrorCode.ToString() }),

                // 409 Conflict
                errorCode.FollowingError
                    => controllerBase.Conflict(new { code = result.ErrorCode.ToString()} ),

                errorCode.UserAlreadyExists
                     => controllerBase.Conflict(new { code = result.ErrorCode.ToString() }),

                errorCode.NotFound
                    => controllerBase.NotFound(),

                errorCode.ChapterNotFound
                  => controllerBase.NotFound(),

                errorCode.CoursesNotFound
                  => controllerBase.NotFound(),

                _ => controllerBase.StatusCode(ErrorMap(result.ErrorCode), new { code = result.ErrorCode.ToString() })
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
               return controllerBase.Ok();
            }

            return MapToAction(result, controllerBase);

        }

    }

} 


  