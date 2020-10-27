import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

@Injectable({ providedIn: 'root' })
export class IRSequencesService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "IRSequences"); }

    getByCode(code: string) {
        return this.fetch(this.tableName, {
            filter: {
                logic: 'and',
                filters: [
                    { field: "Code", operator: "eq", value: code }
                ]
            }
        });
    }
}
