import { values } from 'lodash';
import { Injectable } from '@angular/core';
import { DataResult, State } from '@progress/kendo-data-query';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';

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
    debugger
    var array = localStorage.getItem(key);
    const seen = array != null ? localStorage.getItem(key).split(',') :  new Array;
    if (seen.length > 0) {       
        for (let value of values) {
            var index = seen.find(s => s == value);
            if (index) {
               continue;
            }
            seen.push(value);
        }   

        return seen;
    }else{
        values.forEach(element => {
            seen.push(element);
        }); 
    }

    return seen;
};

@Injectable()
export class StatePersistingService {
    constructor(private sessionInfoStorageService: SessionInfoStorageService) { }
    public get<T>(token: string): T {
        const settings = localStorage.getItem(token);
        return settings ? JSON.parse(settings) : settings;
    }

    public set<T>(token: string, values: any): void {
        localStorage.setItem(token, JSON.stringify(getCircularReplacer(token, values)));
    }


}
