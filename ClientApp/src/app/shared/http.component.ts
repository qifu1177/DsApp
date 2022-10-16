import { HttpClient} from "@angular/common/http";
import { Directive } from '@angular/core';
import { GlobalConstants } from 'src/common/global-constants';
@Directive()
export abstract class HttpComponent {
    constructor(protected _http:HttpClient) {
        
    }
    createUrl(url:string):string{
        return `${GlobalConstants.apiURL}${url}`;
    }
} 