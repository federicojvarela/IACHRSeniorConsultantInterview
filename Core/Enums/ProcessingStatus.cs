
namespace Core.Enums
{
    /// <summary>
    /// Representa el estado de procesamiento de un documento
    /// </summary>
    public enum ProcessingStatus
    {
        /// <summary>
        /// El documento está pendiente de procesamiento
        /// </summary>
        Pending,

        /// <summary>
        /// El documento está siendo procesado actualmente
        /// </summary>
        Processing,

        /// <summary>
        /// El procesamiento del documento se ha completado exitosamente
        /// </summary>
        Completed,

        /// <summary>
        /// El procesamiento del documento ha fallado
        /// </summary>
        Failed
    }
}
