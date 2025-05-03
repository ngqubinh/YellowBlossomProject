using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Infrastructure.Services
{
    public static class Mapper
    {
        public static ProductDTO MapProductToProductDTO(PMIS_Product product)
        {
            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Version = product.Version,
                CreatedBy = product.CreatedBy,
                User = product.User != null ? MapUserToUserDTO(product.User) : null,
                CreatedAt = product.CreatedAt,
                LastUpdated = product.LastUpdated
            };
        }

        public static List<ProductDTO> MapProductToProductDTOByList(List<PMIS_Product> products)
        {
            return products.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Version = product.Version,
                CreatedBy = product.CreatedBy,
                User = product.User != null ? MapUserToUserDTO(product.User) : null,
                CreatedAt = product.CreatedAt,
                LastUpdated = product.LastUpdated
            }).ToList();
        }

        public static UserDTO MapUserToUserDTO(User user)
        {
            return new UserDTO
            {
                Email = user.Email!,
                FullName = user.FullName,
            };
        }

        public static PhaseStatusDTO MapPhaseStatusToPhaseStatusDTO(PMIS_PhaseStatus phaseStatus)
        {
            return new PhaseStatusDTO
            {
                PhaseStatusId = phaseStatus.PhaseStatusId,
                PhaseStatusName = phaseStatus.PhaseStatusName,
            };
        }
    
        public static PhaseDTO MapPhaseToPhaseDTO(PMIS_Phase phase)
        {
            return new PhaseDTO
            {
                PhaseId = phase.PhaseId,
                PhaseName = phase.PhaseName,
                StartDate = phase.StartDate,
                EndDate = phase.EndDate,
                PhaseStatusId = phase.PhaseStatusId,
                PhaseStatusDTO = phase.PhaseStatus != null ? MapPhaseStatusToPhaseStatusDTO(phase.PhaseStatus) : null,
                ProjectId = phase.ProjectId,
                ProjectDTO = phase.Project != null ? MapProjectToProjectDTO(phase.Project) : null,
            };
        }

        public static ProjectStatusDTO MapProjectStatusToProjectStatusDTO(PMIS_ProjectStatus projectStatus)
        {
            return new ProjectStatusDTO
            {
                ProjectStatusId = projectStatus.ProjectStatusId,
                ProjectStatusName = projectStatus.ProjectStatusName,
            };
        }

        public static ProjectTypeDTO MapProjectTypeToProjectTypeDTO(PMIS_ProjectType projectType)
        {
            return new ProjectTypeDTO
            {
                ProjectTypeId = projectType.ProjectTypeId,
                ProjectTypeName = projectType.ProjectTypeName,
            };
        }

        public static ProjectDTO MapProjectToProjectDTO(PMIS_Project project)
        {
            return new ProjectDTO
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectStatusId = project.ProjectStatusId,
                ProjectStatusDTO = project.ProjectStatus != null ? MapProjectStatusToProjectStatusDTO(project.ProjectStatus) : null,
                ProductId = project.ProductId,
                ProductDTO = project.Product != null ? MapProductToProductDTO(project.Product) : null,
                UserId = project.UserId,
                UserDTO = project.User != null ? MapUserToUserDTO(project.User) : null,
                ProjectTypeId = project.ProjectTypeId,
                ProjectTypeDTO = project.ProjectType != null ? MapProjectTypeToProjectTypeDTO(project.ProjectType) : null,
            };
        }

        public static List<ProjectDTO> MapProjectToProjectDTOByList(List<PMIS_Project> projects)
        {
            return projects.Select(project => new ProjectDTO
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectStatusId = project.ProjectStatusId,
                ProjectStatusDTO = project.ProjectStatus != null ? MapProjectStatusToProjectStatusDTO(project.ProjectStatus) : null,
                ProductId = project.ProductId,
                ProductDTO = project.Product != null ? MapProductToProductDTO(project.Product) : null,
                UserId = project.UserId,
                UserDTO = project.User != null ? MapUserToUserDTO(project.User) : null,
                ProjectTypeId = project.ProjectTypeId,
                ProjectTypeDTO = project.ProjectType != null ? MapProjectTypeToProjectTypeDTO(project.ProjectType) : null
            }).ToList();
        }
    
        public static PriorityDTO MapPriorityToPriorityDTO(PMIS_Priority priority)
        {
            return new PriorityDTO
            {
                PriorityId = priority.PriorityId,
                PriorityName = priority.PriorityName,
                PriorityDescription = priority.PriorityDescription,
            };
        }
    
        public static TaskStatusDTO MapTaskStatusToTastStatusDTO(PMIS_TaskStatus taskStatus)
        {
            return new TaskStatusDTO
            {
                TaskStatusId = taskStatus.TaskStatusId,
                TaskStatusName = taskStatus.TaskStatusName,
            };
        }
        
        public static TaskDTO MapTaskToTaskDTO(PMIS_Task task)
        {
            return new TaskDTO
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                PriorityId = task.PriorityId,
                TastStatusId = task.TaskStatusId,
                ProjectId = task.ProjectId,
                CreatedBy = task.CreatedBy,
                AssignedTo = task.AssignedTeam,
                Priority = task.Priority != null ? MapPriorityToPriorityDTO(task.Priority) : null,
                TaskStatus = task.TaskStatus != null ? MapTaskStatusToTastStatusDTO(task.TaskStatus) : null,
                Project = task.Project != null ? MapProjectToProjectDTO(task.Project) : null,
                User = task.User != null ? MapUserToUserDTO(task.User) : null,
            };
        }
    
        public static List<TaskDTO> MapTaskToTaskDTOByList(List<PMIS_Task> tasks)
        {
            return tasks.Select(task => new TaskDTO
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                PriorityId = task.PriorityId,
                TastStatusId = task.TaskStatusId,
                ProjectId = task.ProjectId,
                CreatedBy = task.CreatedBy,
                AssignedTo = task.AssignedTeam,
                Priority = task.Priority != null ? MapPriorityToPriorityDTO(task.Priority) : null,
                TaskStatus = task.TaskStatus != null ? MapTaskStatusToTastStatusDTO(task.TaskStatus) : null,
                Project = task.Project != null ? MapProjectToProjectDTO(task.Project) : null,
                User = task.User != null ? MapUserToUserDTO(task.User) : null,
            }).ToList();
        }

        public static TeamDTO MapTeamToTeamDTO(PMIS_Team team)
        {
            return new TeamDTO
            {
                TeamId = team.TeamId,
                TeamDescription = team.TeamDescription,
                TeamName = team.TeamName,
                CreatedBy = team.CreatedBy,
                CreatedDate = team.CreatedDate,
            };
        }
    
        public static List<TeamDTO> MapTeamToTeamDTOByList(List<PMIS_Team> teams)
        {
            return teams.Select(team => new TeamDTO
            {
                TeamId = team.TeamId,
                TeamDescription = team.TeamDescription,
                TeamName = team.TeamName,
                CreatedBy = team.CreatedBy,
                CreatedDate = team.CreatedDate,
            }).ToList();
        }
    
        public static UserTeamDTO MapUserTeamToUserTeamDTO(PMIS_UserTeam userTeam)
        {
            return new UserTeamDTO
            {
                UserId = userTeam.UserId,
                TeamId = userTeam.TeamId,
                AssignedDate = userTeam.AssignedDate,
            };
        }
    
        public static ProjectTeamDTO MapProjectTeamToProjectTeamDTO(PMIS_ProjectTeam projectTeam)
        {
            return new ProjectTeamDTO
            {
                ProjectId = projectTeam.ProjectId,
                TeamId = projectTeam.TeamId,
                RoleOfTeam = projectTeam.RoleOfTeam,
                AssignedDate = projectTeam.AssignedDate,
                CreatedBy = projectTeam.CreatedBy,
                Project = projectTeam.Project != null ? MapProjectToProjectDTO(projectTeam.Project) : null,
                User = projectTeam.User != null ? MapUserToUserDTO(projectTeam.User) : null,
            };
        }
    
        public static TestTypeDTO MapTestTypeToTestTypeDTO(PMIS_TestType testType)
        {
            return new TestTypeDTO
            {
                TestTypeId = testType.TestTypeId,
                TestTypeName = testType.TestTypeName,
                TestDescription = testType.TestDescription,
            };
        }
    
        public static TestCaseStatusDTO MapTestCaseStatusToTestCaseStatusDTO(PMIS_TestCaseStatus tcs)
        {
            return new TestCaseStatusDTO
            {
                TestCaseStatudId = tcs.TestCaseStatusId,
                TestCaseStatusName = tcs.TestCaseStatusName,
            };
        }
    
        public static TestCaseDTO MapTestCaseToTestCaseDTO(PMIS_TestCase testCase)
        {
            return new TestCaseDTO
            {
                TestCaseId = testCase.TestCaseId,
                Title = testCase.Title,
                Description = testCase.Description,
                Steps = testCase.Steps,
                ExpectedResult = testCase.ExpectedResult,
                ActualResult = testCase.ActualResult,
                TaskId = testCase.TaskId,
                CreatedBy = testCase.CreateBy,
                TestTypeId = testCase.TestTypeId,
                TestCaseStatusId = testCase.TestCaseStatusId,
                Task = testCase.Task != null ? MapTaskToTaskDTO(testCase.Task) : null,
                Team = testCase.Team != null ? MapTeamToTeamDTO(testCase.Team) : null,
                TestType = testCase.TestType != null ? MapTestTypeToTestTypeDTO(testCase.TestType) : null,
                TestCaseStatus = testCase.TestCaseStatus != null ? MapTestCaseStatusToTestCaseStatusDTO(testCase.TestCaseStatus) : null
            };
        }
    
        public static TestRunStatusDTO MapTestRunStatusToTestRunStatusDTO(PMIS_TestRunStatus testRunStatus)
        {
            return new TestRunStatusDTO
            {
                TestRunStatusId = testRunStatus.TestRunStatusId,
                TestRunStatusName = testRunStatus.TestRunStatusName,
            };
        }
    
        public static TestRunDTO MapTestRunToTestRunDTO(PMIS_TestRun testRun)
        {
            return new TestRunDTO
            {
                TestRunId = testRun.TestRunId,
                Title = testRun.Title,
                Description = testRun.Description,
                RunDate = testRun.RunDate,
                TaskId = testRun.TaskId,
                CreatedBy = testRun.CreatedBy,
                ExecutedBy = testRun.ExecutedBy,
                TestRunStatusId = testRun.TestRunStatusId,
                Task = testRun.Task != null ? MapTaskToTaskDTO(testRun.Task) : null,
                CreatedByTeam = testRun.CreatedByTeam != null ? MapTeamToTeamDTO(testRun.CreatedByTeam) : null,
                ExecutedByTeam = testRun.ExecutedByTeam != null ? MapTeamToTeamDTO(testRun.ExecutedByTeam) : null,
                TestRunStatus = testRun.TestRunStatus != null ? MapTestRunStatusToTestRunStatusDTO(testRun.TestRunStatus) : null,
            };
        }
    
        public static TestRunTestCaseDTO MapTestRunTestCaseToTestRunTestCaseDTO(PMIS_TestRunTestCase trtc)
        {
            return new TestRunTestCaseDTO
            {
                TestRunId = trtc.TestRunId,
                TestCaseId = trtc.TestCaseId,
                ActualResult = trtc.ActualResult,
                TestRun = trtc.TestRun != null ? MapTestRunToTestRunDTO(trtc.TestRun) : null,
                TestCase = trtc.TestCaseStatus != null ? MapTestCaseToTestCaseDTO(trtc.TestCase) : null,
                TestCaseStatusId = trtc.TestCaseStatusId,
                TestCaseStatus = trtc.TestCaseStatus != null ? MapTestCaseStatusToTestCaseStatusDTO(trtc.TestCaseStatus) : null,
            };
        }
    
        public static BugDTO MapBugToBugDTO(PMIS_Bug bug)
        {
            return new BugDTO
            {
                BugId = bug.BugId,
                Title = bug.Title,
                Description = bug.Description,
                StepsToReduces = bug.StepsToReduce,
                Serverity = bug.Serverity,
                ReportedDate = bug.ReportedDate,
                ResolvedDate = bug.ResolvedDate,
                ReportedByTeamId = bug.ReportedByTeamId,
                AssginedToTeamId = bug.AssignedToTeamId,
                PriorityId = bug.PriorityId,
                TestRunId = bug.TestRunId,
                TestCaseId = bug.TestCaseId,
                Priority = bug.Priority != null ? MapPriorityToPriorityDTO(bug.Priority) : null,
                TestRun = bug.TestRun != null ? MapTestRunToTestRunDTO(bug.TestRun) : null,
                TestCase = bug.TestCase != null ? MapTestCaseToTestCaseDTO(bug.TestCase) : null,
                ReportedByTeam = bug.ReportedByTeam != null ? MapTeamToTeamDTO(bug.ReportedByTeam) : null,
                AssignedToTeam = bug.AssignedToTeam != null ? MapTeamToTeamDTO(bug.AssignedToTeam) : null,
            };
        }
    
        public static List<BugDTO> MapBugToBugDTOByList(List<PMIS_Bug> bugs)
        {
            return bugs.Select(bug => new BugDTO
            {
                BugId = bug.BugId,
                Title = bug.Title,
                Description = bug.Description,
                StepsToReduces = bug.StepsToReduce,
                Serverity = bug.Serverity,
                ReportedDate = bug.ReportedDate,
                ResolvedDate = bug.ResolvedDate,
                ReportedByTeamId = bug.ReportedByTeamId,
                AssginedToTeamId = bug.AssignedToTeamId,
                PriorityId = bug.PriorityId,
                TestRunId = bug.TestRunId,
                TestCaseId = bug.TestCaseId,
                Priority = bug.Priority != null ? MapPriorityToPriorityDTO(bug.Priority) : null,
                TestRun = bug.TestRun != null ? MapTestRunToTestRunDTO(bug.TestRun) : null,
                TestCase = bug.TestCase != null ? MapTestCaseToTestCaseDTO(bug.TestCase) : null,
                ReportedByTeam = bug.ReportedByTeam != null ? MapTeamToTeamDTO(bug.ReportedByTeam) : null,
                AssignedToTeam = bug.AssignedToTeam != null ? MapTeamToTeamDTO(bug.AssignedToTeam) : null,
            }).ToList();
        }
    }
}
