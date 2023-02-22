import { Injectable } from '@angular/core';

@Injectable()
export class AppLoadingService {
    loading = false;

    setLoading(value: boolean) {
        this.loading = value;
    }
}