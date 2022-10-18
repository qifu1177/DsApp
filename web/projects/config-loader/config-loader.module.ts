import { NgModule,ModuleWithProviders,APP_INITIALIZER  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import {ConfigLoaderService} from './config-loader.service';

export function initConfig(configSvc: ConfigLoaderService) {
	return () => configSvc.loadConfig();
}

@NgModule({
	imports: [HttpClientModule],
	providers: [
		ConfigLoaderService,
		{
			provide: APP_INITIALIZER,
			useFactory: initConfig,
			deps: [ConfigLoaderService],
			multi: true,
		},
	],
})
export class ConfigLoaderModule {
	static forRoot(): ModuleWithProviders<ConfigLoaderModule> {
		return {
			ngModule: ConfigLoaderModule,
			providers: [				
				ConfigLoaderService,
			],
		};
	}
}
