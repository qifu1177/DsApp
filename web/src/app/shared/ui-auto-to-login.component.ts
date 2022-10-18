import { Directive } from '@angular/core';
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { HttpBaseComponent } from "./ui-base-http.component";
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { Store } from "projects/store";

@Directive()
export abstract class AutoToLoginComponent extends HttpBaseComponent {
    waiting: boolean = false;
    constructor(protected _http: HttpClient, protected _translate: TranslateService, protected _router: Router, protected _location: Location, public dialog: MatDialog) {
        super(_http, _translate, _router, _location, dialog);
        if (!Store.func('user', 'checkLogin')())
            this._router.navigate(['user-login']);
        else {
            this.autoCheck();
        }
    }
    autoCheck() {
        if (Store.func('user', 'checkLogin')()) {
            let that = this;
            setTimeout(function () {
                that.autoCheck();
            }, 10000);
            
        } else {
            Store.action('user', 'logout')();
            this._router.navigate(['user-login']);
        }
    }


}


