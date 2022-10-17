using DS.Api.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace DS.Api.Base
{
    public abstract class ControllerAbstract : ControllerBase
    {
        protected ResultWithException DoInTry(Action action)
        {
            ResultWithException result = new ResultWithException();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                result.ExceptionData = ex;
            }
            return result;
        }
        protected ResultWithException<T> DoInTry<T>(Func<T> func)
        {
            ResultWithException<T> result = new ResultWithException<T>();
            try
            {
                T data = func();
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.ExceptionData = ex;
            }
            return result;
        }
        protected abstract Action<Exception> ExceptionHandler { get; }
        protected void ResultHandler(Action action)
        {
            var result = DoInTry(action);
            if (!result.Success && ExceptionHandler != null)
                ExceptionHandler(result.ExceptionData);
        }
        protected T ResultHandler<T>(Func<T> func, T defaultValue)
        {
            var result = DoInTry<T>(func);
            if (result.Success)
                return result.Data;
            else if (ExceptionHandler != null)
                ExceptionHandler(result.ExceptionData);
            return defaultValue;

        }
        protected IActionResult RequestHandler(Action action, int statusCode = 200)
        {
            var result = DoInTry(action);
            if (!result.Success && ExceptionHandler != null)
                ExceptionHandler(result.ExceptionData);
            if (result.Success)
                return StatusCode(statusCode, new MessageResponse { Message = "OK" });
            else
                return BadRequest(result.ExceptionData.Message);
        }
        protected IActionResult RequestHandler<T>(Func<T> func, int statusCode = 200)
        {
            var result = DoInTry<T>(func);
            if (!result.Success && ExceptionHandler != null)
                ExceptionHandler(result.ExceptionData);
            if (result.Success)
                return StatusCode(statusCode, new ResultResponse<T> { Value = result.Data });
            else
                return BadRequest(result.ExceptionData.Message);

        }
    }
}
