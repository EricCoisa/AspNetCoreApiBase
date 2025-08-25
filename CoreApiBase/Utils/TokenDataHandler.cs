using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using CoreApiBase.Enums;

namespace CoreApiBase.Utils
{
    public static class TokenDataHandler
    {
        /// <summary>
        /// Extrai dados do token baseado no tipo especificado
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <param name="dataType">Tipo de dado a ser extraído</param>
        /// <returns>Valor do claim ou null se não encontrado</returns>
        public static string? GetTokenData(ClaimsPrincipal user, TokenDataType dataType)
        {
            var claimType = GetClaimType(dataType);
            return user.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Extrai o ID do usuário do token
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <returns>ID do usuário ou 0 se não encontrado</returns>
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userIdStr = GetTokenData(user, TokenDataType.UserId) ?? 
                           GetTokenData(user, TokenDataType.NameId) ?? "0";
            return int.TryParse(userIdStr, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Extrai o username do token
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <returns>Username ou string vazia se não encontrado</returns>
        public static string GetUsername(ClaimsPrincipal user)
        {
            return GetTokenData(user, TokenDataType.Username) ?? string.Empty;
        }

        /// <summary>
        /// Extrai o email do token
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <returns>Email ou string vazia se não encontrado</returns>
        public static string GetEmail(ClaimsPrincipal user)
        {
            return GetTokenData(user, TokenDataType.Email) ?? string.Empty;
        }

        /// <summary>
        /// Extrai o SecurityStamp do token
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <returns>SecurityStamp ou string vazia se não encontrado</returns>
        public static string GetSecurityStamp(ClaimsPrincipal user)
        {
            return GetTokenData(user, TokenDataType.SecurityStamp) ?? string.Empty;
        }

        /// <summary>
        /// Verifica se o usuário possui um determinado role
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <param name="role">Role a ser verificado</param>
        /// <returns>True se o usuário possui o role</returns>
        public static bool HasRole(ClaimsPrincipal user, string role)
        {
            return user.IsInRole(role);
        }

        /// <summary>
        /// Verifica se o usuário é administrador
        /// </summary>
        /// <param name="user">ClaimsPrincipal do usuário autenticado</param>
        /// <returns>True se o usuário é administrador</returns>
        public static bool IsAdmin(ClaimsPrincipal user)
        {
            return HasRole(user, "Admin");
        }

        /// <summary>
        /// Obtém o tipo de claim baseado no enum TokenDataType
        /// </summary>
        /// <param name="dataType">Tipo de dado</param>
        /// <returns>String do tipo de claim</returns>
        private static string GetClaimType(TokenDataType dataType)
        {
            var field = dataType.GetType().GetField(dataType.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? dataType.ToString();
        }
    }
}
