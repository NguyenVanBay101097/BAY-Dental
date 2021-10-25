import { Injectable } from '@angular/core';

@Injectable({providedIn: 'root'})
export class AppLoadingService {
    loading = false;

    // 7 fix after change
    setLoading(value: boolean) {
       setTimeout(() => {
        this.loading = value;
       }, 0);
    }
}