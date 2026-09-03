namespace Domain.Enum
{
    /// <summary>
    /// Estados posibles de una Subasta.
    /// </summary>
    public enum EstadoSubasta
    {
        /// <summary>Subasta programada para iniciarse en el futuro.</summary>
        PROGRAMADA,

        /// <summary>Subasta activa en proceso de pujas.</summary>
        ACTIVA,

        /// <summary>Subasta finalizada con puja ganadora.</summary>
        FINALIZADA,

        /// <summary>Subasta finalizada sin pujas.</summary>
        DESIERTA
    }

}
