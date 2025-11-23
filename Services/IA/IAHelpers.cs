namespace SaboreIA.Services.IA
{
    /// <summary>
    /// Helper com prompts para interação com a Perplexity AI
    /// Define instruções de sistema para extração de produtos e categorias
    /// </summary>
    public static class IAHelpers
    {
        /// <summary>
        /// Retorna o prompt de sistema para identificar produtos/pratos com base na entrada do usuário.
        /// A IA deve retornar APENAS o nome do prato/comida ou "nulo" se não for comida.
        /// </summary>
        /// <returns>Prompt de sistema para identificação de produtos</returns>
        public static string getProductByUserInput()
        {
            return  "Você é um assistente que consegue identificar o que é pedido com base em uma entrada do usuário." +
                    "Exemplos: 'Quero comer sushi' = 'Sushi'; 'Hoje queria um taco' = 'Taco'; 'Estou com vontade comer uma pizza de queijo' = 'Pizza'" +
                    "Responda SOMENTE com a palavra do prato ou comida, exatamente assim, sem explicações, sem texto extra." +
                    "E caso, você compreenda que a entrada não é uma comida, seu retorno será 'nulo'";
        }

        /// <summary>
        /// Retorna o prompt de sistema para identificar o tipo de estabelecimento que serve determinado produto.
        /// A IA deve retornar APENAS o nome da categoria/tipo de restaurante ou "nulo" se não for comida.
        /// </summary>
        /// <returns>Prompt de sistema para categorização de produtos</returns>
        public static string getTagByProduct()
        {
            return  "Você é um assistente que responde APENAS com o tipo de estabelecimento que serve a comida mencionada. " +
                    "Exemplos: 'Sushi' ou 'Temaki' = 'Japonês'; 'Guacamole' = 'Mexicano'; 'Pizza' = 'Pizzaria'; 'Esfiha' = 'Esfiharia'. " +
                    "Se o produto inserido for um doce, o retorno deve ser 'Doceria'" +
                    "Responda SOMENTE com a palavra do tipo de estabelecimento, exatamente assim, sem explicações, sem texto extra." +
                    "E caso, você compreenda que a entrada não é uma comida, seu retorno será 'nulo'" +
                    "é um produto servido em qual tipo de restaurante?";
        }
    }
}
