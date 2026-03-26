namespace Entities
{
    /// <summary>
    /// Constantes partagées entre les deux applications (Admin et Vendeur).
    /// Centralise les valeurs magiques pour éviter les duplications et faciliter
    /// la maintenance (noms de rôles, paramètres de pagination).
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Constantes relatives aux rôles Identity de l'application.
        /// </summary>
        public static class Roles
        {
            /// <summary>Rôle accordant l'accès complet à l'administration.</summary>
            public const string Administrateur = "Administrateur";

            /// <summary>Rôle accordant l'accès à l'espace vendeur.</summary>
            public const string Vendeur = "Vendeur";
        }

        /// <summary>
        /// Paramètres de pagination des listes.
        /// </summary>
        public static class Pagination
        {
            /// <summary>Nombre d'éléments affichés par page dans les listes paginées.</summary>
            public const int DefaultPageSize = 10;
        }
    }
}
