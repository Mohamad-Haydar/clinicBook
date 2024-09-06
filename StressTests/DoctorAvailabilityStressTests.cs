using api.BusinessLogic.DataAccess;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Data;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NBench;

namespace StressTests
{
    public class DoctorAvailabilityStressTests
    {
        Counter testCounter;
        DoctorAvailabilityController doctorAvailability;
        public IDoctorAvailabilityData _doctorAvailabilityData;
        private readonly UserManager<UserModel> _userManager;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly ApplicationDbContext _appDbContext;
        private readonly Dictionary<string, string> Days;
        private readonly ISqlDataAccess _sql;

        public DoctorAvailabilityStressTests( 
            BenchmarkContext context,
            UserManager<UserModel> userManager,
            ApplicationDbContext appDbContext,
            ISqlDataAccess sql,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            Days = new()
            {
                 { "Monday", "الاثنين" },
                 { "Tuesday", "الثلاثاء" },
                 { "Wednesday", "الاربعاء" },
                 { "Thursday", "الخميس" },
                 { "Friday", "الجمعة" },
                 { "Saturday", "السبت" },
                 { "Sunday", "الاحد" }
            };
            _sql = sql;
            _connectionStrings = connectionStrings;
            testCounter = context.GetCounter("doctoravailabilityCounter");
        }

        
        public void SetUp(BenchmarkContext context) 
        {
            _doctorAvailabilityData = new DoctorAvailabilityData(_userManager, _appDbContext, _sql, _connectionStrings);
            doctorAvailability = new(_doctorAvailabilityData);
        }

        [PerfBenchmark(NumberOfIterations = 5, RunMode = RunMode.Throughput, RunTimeMilliseconds = 2000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("doctoravailabilityCounter", MustBe.GreaterThan, 11000000)]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo, ByteConstants.SixtyFourKb)]
        public async Task GetAvailableDatesAsync_Test()
        {
            string testDoctorId = "12345"; // Replace with actual test doctor ID
            await doctorAvailability.GetAvailableDates(testDoctorId);
            testCounter.Increment();
        }

        [PerfCleanup]
        public void CleanUp()
        {
            // Dispose
        }


    }
}