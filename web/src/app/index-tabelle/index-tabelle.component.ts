import { Component, Input } from '@angular/core';
import { IndexTabelle } from 'src/models/responses/IndexTabelleResponse';
@Component({
    selector: 'index-tabelle',
    templateUrl: './index-tabelle.component.html'
})
export class IndexTabelleComponent {
    @Input('data') data: IndexTabelle = {
        productivity: 0,
        mtbi: 0,
        standbycount: 0,
        min1statndbycount: 0,
        min10statndbycount: 0,
        min10plusstatndbycount: 0,
        standbyduration: 0,
        addvalue: 0,
        consumption: 0,
        standbyconsumption: 0,
        indexcondition: 0
    };
}