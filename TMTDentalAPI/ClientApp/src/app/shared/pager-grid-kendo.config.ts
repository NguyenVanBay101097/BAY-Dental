import { InjectionToken } from '@angular/core';

export const PAGER_GRID_CONFIG = new InjectionToken<PageGridConfig>('pager-grid-kendo.config');

export interface PageGridConfig {
    pagerSettings?: any;
    pagerSettingsPopup?: any;
}

export const PAGER_CONFIG: PageGridConfig = {
    pagerSettings: { pageSizes : [20, 50, 100, 200] },
    pagerSettingsPopup: { pageSizes : [10, 20, 50, 100, 200] }
}