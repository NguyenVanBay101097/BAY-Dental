import { values } from 'lodash';
import { Injectable } from '@angular/core';
import { DataResult, State } from '@progress/kendo-data-query';

export interface GridSettings {
    columnsConfig: ColumnSettings[];
    gridData?: DataResult;
}

export interface ColumnSettings {
    field: string;
    title?: string;
    filter?: 'string' | 'numeric' | 'date' | 'boolean';
    format?: string;
    width?: number;
    _width?: number;
    orderIndex?: number;
    hidden?: boolean;
}

const getCircularReplacer = (key, values) => {
    var array = JSON.parse(localStorage.getItem(key));
    let seen = array == null ? new Array : array;
    if (seen.length > 0) {
        const set1 = new Set(values);
        let i = seen.length;

        while (i--) if (!set1.delete(seen[i])) seen.splice(i, 1);
        seen.push(...set1);
      
        return seen;
    } else {
        values.forEach(element => {
            seen.push(element);
        });
    }

    return seen;
};

@Injectable()
export class StatePersistingService {

    public get(token: string) {
        const settings = localStorage.getItem(token);
        return settings ? JSON.parse(settings) : settings;
    }

    public set(token: string, values: any): void {
        localStorage.setItem(token, JSON.stringify(getCircularReplacer(token, values)));
    }


}
