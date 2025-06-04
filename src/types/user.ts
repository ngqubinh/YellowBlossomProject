export interface User {
    fullName: string;
    email: string;
}

export interface Team {
    teamId: string;
    teamName: string;
    teamDescription: string;
    createdDate: string;
    createdBy: string;
    users: User[];
}

export interface CreateTeamRequest {
    teamName: string;
    teamDescription: string;
}

export interface InviteUserRequest {
    email: string;
    teamId: string;
    expiryDays: number;
}

export interface GeneralResponse {
    success: boolean;
    message: string;
}

export interface UserService {
    getAllUsers: () => Promise<User[]>;
    getAllTeams: () => Promise<Team[]>;
    getTeamDetails: (teamId: string) => Promise<Team>;
    createTeam: (model: CreateTeamRequest) => Promise<Team>;
    inviteUserToTeam: (model: InviteUserRequest) => Promise<GeneralResponse>;
}