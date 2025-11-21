using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Requisito de autorização que valida se o usuário é o proprietário do recurso ou um administrador
/// </summary>
public class IsOwnerOrAdminRequirement : IAuthorizationRequirement
{
    // Requisito vazio - a lógica está no Handler
}
