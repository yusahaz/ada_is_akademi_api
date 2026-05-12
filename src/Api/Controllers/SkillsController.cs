namespace Azoxia.AdaIsAkademi.Api.Controllers
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Api.Controllers;
    using Azoxia.Core.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.Collections.Generic;

    /// <summary>
    /// Skill dictionary endpoints used by autocomplete and search flows.
    /// </summary>
    [Tags("Skills")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public sealed class SkillsController(IServiceProvider serviceProvider) :
        ApiControllerBase(serviceProvider)
    {
        #region Methods

        /// <summary>Returns normalized global skill dictionary entries.</summary>
        [HttpPost]
        [Consumes("application/json")]
        [EndpointSummary("List global skills")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<string>>), StatusCodes.Status200OK)]
        public Task<IActionResult> List(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ListGlobalSkillsQuery? query,
            CancellationToken cancellationToken)
            => ExecuteQuery(query ?? new ListGlobalSkillsQuery(), cancellationToken);

        #endregion Methods
    }
}
