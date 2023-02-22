import { Injectable } from '@angular/core';

@Injectable()
export class WebUtils {
    round_precision(value, precision) {
        if (!value) {
            return 0;
        } else if (!precision || precision < 0) {
            precision = 1;
        }

        var normalized_value = value / precision;
        var epsilon_magnitude = Math.log(Math.abs(normalized_value)) / Math.log(2);
        var epsilon = Math.pow(2, epsilon_magnitude - 53);
        normalized_value += normalized_value >= 0 ? epsilon : -epsilon;

        var sign = normalized_value < 0 ? -1 : 1;
        var rounded_value = sign * Math.round(Math.abs(normalized_value));
        return rounded_value * precision;
    }

    round_decimals(value, decimals) {
        return this.round_precision(value, Math.pow(10, -decimals));
    }

    lpad(str, size) {
        str = "" + str;
        return new Array(size - str.length + 1).join('0') + str;
    }

    insert_thousand_seps(num) {
        var negative = num[0] === '-';
        num = (negative ? num.slice(1) : num);
        return (negative ? '-' : '') + this.intersperse(num, [3, 0], '.');
    }

    intersperse(str, indices, separator) {
        separator = separator || '';
        var result = [], last = str.length;
        for (var i = 0; i < indices.length; i++) {
            var section = indices[i];
            if (section === -1 || last <= 0) {
                break;
            } else if (section === 0 && i === 0) {
                break;
            } else if (section === 0) {
                section = indices[--i];
            }

            result.push(str.substring(last - section, last));
            last -= section;
        }

        var s = str.substring(0, last);
        if (s) {
            result.push(s);
        }

        return result.reverse().join(separator);
    }

}