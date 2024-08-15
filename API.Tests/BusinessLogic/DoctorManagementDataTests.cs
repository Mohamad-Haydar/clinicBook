using api.BusinessLogic.DataAccess;
using api.Data;
using api.Exceptions;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using API.Tests.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Tests.BusinessLogic
{
    public class DoctorManagementDataTests
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings = Substitute.For<IOptions<ConnectionStrings>>();
        private readonly ISqlDataAccess _sql = Substitute.For<ISqlDataAccess>();
        private readonly ApplicationDbContext _appContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly IdentityAppDbContext _identityContext;
        private readonly DbFactory _contextFactory;


        private readonly DoctorManagementData _sut;

        public DoctorManagementDataTests()
        {
            _contextFactory = new DbFactory();

            _appContext = _contextFactory.CreateAppContext();
            _userManager = _contextFactory.CreateUserManager();
            _identityContext = _contextFactory.CreateIdentityContext();

            _sut = new DoctorManagementData(_connectionStrings, _sql, _appContext, _userManager, _identityContext);
        }

        [Fact]
        public async Task AddDoctorServiceAsync_ThrowException()
        {
            // Arrange
            DoctorServiceRequest model = new();
            _sql.SaveDataAsync<DoctorServiceRequest>("storedProcedure", model, "connectionString").Throws(new Exception());

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.AddDoctorServiceAsync(model));

            // Assert
            Assert.IsType<BusinessException>(exception);
            Assert.Equal("Something when wrong please try again", exception.Message);
        }

        [Fact]
        public async Task AddDoctorServiceAsync_AddedSuccessfully()
        {
            // Arrange
            DoctorServiceRequest model = new();
            var myOptions = new ConnectionStrings
            {
                AppDbConnection = "YourConnectionString",
                IdentityDbConnection = "connection string",
            };
            _connectionStrings.Value.Returns(myOptions);
            _sql.SaveDataAsync<DoctorServiceRequest>("storedProcedure", model, _connectionStrings.Value.AppDbConnection).Returns(Task.CompletedTask);

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.AddDoctorServiceAsync(model));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task UpdateDoctorServiceDurationAsync_ServiceNotFound_ThrowUserNotFoundException()
        {
            // Arrange
            int id = 1;
            int duration = 1;
            IEnumerable<DoctorServiceModel> doctorServices =[];
            _appContext.DoctorServices.AddRange(doctorServices);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.UpdateDoctorServiceDurationAsync(id, duration));

            // Assert
            Assert.IsType<UserNotFoundException>(exception);
            Assert.Equal("something whent wrong please check your input data", exception.Message);

        }

        [Fact]
        public async Task UpdateDoctorServiceDurationAsync_ServiceFound_UpdatedSuccessfully()
        {
            // Arrange
            int id = 1;
            int duration = 1;
            IEnumerable<DoctorServiceModel> doctorServices =
              [
                  new()
                    {
                        ServiceName  = "service 1",
                        Duration  = 10,
                        DoctorId  = "id1",
                        ServiceId  = 1,
                    },
                    new()
                    {
                        ServiceName  = "service 2",
                        Duration  = 15,
                        DoctorId  = "id1",
                        ServiceId  = 2,
                    },
                    new()
                    {
                        ServiceName  = "service 3",
                        Duration  = 10,
                        DoctorId  = "id2",
                        ServiceId  = 3,
                    }
              ];
            _appContext.DoctorServices.AddRange(doctorServices);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.UpdateDoctorServiceDurationAsync(id, duration));
            var res = await _appContext.DoctorServices.ToListAsync();
            var updatedService = await _appContext.DoctorServices.FirstOrDefaultAsync(x => x.Id == id);

            // Assert
            Assert.Null(exception);
            Assert.Equal(3, res.Count);
            Assert.NotNull(updatedService);
            Assert.Equal(updatedService.Duration, duration);

        }

        [Fact]
        public async Task DeleteDoctorServiceAsync_ServiceNotFound_ThrowUserNotFoundException()
        {
            // Arrange
            int id = 1;

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.DeleteDoctorServiceAsync(id));

            // Assert
            Assert.IsType<UserNotFoundException>(exception);
            Assert.Equal("Service not found", exception.Message);
        }

        [Fact]
        public async Task DeleteDoctorServiceAsync_ServiceDeleted()
        {
            // Arrange
            int id = 1;
            IEnumerable<DoctorServiceModel> doctorServices =
             [
                 new()
                    {
                        ServiceName  = "service 1",
                        Duration  = 10,
                        DoctorId  = "id1",
                        ServiceId  = 1,
                    },
                    new()
                    {
                        ServiceName  = "service 2",
                        Duration  = 15,
                        DoctorId  = "id1",
                        ServiceId  = 2,
                    },
                    new()
                    {
                        ServiceName  = "service 3",
                        Duration  = 10,
                        DoctorId  = "id2",
                        ServiceId  = 3,
                    }
             ];
            _appContext.DoctorServices.AddRange(doctorServices);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.DeleteDoctorServiceAsync(id));
            var res = await _appContext.DoctorServices.ToListAsync();
            var deletedService = await _appContext.DoctorServices.FirstOrDefaultAsync(x => x.Id == id);

            // Assert
            Assert.Null(exception);
            Assert.Null(deletedService);
            Assert.Equal(2, res.Count);

        }

        [Fact]
        public async Task GetDoctorByEmailAsync_NotFound()
        {
            // Arrange
            string email = "someemail22";
            IEnumerable<DoctorModel> doctors = [
                    new()
                        {
                            Id = "id1",
                            FirstName = "name1",
                            LastName = "lastname1",
                            Email = "someemail1",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id2",
                            FirstName = "name2",
                            LastName = "lastname2",
                            Email = "someemail2",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id3",
                            FirstName = "name3",
                            LastName = "lastname3",
                            Email = "someemail3",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 2
                        }
                ];
            IEnumerable<CategoryModel> categories = [
                    new()
                    {
                        CategoryName = "category1"
                    },
                    new()
                    {
                        CategoryName = "category2"
                    },
                    new()
                    {
                        CategoryName = "category3"
                    }
                ];
            _appContext.AddRange( doctors );
            _appContext.AddRange( categories );
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.GetDoctorByEmailAsync(email));

            // Assert
            Assert.IsType<UserNotFoundException>(exception);
            Assert.Equal("doctor not found", exception.Message);

        }

        [Fact]
        public async Task GetDoctorByEmailAsync_ReturnTheDoctorInfoResponse()
        {
            // Arrange
            string email = "someemail2";
            IEnumerable<DoctorModel> doctors = [
                    new()
                        {
                            Id = "id1",
                            FirstName = "name1",
                            LastName = "lastname1",
                            Email = "someemail1",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id2",
                            FirstName = "name2",
                            LastName = "lastname2",
                            Email = "someemail2",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id3",
                            FirstName = "name3",
                            LastName = "lastname3",
                            Email = "someemail3",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 2
                        }
                ];
            IEnumerable<CategoryModel> categories = [
                    new()
                    {
                        CategoryName = "category1"
                    },
                    new()
                    {
                        CategoryName = "category2"
                    },
                    new()
                    {
                        CategoryName = "category3"
                    }
                ];
            _appContext.AddRange(doctors);
            _appContext.AddRange(categories);
            _appContext.SaveChanges();

            // Act
            var doctor = await _sut.GetDoctorByEmailAsync(email);
            
            // Assert
            Assert.IsType<DoctorInfoResponse>(doctor);
            Assert.Equal("lastname2", doctor.LastName);
            Assert.Equal("someemail2", doctor.Email);
            Assert.Equal("category1", doctor.CategoryName);

        }

        [Fact]
        public async Task GetDoctorByIDAsync_NotFound()
        {
            // Arrange
            string ID = "id12";
            IEnumerable<DoctorModel> doctors = [
                    new()
                        {
                            Id = "id1",
                            FirstName = "name1",
                            LastName = "lastname1",
                            Email = "someemail1",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id2",
                            FirstName = "name2",
                            LastName = "lastname2",
                            Email = "someemail2",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id3",
                            FirstName = "name3",
                            LastName = "lastname3",
                            Email = "someemail3",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 2
                        }
                ];
            IEnumerable<CategoryModel> categories = [
                    new()
                    {
                        CategoryName = "category1"
                    },
                    new()
                    {
                        CategoryName = "category2"
                    },
                    new()
                    {
                        CategoryName = "category3"
                    }
                ];
            _appContext.AddRange(doctors);
            _appContext.AddRange(categories);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.GetDoctorByIdAsync(ID));

            // Assert
            Assert.IsType<UserNotFoundException>(exception);
            Assert.Equal("doctor not found", exception.Message);

        }

        [Fact]
        public async Task GetDoctorByIDAsync_ReturnTheDoctorInfoResponse()
        {
            // Arrange
            string ID = "id2";
            IEnumerable<DoctorModel> doctors = [
                    new()
                        {
                            Id = "id1",
                            FirstName = "name1",
                            LastName = "lastname1",
                            Email = "someemail1",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id2",
                            FirstName = "name2",
                            LastName = "lastname2",
                            Email = "someemail2",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id3",
                            FirstName = "name3",
                            LastName = "lastname3",
                            Email = "someemail3",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 2
                        }
                ];
            IEnumerable<CategoryModel> categories = [
                    new()
                    {
                        CategoryName = "category1"
                    },
                    new()
                    {
                        CategoryName = "category2"
                    },
                    new()
                    {
                        CategoryName = "category3"
                    }
                ];
            _appContext.AddRange(doctors);
            _appContext.AddRange(categories);
            _appContext.SaveChanges();

            // Act
            var doctor = await _sut.GetDoctorByIdAsync(ID);

            // Assert
            Assert.IsType<DoctorInfoResponse>(doctor);
            Assert.Equal("lastname2", doctor.LastName);
            Assert.Equal("someemail2", doctor.Email);
            Assert.Equal("category1", doctor.CategoryName);

        }

        [Fact]
        public async Task GetAllDoctorsNameAndIdAsync_NotFound()
        {
            // Arrange

            // Act
            var result = await _sut.GetAllDoctorsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllDoctorsNameAndIdAsync_ReturnTheDoctorInfoResponse()
        {
            // Arrange
            IEnumerable<DoctorModel> doctors = [
                    new()
                        {
                            Id = "id1",
                            FirstName = "name1",
                            LastName = "lastname1",
                            Email = "someemail1",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id2",
                            FirstName = "name2",
                            LastName = "lastname2",
                            Email = "someemail2",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id3",
                            FirstName = "name3",
                            LastName = "lastname3",
                            Email = "someemail3",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 2
                        }
                ];
            _appContext.AddRange(doctors);
            _appContext.SaveChanges();

            // Act
            var result = await _sut.GetAllDoctorsAsync();

            // Assert
            Assert.IsAssignableFrom<IEnumerable<DoctorInfoResponse>>(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetDoctorsByCategoryAsync_NotFound()
        {
            // Arrange
            int categoryId = 1;

            // Act
            var result = await _sut.GetDoctorsByCategoryAsync(categoryId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDoctorsByCategoryAsync_ReturnTheDoctorInfoResponse()
        {
            // Arrange
            int categoryId = 1;
            IEnumerable<DoctorModel> doctors = [
                    new()
                        {
                            Id = "id1",
                            FirstName = "name1",
                            LastName = "lastname1",
                            Email = "someemail1",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id2",
                            FirstName = "name2",
                            LastName = "lastname2",
                            Email = "someemail2",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 1
                        },
                        new()
                        {
                            Id = "id3",
                            FirstName = "name3",
                            LastName = "lastname3",
                            Email = "someemail3",
                            PhoneNumber = "1234134",
                            Description = "description",
                            CategoryId = 2
                        }
                ];
            IEnumerable<CategoryModel> categories = [
                    new()
                    {
                        CategoryName = "category1"
                    },
                    new()
                    {
                        CategoryName = "category2"
                    },
                    new()
                    {
                        CategoryName = "category3"
                    }
                ];
            _appContext.AddRange(doctors);
            _appContext.AddRange(categories);
            _appContext.SaveChanges();

            // Act
            var result = await _sut.GetDoctorsByCategoryAsync(categoryId);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<DoctorInfoResponse>>(result);
            Assert.Equal(2, result.Count());
        }



    }
}
