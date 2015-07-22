namespace MyUni.Business
{
    public class Enrollment
    {
        public int Id { get; set; }
        public Grade? Grade { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}