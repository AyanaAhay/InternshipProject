namespace StudentApi.Contracts.DTOs;

// Связка студент ↔ руководитель (расширенный ответ от руководителя)
public class StudentSupervisorLinkDetailDto
{
    public int IdSupervisorApplication { get; set; }
    public int IdStudentApplication { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public int AvailableSlotsCount { get; set; }
    public InterviewInfoDto? Interview { get; set; }
    public SupervisorInfoDto? Supervisor { get; set; } // НОВОЕ

    // Добавляется на стороне студента
    public string StatusRu { get; set; } = string.Empty;
}

// Старый DTO для обратной совместимости
public class StudentSupervisorLinkDto
{
    public int IdSupervisorApplication { get; set; }
    public int IdStudentApplication { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StatusName { get; set; }
}

// Информация о собеседовании внутри связки
public class InterviewInfoDto
{
    public int IdInterviewSlot { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingPlace { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Result { get; set; }
    public string? Comment { get; set; }
}

// Доступный слот для собеседования
public class AvailableInterviewSlotDto
{
    public int IdInterviewSlot { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingPlace { get; set; }
}

// Запрос на бронирование слота
public class BookSlotRequestDto
{
    public int IdStudentApplication { get; set; }
    public int? IdSupervisorApplication { get; set; } // НОВОЕ
}

// Ответ после бронирования
public class BookSlotResponseDto
{
    public int IdInterviewSlot { get; set; }
    public string Message { get; set; } = string.Empty;
}

// Слот для отображения студенту
public class InterviewSlotForStudentDto
{
    public int IdInterviewSlot { get; set; }
    public int IdSupervisorApplication { get; set; }
    public int SupervisorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingPlace { get; set; }
}

// Заявка руководителя (для получения supervisorId)
public class SupervisorApplicationDto
{
    public int IdSupervisorApplication { get; set; }
    public int IdEmployee { get; set; }
}

// Забронированный слот
public class BookedInterviewSlotDto
{
    public int IdInterviewSlot { get; set; }
    public int IdStudentApplication { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingPlace { get; set; }
    public string Status { get; set; } = string.Empty;
}

// Свободный слот менеджера
public class ManagerSlotDto
{
    public int IdManagerSlot { get; set; }
    public int IdEmployee { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingPlace { get; set; }
    public string Status { get; set; } = string.Empty;
}

// Запрос на запись к менеджеру
public class SignUpForManagerInterviewDto
{
    public int IdSlot { get; set; }
    public int IdStudent { get; set; }
    public int IdStudentApplication { get; set; }
}

// Собеседование студента (от руководителя)
public class StudentInterviewResponseDto
{
    public int IdInterviewSlot { get; set; }
    public int IdStudentApplication { get; set; }
    public int? IdSupervisorApplication { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingPlace { get; set; }
    public string InterviewType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusRu { get; set; } = string.Empty;
    public bool Result { get; set; }
    public string? Comment { get; set; }
}

// Запрос на выбор руководителя
public class ChooseSupervisorDto
{
    public int IdSupervisorApplication { get; set; }
    public int IdStudentApplication { get; set; }
}

// Информация о руководителе внутри связки
public class SupervisorInfoDto
{
    public int IdEmployee { get; set; }
    public string? FullName { get; set; }
    public string? Specialization { get; set; }
    public string? Department { get; set; }
    public string? Address { get; set; }
}


// НОВОЕ (добавление собеседования с менеджером) - 26.04.2026

// Информация о департаменте
public class DepartmentDto
{
    public int IdDepartment { get; set; }
    public string Name { get; set; } = string.Empty;
}

// Информация о менеджере
public class ManagerInfoDto
{
    public int IdEmployee { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public string? PersonnelNumber { get; set; }
    public string? Position { get; set; }
    public string? Role { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DepartmentDto? Department { get; set; }

    // Вспомогательное свойство
    public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
}

// Детальный слот менеджера
public class ManagerSlotDetailDto
{
    public int IdManagerSlot { get; set; }
    public ManagerInfoDto? Manager { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MeetingPlace { get; set; }
}

// Интервью с менеджером
public class ManagerInterviewResponseDto
{
    public ManagerSlotDetailDto? Slot { get; set; }
    public int IdStudent { get; set; }
    public int? IdStudentApplication { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusRu { get; set; } = string.Empty;
    public bool Result { get; set; }
    public string? Comment { get; set; }
}

// Запрос на создание интервью с менеджером
public class CreateManagerInterviewDto
{
    public int IdSlot { get; set; }
    public int IdStudent { get; set; }
    public int? IdStudentApplication { get; set; }
}