import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { MemberLevelService } from "./member-level.service";
import { delay, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
@Injectable({
    providedIn: 'root'
})
export class MemberLevelResolve implements Resolve<any>{
    constructor(
        private memberLevelService: MemberLevelService,
        private router: Router
    ) { }
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.memberLevelService.get().pipe(
            catchError(error => {
                this.router.navigateByUrl('/404');
                return of(null);
            }
            ));
    }
}