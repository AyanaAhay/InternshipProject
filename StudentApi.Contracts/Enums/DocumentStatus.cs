//namespace StudentApi.Models;
namespace StudentApi.Contracts.Enums;

// Статус загрузки документа
public enum UploadStatus {
    NotUploaded, // Не загружен
    Uploaded // Загружен
}

// Статус проверки менеджером
public enum VerificationStatus {
    Pending, // На проверке (загружен, но ещё не проверен)
    Approved, // Одобрено
    Rejected // Не одобрено
}

// Статус подписания договора
public enum ContractStatus {
    NotReceived, // Не получен
    Received, // Получен
    Signed // Подписан
}