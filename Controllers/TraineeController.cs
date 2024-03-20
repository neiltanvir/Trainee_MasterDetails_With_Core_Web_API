using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TraineeCoreAPI.DTOs;
using TraineeCoreAPI.Models;

namespace TraineeCoreAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TraineeController : ControllerBase
    {
        private readonly TraineeDbContext _db;
        private IWebHostEnvironment _e;

        public TraineeController(TraineeDbContext db, IWebHostEnvironment e)
        {
            _db = db;
            _e = e;
        }

        [HttpGet]
        public IActionResult GetAllTrainee()
        {
            List<Trainee> trainees = _db.Trainees.Include(x => x.Courses).ToList();  //EgerLoading
            string JsonString = JsonConvert.SerializeObject(trainees, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

            });
            return Content(JsonString, "application/Json");
        }

        [HttpGet("{id}")]
        public IActionResult GetTraineeById(int id)
        {

            Trainee trainee = _db.Trainees.Include(x => x.Courses).FirstOrDefault(x => x.TraineeId == id);

            if (trainee == null)
            {
                return NotFound("Empty Data");
            }

            string jsonstring = JsonConvert.SerializeObject(trainee, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            return Content(jsonstring, "application/Json");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            var tr = await _db.Trainees.FindAsync(id);
            if (tr == null)
            {
                return BadRequest("Trainee not found.");
            }
            _db.Trainees.Remove(tr);
            _db.SaveChanges();
            return Ok("Deleted SuccessFully.");
        }

        [HttpPost]
        public async Task<IActionResult> PostTrainees([FromForm] Common common)
        {
            string FN = common.ImageName + ".jpg";
            string Url = "\\Upload\\" + FN;
            if (common.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_e.WebRootPath + "\\Upload\\"))
                {
                    Directory.CreateDirectory(_e.WebRootPath + "\\Upload\\");
                }
                using (FileStream fileStream = System.IO.File.Create(_e.WebRootPath + "\\Upload\\" + common.ImageFile.FileName))
                {
                    common.ImageFile.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }
            Trainee trainee = new Trainee();
            trainee.TraineeName = common.TraineeName;
            trainee.IsRegular = common.IsRegular;
            trainee.BirhDate = common.BirhDate;
            trainee.ImageName = FN;
            trainee.ImageUrl = Url;
            _db.Trainees.Add(trainee);
            await _db.SaveChangesAsync();
            var tr = _db.Trainees.FirstOrDefault(x => x.TraineeName == common.TraineeName);
            int trid = tr.TraineeId;
            List<Course> list = JsonConvert.DeserializeObject<List<Course>>(common.Courses);
            AddExperiences(trid, list);
            await _db.SaveChangesAsync();
            return Ok("Saved SuccessFully.");
        }

        private void AddExperiences(int trid, List<Course>? list)
        {
            {
                foreach (var item in list)
                {
                    Course course = new Course()
                    {
                        TraineeId = trid,
                        Duration = item.Duration,
                        CourseName = item.CourseName,
                    };
                    _db.Courses.Add(course);
                    _db.SaveChanges();
                }
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrainees(int id, [FromForm] Common common)
        {
            var trainee = await _db.Trainees.FindAsync(id);
            if (id != common.TraineeId)
            {
                return BadRequest();
            }
            if (trainee == null)
            {
                return NotFound("Trainee not found.");
            }
            string fileName = common.ImageName + ".jpg";
            string imageUrl = "\\Upload\\" + fileName;
            if (common.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_e.WebRootPath + "\\Upload\\"))
                {
                    Directory.CreateDirectory(_e.WebRootPath + "\\Upload\\");
                }
                using (FileStream fileStream = System.IO.File.Create(_e.WebRootPath + "\\Upload\\" + common.ImageFile.FileName))
                {
                    common.ImageFile.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }

            trainee.TraineeName = common.TraineeName;
            trainee.IsRegular = common.IsRegular;
            trainee.BirhDate = common.BirhDate;
            trainee.ImageName = fileName;
            trainee.ImageUrl = imageUrl;

            var exis = _db.Courses.Where(x => x.TraineeId == id);
            _db.Courses.RemoveRange(exis);

            List<Course> list = JsonConvert.DeserializeObject<List<Course>>(common.Courses);
            AddExperiences(trainee.TraineeId, list);
            await _db.SaveChangesAsync();
            return Ok("Updated SuccessFully");
        }
    }
}
