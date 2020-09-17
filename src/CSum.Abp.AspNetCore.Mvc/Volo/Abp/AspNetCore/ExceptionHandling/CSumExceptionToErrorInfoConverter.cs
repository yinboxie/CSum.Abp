using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;

namespace Volo.Abp.AspNetCore.ExceptionHandling
{
    /// <summary>
    /// 异常错误信息转换
    /// </summary>
    public class CSumExceptionToErrorInfoConverter : DefaultExceptionToErrorInfoConverter
    {
        public CSumExceptionToErrorInfoConverter(
            IOptions<AbpExceptionLocalizationOptions> localizationOptions,
            IOptions<AbpExceptionHandlingOptions> exceptionHandlingOptions,
            IStringLocalizerFactory stringLocalizerFactory,
            IStringLocalizer<AbpUiResource> abpUiStringLocalizer,
            IServiceProvider serviceProvider)
            :base(localizationOptions, exceptionHandlingOptions,stringLocalizerFactory, abpUiStringLocalizer, serviceProvider)
        { 
        }

        protected override RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception)
        {
            if (ExceptionHandlingOptions.SendExceptionsDetailsToClients)
            {
                return CreateDetailedErrorInfoFromException(exception);
            }

            exception = TryToGetActualException(exception);

            if (exception is EntityNotFoundException)
            {
                return CreateEntityNotFoundError(exception as EntityNotFoundException);
            }

            if (exception is AbpAuthorizationException)
            {
                var authorizationException = exception as AbpAuthorizationException;
                return new RemoteServiceErrorInfo(authorizationException.Message);
            }

            var errorInfo = new RemoteServiceErrorInfo();

            if (exception is IUserFriendlyException)
            {
                errorInfo.Message = exception.Message;
                errorInfo.Details = (exception as IHasErrorDetails)?.Details;
            }

            //数据库并发
            if (exception is AbpDbConcurrencyException)
            {
                errorInfo.Message = "并发冲突，请刷新重新提交数据";
            }

            //数据库其他异常
            if (IsSqlException(exception))
            {
                errorInfo.Message = FormatSqlExceptionMessage(exception);
            }

            if (exception is IHasValidationErrors)
            {
                if (errorInfo.Message.IsNullOrEmpty())
                {
                    errorInfo.Message = L["ValidationErrorMessage"];
                }

                if (errorInfo.Details.IsNullOrEmpty())
                {
                    errorInfo.Details = GetValidationErrorNarrative(exception as IHasValidationErrors);
                }

                errorInfo.ValidationErrors = GetValidationErrorInfos(exception as IHasValidationErrors);
            }

            TryToLocalizeExceptionMessage(exception, errorInfo);

            if (errorInfo.Message.IsNullOrEmpty())
            {
                errorInfo.Message = L["InternalServerErrorMessage"];
            }

            return errorInfo;
        }

        protected override void TryToLocalizeExceptionMessage(Exception exception, RemoteServiceErrorInfo errorInfo)
        {
            base.TryToLocalizeExceptionMessage(exception, errorInfo);
            // 500错误，显示错误简要信息
            if (errorInfo.Message.IsNullOrEmpty())
            {
                errorInfo.Message = exception.Message;
            }
        }

        protected override RemoteServiceErrorInfo CreateEntityNotFoundError(EntityNotFoundException exception)
        {
            if (exception.EntityType != null)
            {
                var name = exception.EntityType.GetDescription();
                return new RemoteServiceErrorInfo(
                    $"未找到{name}实体({exception.Id})"
                );
            }

            return new RemoteServiceErrorInfo(exception.Message);
        }

        protected override Exception TryToGetActualException(Exception exception)
        {
            if (exception is AggregateException && exception.InnerException != null)
            {
                var aggException = exception as AggregateException;


                if (aggException.InnerException is IUserFriendlyException ||
                    aggException.InnerException is AbpValidationException ||
                    aggException.InnerException is EntityNotFoundException ||
                    aggException.InnerException is AbpAuthorizationException ||
                    aggException.InnerException is IBusinessException)
                {
                    return aggException.InnerException;
                }
            }
            else if (IsSqlException(exception.InnerException))
            {
                //数据库客户端异常
                return exception.InnerException;
            }

            return exception;
        }

        protected virtual bool IsSqlException(Exception exception)
        {
            if (exception != null)
            {
                return exception.GetType().Name == "SqlException";
            }
            return false;
        }

        protected virtual string FormatSqlExceptionMessage(Exception exception)
        {
            var strMessage = "";
            var arrError = exception.Message.Split('。');
            if (exception.Message.Contains("DELETE"))
            {
                strMessage = $"该数据已被引用，不允许删除({arrError[0]})";
            }
            else if (exception.Message.Contains("Could not open a connection"))
            {
                strMessage = "网络请求异常，不能连接至数据库";
            }
            return strMessage;
        }
    }
}
