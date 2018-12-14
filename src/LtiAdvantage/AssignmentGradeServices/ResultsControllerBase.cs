﻿using System;
using System.Threading.Tasks;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// Implements the Assignment and Grade Services results endpoint.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.AgsResultReadonly)]
    [Route("context/{contextid}/lineitems/{id}/results", Name = Constants.ServiceEndpoints.AgsResultsService)]
    [Route("context/{contextid}/lineitems/{id}/results.{format}")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class ResultsControllerBase : ControllerBase
    {
        protected readonly ILogger<ResultsControllerBase> Logger;

        protected ResultsControllerBase(ILogger<ResultsControllerBase> logger)
        {
            Logger = logger;
        }
                
        /// <summary>
        /// Get the results for a line item.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The results.</returns>
        protected abstract Task<ActionResult<ResultContainer>> OnGetResultsAsync(GetResultsRequest request);

        /// <summary>
        /// Get the results for a line item.
        /// </summary>
        [HttpGet]
        [Produces(Constants.MediaTypes.ResultContainer)]
        public async Task<ActionResult<ResultContainer>> GetAsync(string contextId, string id, 
            [FromQuery(Name = "user_id")] string userId = null, 
            [FromQuery] int? limit = null)
        {
            try
            {
                Logger.LogDebug($"Entering {nameof(GetAsync)}.");
            
                if (string.IsNullOrWhiteSpace(id))
                {
                    Logger.LogError($"{nameof(id)} is missing.");
                    return BadRequest();
                }

                try
                {
                    var request = new GetResultsRequest(contextId, id, userId, limit);
                    return await OnGetResultsAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing get results request.");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }
            finally
            {
                Logger.LogDebug($"Exiting {nameof(GetAsync)}.");
            }
        }
    }
}

