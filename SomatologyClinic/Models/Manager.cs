namespace SomatologyClinic.Models
{
    public class Manager
    {
        private List<Therapist> Therapists { get; set; } = new List<Therapist>();
        private List<Booking> Bookings { get; set; } = new List<Booking>();

        public void AddTherapist(Therapist therapist)
        {
            Therapists.Add(therapist);
        }

        public void EditTherapist(int therapistId, string name, string specialty)
        {
            var therapist = Therapists.FirstOrDefault(t => t.Id == therapistId);
            if (therapist != null)
            {
                therapist.Name = name;
                therapist.Specialty = specialty;
            }
            else
            {
                throw new Exception("Therapist not found");
            }
        }

        public void DeleteTherapist(int therapistId)
        {
            var therapist = Therapists.FirstOrDefault(t => t.Id == therapistId);
            if (therapist != null)
            {
                Therapists.Remove(therapist);
                Bookings.RemoveAll(b => b.TherapistId == therapistId);
            }
            else
            {
                throw new Exception("Therapist not found");
            }
        }

        public void AssignBooking(Booking booking, int therapistId)
        {
            var therapist = Therapists.FirstOrDefault(t => t.Id == therapistId);
            if (therapist != null)
            {
                booking.TherapistId = therapistId;
                Bookings.Add(booking);
            }
            else
            {
                throw new Exception("Therapist not found");
            }
        }

        public List<Therapist> GetTherapistsBySpecialty(string specialty)
        {
            return Therapists.Where(t => t.Specialty == specialty).ToList();
        }
    }

}
