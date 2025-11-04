using PPSAsset.Models;

namespace PPSAsset.Services
{
    /// <summary>
    /// Service interface for managing Google Tag Manager configurations
    /// </summary>
    public interface IGtmService
    {
        /// <summary>
        /// Get GTM ID for a specific project based on current environment
        /// </summary>
        /// <param name="projectId">Project identifier</param>
        /// <returns>GTM ID or null if not configured</returns>
        Task<string?> GetGtmIdAsync(string projectId);

        /// <summary>
        /// Get GTM ID for global site configuration based on current environment
        /// </summary>
        /// <returns>Global GTM ID or null if not configured</returns>
        Task<string?> GetGlobalGtmIdAsync();

        /// <summary>
        /// Update GTM configuration for a project
        /// </summary>
        /// <param name="projectId">Project identifier</param>
        /// <param name="gtmId">GTM ID for the project</param>
        /// <returns>True if update successful</returns>
        Task<bool> UpdateProjectGtmAsync(string projectId, string? gtmId);
    }
}