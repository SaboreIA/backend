using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SaboreIA.Authorization.IsOwnerOrAdmin
{
    /// <summary>
    /// Handler de autorização que verifica se o usuário é o proprietário do recurso ou um administrador.
    /// Funciona para validar acesso a recursos por userId (usuário) ou ownerId (restaurante).
    /// </summary>
    public class IsOwnerOrAdminHandler : AuthorizationHandler<IsOwnerOrAdminRequirement, long>
    {
        /// <summary>
        /// Valida se o usuário atual é o proprietário do recurso (resource ID) ou um ADMIN
        /// </summary>
        /// <param name="context">Contexto de autorização com informações do usuário</param>
        /// <param name="requirement">Requisito de autorização</param>
        /// <param name="resourceOwnerId">ID do proprietário do recurso (userId ou ownerId)</param>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsOwnerOrAdminRequirement requirement,
            long resourceOwnerId)
        {
            // Extrai o ID do usuário atual dos claims do token
            var userIdStr = context.User.FindFirst("userId")?.Value;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            // Valida se conseguiu extrair o userId
            if (!long.TryParse(userIdStr, out var currentUserId))
            {
                return Task.CompletedTask; // Falha na autorização
            }

            // Permite acesso se:
            // 1. O usuário é ADMIN (acesso total)
            // 2. O userId atual é igual ao resourceOwnerId (é o dono do recurso)
            if (role?.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) == true ||
                currentUserId == resourceOwnerId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
