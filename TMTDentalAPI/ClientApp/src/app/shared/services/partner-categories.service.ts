import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { ODataService } from './odata.service';

@Injectable({ providedIn: 'root' })
export class PartnerCategoriesService extends ODataService {
    constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "PartnerCategories"); }

    public searchCombobox(q?: string) {
        var state: any = {
            $top: 10
        };

        if (q) {
            state.filter = {
                logic: 'and',
                filters: [
                    { field: 'Name', operator: 'contains', value: q || '' }
                ]
            }
        }

        return this.fetch(this.tableName, state)
            .pipe(
                map((data: GridDataResult) => {
                    return data.data;
                })
            );
    }
}
