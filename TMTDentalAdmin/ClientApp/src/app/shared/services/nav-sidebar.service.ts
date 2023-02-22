import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class NavSidebarService {
    collapsed = false;

    toggle() {
        this.collapsed = !this.collapsed;
    }

    set(collapsed) {
        this.collapsed = collapsed;
    }
}