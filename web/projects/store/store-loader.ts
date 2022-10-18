import { Action, Func } from "./store-delegate";
import { State } from "./store-abstract";

export class Store {
    private static states: Map<string, any> = new Map();
    static create<T>(key: string, state: State<T>) {
        this.states.set(key, state);
    }
    static get(key: string): any {
        return this.states.get(key).data;
    }
    static action(key: string, actionKey: string) {
        let state: any = this.states.get(key);
        return (args: any = null) => {
            if (typeof state[actionKey] != 'undefined')
                state[actionKey](state.data, args);
            else {
                let currentAction: Action<any> | undefined = state.actions.get(actionKey);
                if (typeof currentAction != 'undefined')
                    currentAction(state, args);
            }
        }
    }
    static func(key: string, funcKey: string) {
        let state: any = this.states.get(key);
        return (args: any = null) => {
            if (typeof state[funcKey] != 'undefined')
                return state[funcKey](state.data, args);
            else {
                let currentFunc: Action<any> | undefined = state.actions.get(funcKey);
                if (typeof currentFunc != 'undefined')
                    return currentFunc(state, args);
            }
        }

    }
}