namespace StudentApi.Models;

/// <summary>
/// Статус загрузки документа
/// </summary>
public enum UploadStatus
{
    NotUploaded, // Не загружен
    Uploaded // Загружен
}

/// <summary>
/// Статус проверки менеджером
/// </summary>
public enum VerificationStatus
{
    Pending, // На проверке (загружен, но ещё не проверен)
    Approved, // Одобрено
    Rejected // Не одобрено
}

/// <summary>
/// Статус подписания договора
/// </summary>
public enum ContractStatus
{
    NotReceived, // Не получен
    Received, // Получен
    Signed // Подписан
}