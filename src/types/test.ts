export interface Task {
    taskId: string;
    title: string;
    description: string;
    startDate: string;
    endDate: string;
    projectId: string;
    taskStatus?: { 
        taskStatusId: string;
        taskStatusName: string;
    };
    priority?: { 
        priorityId: string;
        priorityName: string;
    };
    team?: {
        teamId: string;
        teamName: string;
        teamDescription?: string;
    };
}

export interface TestCase {
    testCaseId: string;
    title: string;
    description: string;
    steps: string;
    expectedResult: string;
    actualResult: string;
    taskId: string;
    createdBy: string;
    testTypeId: string;
    testCaseStatusId: string;
}

export interface CreateTestCaseRequest {
    title: string;
    description: string;
    steps: string;
    expectedResult: string;
    actualResult: string;
}

export type TestRun = {
  testRunId: string;
  title: string;
  description?: string;
  runDate: string;
  executedBy: string;
  createdByTeam?: CreatedByTeam;
  testCasesCount: number;
  testRunStatus?: TestRunStatus
};

export type TestRunStatus = {
    testRunStatusId: string;
    testRunStatusName: string;
}

export type CreateTestRunRequest = {
    title: string;
    description: string;
    runDate: string;
}

export type EditTestRunRequest = {
    title: string;
    description: string;
    testRunStatusId: string;
}

export type CreatedByTeam = {
    teamName: string;
}

export type EditTestCaseRequest = {
    title: string;
    description: string;
    steps: string;
    expectedResult: string;
    actualResult: string;
    testTypeId: string;
    testCaseStatusId: string;
}

export type TestCaseStatus = {
    testCaseStatudId: string;
    testCaseStatusName: string;
}

export type TestType = {
    testTypeId: string;
    testTypeName: string;
}

export interface TaskTest {
  taskId: string;
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  projectId: string;
  taskStatus?: {
    taskStatusId: string;
    taskStatusName: string;
  };
  testCase?: {
    testCaseId: string;
    title: string;
  }[];
  testRunDTO?: {
    testRunId: string;
    title: string;
  }[];
  testRunTestCases?: {
        testRunId: string;
        testCaseId: string;
        actualResult: string;
        testCaseStatus: {
            testCaseStatusId: string;
            testCaseStatusName: string;
        }
  }[];
}

export type UpdateResultRequest = {
    actualResult: string;
    testCaseStatusId: string;
}

export type TestRunTestCaseDTO = {
    message: string;
}

export interface TestService {
    getDoneTasks: () => Promise<Task[]>
    getRelatedTestCases: (taskId: string) => Promise<TestCase[]>;
    getTestCaseDetails: (testCaseId: string) => Promise<TestCase>;
    createTestCase: (taskId: string, model: CreateTestCaseRequest) => Promise<TestCase>;
    getRelatedTestRuns: (taskId: string) => Promise<TestRun[]>;
    createTestRun: (taskId: string, model: CreateTestRunRequest) => Promise<TestRun>;
    editTestRun: (testRunId: string, model: EditTestRunRequest) => Promise<TestRun>;
    editTestCase: (testCaseId: string, model: EditTestCaseRequest) => Promise<TestCase>;
    getAllTestRunStatuses: () => Promise<TestRunStatus[]>;
    getAllTestCaseStatuses: () => Promise<TestCaseStatus[]>;
    getAllTestTypes: () => Promise<TestType[]>;
    deleteTestRun: (testRunId: string) => Promise<boolean>;
    deleteTestCase: (testCaseId: string) => Promise<boolean>;
    getAllTestForTester: () => Promise<TaskTest[]>
    updateResult: (testCaseId: string, testRunId: string, model: UpdateResultRequest) => Promise<TestRunTestCaseDTO>
}