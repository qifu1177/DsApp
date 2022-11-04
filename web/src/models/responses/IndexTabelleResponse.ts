export interface IndexTabelle {
    productivity: number;
    mtbi: number;
    standbycount: number;
    min1statndbycount: number;
    min10statndbycount: number;
    min10plusstatndbycount: number;
    addvalue: number;
    consumption: number;
    standbyconsumption: number;
    indexcondition: number;
}
export interface IndexTabelleStatusResponse {
    value: IndexTabelle;
}
 