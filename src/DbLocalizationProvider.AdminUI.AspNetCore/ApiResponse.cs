// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.AdminUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

public class ApiResponse : IActionResult
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings =
        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

    private readonly BaseApiModel _response;

    public ApiResponse(BaseApiModel response)
    {
        _response = response;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        return context.HttpContext.Response.WriteAsync(
            JsonConvert.SerializeObject(
                _response,
                _jsonSerializerSettings));
    }
}
