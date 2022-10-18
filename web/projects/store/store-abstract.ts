import { Action, Func } from "./store-delegate";
export abstract class State<T> {
    data!: T;
    actions: Map<string, Action<T>> = new Map();
    funcs: Map<string, Func<T>> = new Map();
    createAction(action: Action<T>, actionKey: string = '') {
        if (actionKey == '')
            this.actions.set(action.name, action);
        else
            this.actions.set(actionKey, action);
    }
    createFunc(func: Func<T>, funcKey: string = '') {
        if (funcKey == '')
            this.funcs.set(func.name, func);
        else
            this.funcs.set(funcKey, func);
    }
}