
export interface Action<T>{(state:T, args:any):void}
export interface Func<T>{(state:T,args:any):any}