import { Injectable } from '@angular/core';
import {
    CanActivate,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    Router,
    Route
} from '@angular/router';
import { CheckPermissionService } from '../shared/check-permission.service';

import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService, 
        private router: Router, 
        private checkPermissionService: CheckPermissionService
    ) { }

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {
        let url: string = state.url;
        let permissions = null;
        if (next.data && next.data['permissions']) {
            permissions = next.data['permissions'];
        }
        return this.checkLogin(url, permissions);
    }

    checkLogin(url: string, permissions): boolean {
        if (this.authService.isAuthenticated()) {
            if (permissions) {
                const hasPermission = this.checkPermissionService.check(permissions);
                if (!hasPermission) {
                    this.router.navigate(['/']);
                }
                return hasPermission;
            }
            return true;
        }

        // Store the attempted URL for redirecting
        this.authService.redirectUrl = url;

        // Navigate to the login page with extras
        this.router.navigate(['/auth/login']);
        return false;
    }

    canLoad(route: Route): boolean {
        let url = `/${route.path}`;
        return this.checkLogin(url, null);
    }
}
