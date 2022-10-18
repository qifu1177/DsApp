import { NgModule,ModuleWithProviders,APP_INITIALIZER  } from '@angular/core';
import{Store} from "./store-loader";
import{State} from "./store-abstract";

export interface StoreConfig<T>{
    key:string;
    state:State<T>;
}

export function init<T>(config:StoreConfig<T>) {
	return Store.create(config.key,config.state);
}
@NgModule({
	
	providers: [
		Store		
	],
})

export class StoreModule{
    static forRoot<T>(config:StoreConfig<T>): ModuleWithProviders<StoreModule> {
        init(config);
		return {
			ngModule: StoreModule,
			providers: [				
				Store
			],
		};
	}

}