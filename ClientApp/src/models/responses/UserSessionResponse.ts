import { SessionResponse } from "./SessionResponse";
export interface UserSessionResponse extends SessionResponse {
    userName: string;
    userId: string;
    updateTs: number;
    email: string;
    roleId: string;
    phoneNumber: string;
    maritalStatus: boolean;
    birthDate: string;
    photoPath: string;
    maxUploadFileSize:number;
}