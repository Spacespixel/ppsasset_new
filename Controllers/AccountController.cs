using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PPSAsset.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string provider = GoogleDefaults.AuthenticationScheme, string returnUrl = "/", bool isPopup = false)
        {
            var scheme = ResolveScheme(provider);
            if (scheme == null)
            {
                return BadRequest("Unsupported authentication provider.");
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var currentProvider = string.IsNullOrWhiteSpace(provider) ? DetectProvider(User) ?? provider : provider;
                return isPopup
                    ? ClosePopup(returnUrl, currentProvider)
                    : Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(LoginCallback), values: new { returnUrl, isPopup, provider }) ?? returnUrl
            };

            properties.Items["auth-provider"] = scheme;

            return Challenge(properties, scheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginCallback(string returnUrl = "/", bool isPopup = false, string provider = GoogleDefaults.AuthenticationScheme)
        {
            var resolvedProvider = string.IsNullOrWhiteSpace(provider) ? DetectProvider(User) ?? provider : provider;
            // Google handler already signed the user in via the cookie scheme.
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return isPopup
                    ? ClosePopup(Url.Action("Project", "Home") ?? "/", resolvedProvider)
                    : RedirectToAction("Project", "Home");
            }

            return isPopup ? ClosePopup(returnUrl, resolvedProvider) : Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Project", "Home")
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private ContentResult ClosePopup(string returnUrl, string? provider)
        {
            var fallbackUrl = Url.Action("Project", "Home") ?? "/";
            var safeUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : fallbackUrl;
            var safeUrlWithHash = safeUrl.Contains("#register", StringComparison.OrdinalIgnoreCase)
                ? safeUrl
                : safeUrl + "#register";
            var encodedUrl = JavaScriptEncoder.Default.Encode(safeUrl);
            var encodedUrlWithHash = JavaScriptEncoder.Default.Encode(safeUrlWithHash);
            var providerLabel = provider ?? DetectProvider(User);
            var encodedProvider = JavaScriptEncoder.Default.Encode(providerLabel ?? string.Empty);

            var script = $"<html><body><script>" +
                         "if (window.opener && !window.opener.closed) {" +
                         $"if (typeof window.opener.handleSocialLoginSuccess === 'function') {{ window.opener.handleSocialLoginSuccess('{encodedProvider}','{encodedUrl}'); }} else {{ window.opener.location='{encodedUrlWithHash}'; }}" +
                         "window.close();" +
                         "} else {" +
                         $"window.location='{encodedUrlWithHash}';" +
                         "}" +
                         "</script></body></html>";

            return Content(script, "text/html");
        }

        private static string? DetectProvider(ClaimsPrincipal? user)
        {
            if (user?.Claims == null)
            {
                return null;
            }

            if (user.Claims.Any(c => c.Type.StartsWith("urn:facebook", StringComparison.OrdinalIgnoreCase)))
            {
                return FacebookDefaults.AuthenticationScheme;
            }

            if (user.Claims.Any(c => c.Type.StartsWith("urn:google", StringComparison.OrdinalIgnoreCase)))
            {
                return GoogleDefaults.AuthenticationScheme;
            }

            return null;
        }

        private static string? ResolveScheme(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return GoogleDefaults.AuthenticationScheme;
            }

            return provider.ToLowerInvariant() switch
            {
                "google" => GoogleDefaults.AuthenticationScheme,
                "facebook" => FacebookDefaults.AuthenticationScheme,
                var scheme when string.Equals(scheme, GoogleDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) => GoogleDefaults.AuthenticationScheme,
                var scheme when string.Equals(scheme, FacebookDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) => FacebookDefaults.AuthenticationScheme,
                _ => null
            };
        }
    }
}
