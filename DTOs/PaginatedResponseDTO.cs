namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO genérico para respostas paginadas
    /// </summary>
    /// <typeparam name="T">Tipo dos itens da lista</typeparam>
    public class PaginatedResponseDTO<T>
    {
        /// <summary>
        /// Número da página atual (começa em 1)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Número total de páginas disponíveis
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Quantidade total de itens (todos os registros, não apenas da página atual)
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Indica se existe uma página anterior
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indica se existe uma próxima página
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Lista de itens da página atual
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}