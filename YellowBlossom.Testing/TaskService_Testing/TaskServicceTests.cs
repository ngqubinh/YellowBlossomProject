using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using YellowBlossom.Application.DTOs.Task;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Repositories.PMIS;

namespace YellowBlossom.Testing.TaskService_Testing
{
    public class TaskServicceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ILogger<TaskService>> _loggerMock;
        private ApplicationDbContext _dbContext;
        private TaskService _taskService;

        [SetUp]
        public void SetUp()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);

            // Mock HttpContextAccessor
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            // Mock ILogger
            _loggerMock = new Mock<ILogger<TaskService>>();

            // Initialize TaskService
            _taskService = new TaskService(_dbContext, _httpContextAccessorMock.Object, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task CreateTaskAsync_ValidInputAndPermissions_ReturnsTaskDTO()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";

            // Setup priority and task status in the database
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Get the actual IDs that were generated
            var priorityId = priority.PriorityId;
            var taskStatusId = taskStatus.TaskStatusId;

            // Setup HttpContext with valid user and ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Create task request
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Message);
            Assert.AreEqual(request.Title, result.Title);
            Assert.AreEqual(request.Description, result.Description);
            Assert.AreEqual(projectId, result.ProjectId);
            Assert.AreEqual(priorityId, result.PriorityId);
            Assert.AreEqual(taskStatusId, result.TastStatusId);

            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNotNull(taskInDb);
            Assert.AreEqual(userId, taskInDb.CreatedBy);
        }

        [Test]
        public async Task CreateTaskAsync_MissingDefaultPriority_ReturnsErrorMessage()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";

            // Setup task status but not priority
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Setup HttpContext with valid user and ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Create task request
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual($"This priority {StaticPriority.Medium} does not exist.", result.Message);
            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNull(taskInDb); // Task should not be created
        }

        [Test]
        public async Task CreateTaskAsync_NullUserId_ReturnsErrorMessage()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            // Setup HttpContext with no user ID
            var claims = Array.Empty<Claim>();
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("User ID not found in HttpContext.", result.Message);
        }

        [Test]
        public async Task CreateTaskAsync_MissingDefaultTaskStatus_ReturnsErrorMessage()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";

            // Setup priority but not task status
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            _dbContext.Priorities.Add(priority);
            await _dbContext.SaveChangesAsync();

            // Setup HttpContext with valid user and ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Create task request
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual($"This status {StaticTaskStatus.Open} does not exist.", result.Message);
            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNull(taskInDb); // Task should not be created
        }

        // Test Case 3: Non-ProjectManager Role
        [Test]
        public async Task CreateTaskAsync_NonProjectManagerRole_ReturnsErrorMessage()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";

            // Setup priority and task status in the database
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Setup HttpContext with a non-ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "RegularUser") // Not ProjectManager
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Create task request
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("You do not have permission to create a task.", result.Message);
            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNull(taskInDb); // Task should not be created
        }

        // Test Case 4: Null HttpContext
        [Test]
        public async Task CreateTaskAsync_NullHttpContext_ReturnsErrorMessage()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            // Setup priority and task status in the database
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Setup HttpContext as null
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            // Create task request
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("HttpContext or User is null.", result.Message);
            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNull(taskInDb); // Task should not be created
        }

        // Test Case 5: Missing User ID in HttpContext
        [Test]
        public async Task CreateTaskAsync_MissingUserId_ReturnsErrorMessage()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            // Setup priority and task status in the database
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Setup HttpContext with no user ID
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Create task request
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("User ID not found in HttpContext.", result.Message);
            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNull(taskInDb); // Task should not be created
        }

        // Test Case 6: Invalid Date Range (EndDate before StartDate)
        [Test]
        public async Task CreateTaskAsync_InvalidDateRange_ReturnsTaskDTO()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";

            // Setup priority and task status in the database
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Get the actual IDs that were generated
            var priorityId = priority.PriorityId;
            var taskStatusId = taskStatus.TaskStatusId;

            // Setup HttpContext with valid user and ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Create task request with invalid date range
            var request = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(-1), // EndDate before StartDate
                AssignedTo = Guid.Empty
            };

            // Act
            var result = await _taskService.CreateTaskAsync(projectId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Message); // Assuming no validation for date range in current code
            Assert.AreEqual(request.Title, result.Title);
            Assert.AreEqual(request.Description, result.Description);
            Assert.AreEqual(projectId, result.ProjectId);
            Assert.AreEqual(priorityId, result.PriorityId);
            Assert.AreEqual(taskStatusId, result.TastStatusId);

            var taskInDb = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Title == request.Title);
            Assert.IsNotNull(taskInDb);
            Assert.AreEqual(userId, taskInDb.CreatedBy);
            Assert.AreEqual(request.EndDate, taskInDb.EndDate);
        }

        [Test]
        public async Task GetListOfTasksAsync_ValidInputAndTasksExist_ReturnsTaskDTOList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";

            // Setup priority and task status (let EF generate IDs)
            var priority = new PMIS_Priority { PriorityName = StaticPriority.Medium };
            var taskStatus = new PMIS_TaskStatus { TaskStatusName = StaticTaskStatus.Open };
            _dbContext.Priorities.Add(priority);
            _dbContext.TaskStatuses.Add(taskStatus);
            await _dbContext.SaveChangesAsync();

            // Create tasks using generated IDs
            var tasks = new List<PMIS_Task>
            {
                new PMIS_Task { ProjectId = projectId, Title = "Task 1", PriorityId = priority.PriorityId, TaskStatusId = taskStatus.TaskStatusId, CreatedBy = userId },
                new PMIS_Task { ProjectId = projectId, Title = "Task 2", PriorityId = priority.PriorityId, TaskStatusId = taskStatus.TaskStatusId, CreatedBy = userId }
            };
            _dbContext.Tasks.AddRange(tasks);
            await _dbContext.SaveChangesAsync();

            // Setup HttpContext with valid user and ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _taskService.GetListOfTasksAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(t => t.Title == "Task 1"));
            Assert.IsTrue(result.Any(t => t.Title == "Task 2"));
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving task data from database.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Mapping task to DTO.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Test]
        public async Task GetListOfTasksAsync_ValidInputNoTasks_ReturnsEmptyList()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string userId = "test-user-id";

            // Setup HttpContext with valid user and ProjectManager role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, StaticUserRole.ProjectManager)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthtype");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            this._httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _taskService.GetListOfTasksAsync(projectId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("There are no tasks.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Test]
        public async Task GetListOfTasksAsync_NullHttpContext_ReturnsEmptyList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            // Act
            var result = await _taskService.GetListOfTasksAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HttpContextAccessor or HttpContext is null.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Test]
        public async Task GetListOfTasksAsync_NullUser_ReturnsEmptyList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var httpContext = new DefaultHttpContext { User = null };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _taskService.GetListOfTasksAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User ID not found in HttpContext.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Test]
        public async Task GetListOfTasksAsync_NullUserId_ReturnsEmptyList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var claims = Array.Empty<Claim>(); // No NameIdentifier claim
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _taskService.GetListOfTasksAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User ID not found in HttpContext.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Test]
        public async Task GetListOfTasksAsync_NonProjectManagerRole_ReturnsEmptyList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "test-user-id";
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "RegularUser") // Not ProjectManager
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = await _taskService.GetListOfTasksAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User does not have permission to get data.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }
    }
}
