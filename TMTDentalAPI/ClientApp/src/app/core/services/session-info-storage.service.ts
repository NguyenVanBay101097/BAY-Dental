import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class SessionInfoStorageService {
    constructor() { }

    getSessionInfo() {
        if (localStorage['session_info']) {
            return JSON.parse(localStorage['session_info']);
        }

        return null;
    }

    saveSession(data) {
        localStorage['session_info'] = JSON.stringify(data);
    }
}
