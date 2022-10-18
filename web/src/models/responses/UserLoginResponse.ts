import {SessionResponse} from "./SessionResponse";
export interface UserLoginResponse extends  SessionResponse{
    userName:string;
    moduleRights:any;
    sessionDuration:number;
    uploadFileMaxSize:number;
    uploadFileTypes:string[];
}