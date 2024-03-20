using System;
using System.Collections.Generic;

namespace TraineeCoreAPI.Models;

public partial class Trainee
{
    public int TraineeId { get; set; }

    public string TraineeName { get; set; } = null!;

    public bool IsRegular { get; set; }

    public DateTime BirhDate { get; set; }

    public string? ImageName { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
