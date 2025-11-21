using System;

namespace SaboreIA.Helpers
{
    /// <summary>
    /// Helper para gerenciar timestamps no timezone de São Paulo (UTC-3)
    /// </summary>
    public static class DateTimeHelper
    {
        // Timezone de São Paulo (Brasil)
        private static readonly TimeZoneInfo SaoPauloTimeZone = 
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        /// <summary>
        /// Retorna a data/hora atual no timezone de São Paulo (UTC-3)
        /// </summary>
        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, SaoPauloTimeZone);

        /// <summary>
        /// Converte um DateTime UTC para o timezone de São Paulo
        /// </summary>
        public static DateTime ToSaoPauloTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, SaoPauloTimeZone);
        }

        /// <summary>
        /// Converte um DateTime do timezone de São Paulo para UTC
        /// </summary>
        public static DateTime ToUtc(DateTime saoPauloDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(saoPauloDateTime, SaoPauloTimeZone);
        }

        /// <summary>
        /// Formata uma data no padrão brasileiro (dd/MM/yyyy HH:mm:ss)
        /// </summary>
        public static string FormatBrazilian(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
