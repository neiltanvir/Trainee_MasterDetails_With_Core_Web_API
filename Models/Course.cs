using System;
using System.Collections.Generic;

namespace TraineeCoreAPI.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public int Duration { get; set; }

    public int? TraineeId { get; set; }

    public virtual Trainee? Trainee { get; set; }
}
