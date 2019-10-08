import { Injectable } from '@angular/core';

@Injectable()
export class NavSidebarService {
    collapsed = false;

    toggle() {
        this.collapsed = !this.collapsed;
    }
}