export type Bug = {
    bugId: string;
    title: string;
    description: string;
    stepsToReduces: string;
    serverity: string;
    reportedDate: string;
    resolvedDate: string;
    priority?: Priority;
    testRun?: TestRun;
    testCase?: TestCase;
    reportedByTeam?: ReportedByTeam;
}

export type Priority = {
    priorityId: string;
    priorityName: string;
}

export type TestRun = {
    testRunId: string;
    title: string;
}

export type TestCase = {
    testCaseId: string;
    title: string;
}

export type ReportedByTeam = {
    teamId: string;
    teamName: string;
}

export type ResolveBugRequest = {
    StepsToReduce: string;
    Serverity: string;
}



export interface BugService {
    getAllBugs: () => Promise<Bug[]>;
    resolveBug: (bugId: string, model: ResolveBugRequest) => Promise<Bug>;
}