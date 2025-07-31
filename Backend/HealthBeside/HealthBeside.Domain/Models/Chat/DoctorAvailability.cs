using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Chat;
// на подумати
public class DoctorAvailability
{
    
    // Щодо цього класу, я не знаю. Якщо у нас буде логіка така, що користувач просто пише якомусь доктору,
    // а той відповідає тоді, коли йому зручно, то цей клас не потрібен. У випадку, якщо ми хочемо,
    // аби користувач саме запсиувався на прийом до лікаря у певний час, то цей клас потрібен.
    
    public Guid Id { get; private set; }
    public Guid DoctorProfileId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool IsBooked { get; private set; } // if true - doctor is not available for booking
    
    public DoctorProfile DoctorProfile { get; private set; }
    public Appointment Appointment { get; private set; }
    
    private DoctorAvailability() { }

    public static (string? Error, DoctorAvailability? DoctorAvailability) Create(
        Guid doctorProfileId,
        DateTime startTime,
        DateTime endTime)
    {
        var errors = new List<string>();
        
        if (doctorProfileId == Guid.Empty)
        {
           errors.Add("Doctor profile Id cannot be empty.");
        }

        if (startTime >= endTime)
        {
            errors.Add("Start time must be before end time.");
        }

        if (startTime <= DateTime.UtcNow)
        {
            errors.Add("End time must be after start time.");
        }

        if (errors.Any())
        {
            return (string.Join("; ", errors), null);
        }
        
        var doctorAvailability = new DoctorAvailability
        {
            Id = Guid.NewGuid(),
            DoctorProfileId = doctorProfileId,
            StartTime = startTime,
            EndTime = endTime,
            IsBooked = false 
        };
        
        return (null, doctorAvailability);
    }
}