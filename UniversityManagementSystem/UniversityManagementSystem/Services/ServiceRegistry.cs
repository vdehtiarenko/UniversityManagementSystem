using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.Services
{
    public static class ServiceRegistry
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer("Data Source=localhost;Initial Catalog=University;Persist Security Info=True;User ID=User1;Password=sa;Trust Server Certificate=True"));


            services.AddSingleton<ICourseService, CourseService>();
            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IStudentService, StudentService>();
            services.AddSingleton<ITeacherService, TeacherService>();
        }
    }
}
