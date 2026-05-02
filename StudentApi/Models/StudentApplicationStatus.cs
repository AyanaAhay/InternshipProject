namespace StudentApi.Models; 
/// <summary> 
/// Статусы заявки студента. 
/// </summary>
public enum StudentApplicationStatus 
{ 
    Draft, // Черновик — студент создал, но не отправил
    UnderManagerReview, // На рассмотрении менеджером
    Testing, // Менеджер назначил тестирование
    InterviewWithManager, // Собеседование с менеджером
    UnderSupervisorReview, // На рассмотрении руководителем
    InterviewWithSupervisor, // Собеседование с руководителем
    DocumentsSigning, // Оформление документов
    Accepted, // Принят на практику
    Rejected, // Отказано
    CancelledByStudent // Студент отозвал заявку
 }