﻿using ManageUser.Application.Exceptions;
using ManageUser.Errors;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using System.Net;

namespace ManageUser.MIddlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var result = string.Empty;

                switch (ex)
                {
                    case NoFoundException noFoundException:
                        statusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case FluentValidation.ValidationException validationException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        var errors = validationException.Errors.Select(err => err.ErrorMessage).ToArray();
                        var validationJsons = JsonConvert.SerializeObject(errors);
                        result = JsonConvert.SerializeObject(new CodeErrorException(statusCode, errors, validationJsons));
                        break;

                    case Application.Exceptions.BadRequestException badRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                }

                if (string.IsNullOrEmpty(result))
                {
                    result = JsonConvert.SerializeObject(new CodeErrorException(statusCode, new string[] { ex.Message }, ex.StackTrace));
                }
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(result);
            }
        }

    }
}
