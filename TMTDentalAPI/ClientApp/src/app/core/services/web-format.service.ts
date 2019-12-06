import { WebUtils } from './web-utils.service';
import { sprintf } from 'sprintf-js';
import { Injectable } from '@angular/core';

@Injectable()
export class WebFormat {
    constructor(private webUtils: WebUtils) {
    }

    format_value(value, descriptor, value_if_empty) {
        // If NaN value, display as with a `false` (empty cell)
        if (typeof value === 'number' && isNaN(value)) {
            value = false;
        }
        //noinspection FallthroughInSwitchStatementJS
        switch (value) {
            case '':
                if (descriptor.type === 'char' || descriptor.type === 'text') {
                    return '';
                }
                console.warn('Field', descriptor, 'had an empty string as value, treating as false...');
                return value_if_empty === undefined ? '' : value_if_empty;
            case false:
            case undefined:
            case Infinity:
            case -Infinity:
                return value_if_empty === undefined ? '' : value_if_empty;
        }

        switch (descriptor.type) {
            case 'integer':
                return this.webUtils.insert_thousand_seps(
                    sprintf('%d', value));
            case "float":
                var digits = descriptor.digits ? descriptor.digits : [69, 2];
                digits = typeof (digits) === "string" ? eval(digits) : digits;
                var precision = digits[1];
                var formated = sprintf('%.' + precision + 'f', value).split('.');
                formated[0] = this.webUtils.insert_thousand_seps(formated[0]);
                return formated.join(',');
            default:
                return value;
        }
    }

    parse_value(value, descriptor, value_if_empty) {
        var tmp;
        switch (descriptor.type) {
            case 'float':
                var tmp2 = value;
                do {
                    tmp = tmp2;
                    tmp2 = tmp.replace(".", "");
                } while (tmp !== tmp2);
                var reformatted_value = tmp.replace(",", ".");
                var parsed = Number(reformatted_value);
                if (isNaN(parsed)) {
                    throw new Error(sprintf("'%s' is not a correct float", value));
                }
                return parsed;
        }
    }
}