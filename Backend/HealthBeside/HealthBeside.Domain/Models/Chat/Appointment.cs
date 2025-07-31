using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Chat;

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled,
    Rescheduled
}
public class Appointment
{
    public Guid Id { get; private set; }
    public DateTime AppointmentTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string ReasonForVisit { get; private set; }

    public Guid PatientProfileId { get; private set; }
    public PatientProfile PatientProfile { get; private set; }
    public Guid DoctorProfileId { get; private set; }
    public DoctorProfile DoctorProfile { get; private set; }
    public Guid DoctorAvailabilityId { get; private set; }
    public DoctorAvailability DoctorAvailability { get; private set; }
    
    public Consultation Consultation { get; private set; }
    
    private Appointment() { }

    public static (string? Error, Appointment? Appointment) Create(
        Guid patientProfileId,
        Guid doctorProfileId,
        Guid doctorAvilabilityId,
        DateTime appointmentTime,
        string reasonForVisit)
    {
        var errors = new List<string>();
        
        if(patientProfileId == Guid.Empty)
            errors.Add("Patient profile ID cannot be empty.");
        
        if(doctorProfileId == Guid.Empty) 
            errors.Add("Doctor profile ID cannot be empty.");
        
        if(doctorAvilabilityId == Guid.Empty)
            errors.Add("Doctor availability ID cannot be empty.");
        
        if(appointmentTime <= DateTime.UtcNow)
            errors.Add("Appointment time must be after current time.");
        
        if(string.IsNullOrWhiteSpace(reasonForVisit))
            errors.Add("Reason for visit cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientProfileId = patientProfileId,
            DoctorProfileId = doctorProfileId,
            DoctorAvailabilityId = doctorAvilabilityId,
            AppointmentTime = appointmentTime,
            Status = AppointmentStatus.Scheduled,
            ReasonForVisit = reasonForVisit
        };
        
        return (null, appointment);
    }
}
