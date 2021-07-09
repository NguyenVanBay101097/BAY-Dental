import { InjectionToken, Injector } from '@angular/core';
import * as _moment from 'moment';
const moment = _moment;

export const LOCALE_CONFIG = new InjectionToken<LocaleConfig>('daterangepicker.config');
/**
 *  LocaleConfig Interface
 */
export interface LocaleConfig {
    direction?: string;
    separator?: string;
    weekLabel?: string;
    applyLabel?: string;
    cancelLabel?: string;
    clearLabel?: string;
    customRangeLabel?: string;
    daysOfWeek?: string[];
    monthNames?: string[];
    firstDay?: number;
    format?: string;
    displayFormat?: string;
}
/**
 *  DefaultLocaleConfig
 */
export const DefaultLocaleConfig: LocaleConfig = {
    direction: 'ltr',
    separator: ' - ',
    weekLabel: 'W',
    applyLabel: 'Áp dụng',
    cancelLabel: 'Đóng',
    clearLabel: 'Hủy',
    customRangeLabel: 'Chọn ngày',
    daysOfWeek: [
        "CN",
        "T2",
        "T3",
        "T4",
        "T5",
        "T6",
        "t7"
    ],
    monthNames: [
        "Tháng 1",
        "Tháng 2",
        "Tháng 3",
        "Tháng 4",
        "Tháng 5",
        "Tháng 6",
        "Tháng 7",
        "Tháng 8",
        "Tháng 9",
        "Tháng 10",
        "Tháng 11",
        "Tháng 12"
    ],
    firstDay: 1,
    format: "DD/MM/YYYY",
};
