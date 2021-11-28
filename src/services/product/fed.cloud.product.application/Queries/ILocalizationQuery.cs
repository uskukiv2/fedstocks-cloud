using fed.cloud.product.domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Queries
{
    public interface ILocalizationQuery
    {
        Task<string> GetLocalizationForAsync(string target, LocalizationTargetType targetType, string targetLocalization);
    }
}
