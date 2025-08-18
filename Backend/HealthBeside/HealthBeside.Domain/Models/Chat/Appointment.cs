using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Chat;

public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    Rescheduled = 4
}

public class Appointment
{
    public Guid Id { get; private set; }
    public DateTime StartUtc { get; private set; }
    public int DurationInMinutes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string ReasonForVisit { get; private set; }

    public Guid PatientProfileId { get; private set; }
    public PatientProfile PatientProfile { get; private set; }
    public Guid DoctorProfileId { get; private set; }
    public DoctorProfile DoctorProfile { get; private set; }
    public Guid DoctorAvailabilityId { get; private set; }
    public DoctorAvailability DoctorAvailability { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private Appointment() { }

    public static (string? Error, Appointment? Appointment) Create(
        Guid patientProfileId,
        Guid doctorProfileId,
        Guid doctorAvailabilityId,
        DateTime startUtc,
        string reasonForVisit,
        int durationInMinutes = 30)
    {
        var errors = new List<string>();

        if (patientProfileId == Guid.Empty)
            errors.Add("Patient profile ID cannot be empty.");

        if (doctorProfileId == Guid.Empty)
            errors.Add("Doctor profile ID cannot be empty.");

        if (doctorAvailabilityId == Guid.Empty)
            errors.Add("Doctor availability ID cannot be empty.");

        var todayUtc = DateTime.UtcNow.Date;
        if (startUtc.Date <= todayUtc)
            errors.Add("Appointment must be at least 1 day in advance.");
        if (startUtc.Date > todayUtc.AddDays(30))
            errors.Add("Appointment cannot be booked more than 30 days in advance.");

        if (string.IsNullOrWhiteSpace(reasonForVisit))
            errors.Add("Reason for visit cannot be empty.");

        if (durationInMinutes <= 0)
            errors.Add("Duration must be positive.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientProfileId = patientProfileId,
            DoctorProfileId = doctorProfileId,
            DoctorAvailabilityId = doctorAvailabilityId,
            StartUtc = startUtc,
            DurationInMinutes = durationInMinutes,
            Status = AppointmentStatus.Scheduled,
            ReasonForVisit = reasonForVisit
        };

        return (null, appointment);
    }

    public string Update(DateTime newStartUtc, string newReason, int? newDuration = null)
    {
        var todayUtc = DateTime.UtcNow.Date;
        var errors = new List<string>();

        if (newStartUtc.Date <= todayUtc)
            errors.Add("Appointment must be at least 1 day in advance.");
        if (newStartUtc.Date > todayUtc.AddDays(30))
            errors.Add("Appointment cannot be booked more than 30 days in advance.");

        if (string.IsNullOrWhiteSpace(newReason))
            errors.Add("Reason for visit cannot be empty.");

        if (newDuration.HasValue || newDuration <= 0)
            errors.Add("Duration must be positive.");

        if (errors.Any())
            return string.Join("; ", errors);

        StartUtc = newStartUtc;
        ReasonForVisit = newReason;
        
        if (newDuration.HasValue)
            DurationInMinutes = newDuration.Value;
        
        Status = AppointmentStatus.Rescheduled;

        return null;
    }
}
