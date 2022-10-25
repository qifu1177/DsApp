interface StatusData {
    sdt: number;
    edt: number;
    status: string;
}
export interface StatusResponse
{
    value:StatusData[];
}