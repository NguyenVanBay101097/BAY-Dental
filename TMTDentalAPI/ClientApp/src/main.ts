import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

import 'hammerjs';

import { Chart, registerables } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';

Chart.register(...registerables);
Chart.register(ChartDataLabels);
Chart.defaults.plugins.datalabels.display = false;

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export function getBaseApi() {
  return environment.apiDomain;
}

const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  { provide: 'BASE_API', useValue: getBaseApi() },
]

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule).then(() => {
    if ('serviceWorker' in navigator && environment.production) {
      navigator.serviceWorker.register('ngsw-worker.js');
    }
  })
  .catch(err => console.error(err));
